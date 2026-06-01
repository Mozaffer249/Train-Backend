import { useState, useEffect, useMemo } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Clock, MapPin, Users, Filter, SortAsc, Train as TrainIcon } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';
import { catalogApi } from '../services/api';
import { addMinutesToIso, formatDateSafe, formatDurationSafe, formatTimeSafe, shouldShowTripForSearchDate } from '../utils/dateUtils';

interface SearchState {
  originStationId: number;
  destinationStationId: number;
  originName: string;
  destinationName: string;
  date: string;
  passengers: number;
  class: string;
}

interface TripResult {
  id: number;
  trainName: string;
  trainNumber: string;
  routeId: number;
  // Segment-specific times, computed from route stop offsets — not the full
  // trip endpoints. The customer is buying a leg, not the whole journey.
  departureISO: string;
  arrivalISO: string;
  availableSeats: number;
  price: number;
  currency: string;
  coachClassId: number; // 1=First, 2=Second, 3=Third
  // Where the resolved price came from. Drives the small scope chip on the
  // card: 'trip' / 'segment' override is shown; plain 'route' stays unbadged.
  fareScope: 'route' | 'segment' | 'trip' | null;
  // Class label of the resolved cheapest fare ("First"/"Second"/"Third").
  // Shown next to the price so the customer knows what they're seeing.
  fareClass: string | null;
}

function coachClassToId(c: string): number {
  switch ((c || '').toLowerCase()) {
    case 'first':
    case 'business':
      return 1;
    case 'second':
    case 'standard':
      return 2;
    case 'third':
    case 'economy':
    default:
      return 3;
  }
}

export default function SearchResults() {
  const location = useLocation();
  const navigate = useNavigate();
  const { t } = useLanguage();

  const params = location.state as SearchState | null;

  const [trips, setTrips] = useState<TripResult[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [sortBy, setSortBy] = useState<'time' | 'price' | 'duration'>('time');
  const [priceRange, setPriceRange] = useState<[number, number]>([0, 100000]);

  useEffect(() => {
    if (!params?.originStationId || !params?.destinationStationId) {
      setLoading(false);
      return;
    }
    let cancelled = false;
    (async () => {
      setLoading(true);
      setError('');
      try {
        const coachClassId = coachClassToId(params.class);
        const routes = await catalogApi.getRoutes({
          originStationId: params.originStationId,
          destinationStationId: params.destinationStationId,
          isActive: true,
        });

        // Per route: find this segment's departure/arrival offsets relative
        // to the trip's departureTime. Origin = 0, intermediate = stop's
        // (departure|arrival)Offset, Destination = max(stop offset) (we use
        // arrival to be conservative; for trips the API returns absolute arrivalTime
        // for the route end, but we won't need it because the destination case
        // simply uses the trip's arrivalTime when boarding/alighting align with route ends).
        type SegOff = { departureOffset: number | string | null; arrivalOffset: number | string | null };
        const segOffsetsForRoute = (route: typeof routes[number]): SegOff => {
          const isOrigin = route.origin?.id === params.originStationId;
          const isDest = route.destination?.id === params.destinationStationId;
          const stops = route.intermediateStops ?? [];

          const depOffset = isOrigin
            ? 0
            : stops.find((s) => s.stationId === params.originStationId)?.departureOffset ?? null;

          const arrOffset = isDest
            ? null // use trip.arrivalTime
            : stops.find((s) => s.stationId === params.destinationStationId)?.arrivalOffset ?? null;

          return { departureOffset: depOffset, arrivalOffset: arrOffset };
        };

        // Collect trips with placeholder fare (resolved next pass in parallel).
        const tripBuckets: Array<{ trip: TripResult }> = [];
        const fareFetches: Promise<{ id: number; price: number; currency: string; fareScope: 'route' | 'segment' | 'trip' | null; fareClass: string | null }>[] = [];

        for (const route of routes) {
          const off = segOffsetsForRoute(route);
          const routeTrips = await catalogApi.getTrips({ routeId: route.id, date: params.date });
          for (const trip of routeTrips) {
            const segDeparture = off.departureOffset == null
              ? trip.departureTime
              : addMinutesToIso(trip.departureTime, off.departureOffset);
            const segArrival = off.arrivalOffset == null
              ? trip.arrivalTime
              : addMinutesToIso(trip.departureTime, off.arrivalOffset);

            if (!shouldShowTripForSearchDate(params.date, segDeparture)) {
              continue;
            }

            tripBuckets.push({
              trip: {
                id: trip.id,
                trainName: trip.trainName,
                trainNumber: trip.trainNumber,
                routeId: route.id,
                departureISO: segDeparture,
                arrivalISO: segArrival,
                availableSeats: trip.availableSeats,
                price: 0,
                currency: 'SDG',
                coachClassId,
                fareScope: null,
                fareClass: null,
              },
            });

            fareFetches.push(
              // No class arg → backend returns the cheapest available fare
              // for this trip+segment across any class. With a class arg → strict
              // match. The card shows the resolved class either way.
              catalogApi
                .getApplicableFare(trip.id, params.originStationId, params.destinationStationId)
                .then((fare) => {
                  // Resolution priority on the server is Trip > Segment > Route.
                  // We mirror that classification here so the card can show a chip
                  // when the customer is being shown an override price.
                  const fareScope: 'route' | 'segment' | 'trip' | null = !fare
                    ? null
                    : fare.tripId
                      ? 'trip'
                      : fare.originStationId && fare.destinationStationId
                        ? 'segment'
                        : 'route';
                  return {
                    id: trip.id,
                    price: fare?.finalPrice || fare?.basePrice || 0,
                    currency: fare?.currency || 'SDG',
                    fareScope,
                    fareClass: typeof fare?.coachClass === 'string' ? fare.coachClass : null,
                  };
                })
                // No fare configured for this trip+segment yet — show 0 (UI shows '—').
                .catch(() => ({ id: trip.id, price: 0, currency: 'SDG', fareScope: null, fareClass: null })),
            );
          }
        }

        const fares = await Promise.all(fareFetches);
        const fareById = new Map(fares.map((f) => [f.id, f]));
        const collected = tripBuckets.map(({ trip }) => {
          const f = fareById.get(trip.id);
          return f
            ? { ...trip, price: f.price, currency: f.currency, fareScope: f.fareScope, fareClass: f.fareClass }
            : trip;
        });

        if (!cancelled) setTrips(collected);
      } catch (err) {
        if (!cancelled) setError(err instanceof Error ? err.message : t('error'));
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [params?.originStationId, params?.destinationStationId, params?.date]);

  const visibleTrips = useMemo(() => {
    return [...trips]
      .filter((tr) => shouldShowTripForSearchDate(params?.date, tr.departureISO))
      .filter((tr) => tr.price >= priceRange[0] && tr.price <= priceRange[1])
      .sort((a, b) => {
        if (sortBy === 'price') return a.price - b.price;
        if (sortBy === 'duration') {
          const da = new Date(a.arrivalISO).getTime() - new Date(a.departureISO).getTime();
          const db = new Date(b.arrivalISO).getTime() - new Date(b.departureISO).getTime();
          return da - db;
        }
        return new Date(a.departureISO).getTime() - new Date(b.departureISO).getTime();
      });
  }, [trips, sortBy, priceRange, params?.date]);

  const handleBooking = (trip: TripResult) => {
    navigate('/book', {
      state: {
        trip,
        originName: params?.originName,
        destinationName: params?.destinationName,
        boardingStationId: params?.originStationId,
        alightingStationId: params?.destinationStationId,
        passengers: params?.passengers ?? 1,
        coachClass: params?.class ?? 'economy',
        coachClassId: trip.coachClassId,
      },
    });
  };

  if (!params?.originStationId) {
    return (
      <div className="min-h-[60vh] flex flex-col items-center justify-center text-center px-4">
        <TrainIcon className="h-12 w-12 text-gray-400 mb-4" />
        <p className="text-gray-600 mb-4">{t('search.trains')}</p>
        <button onClick={() => navigate('/')} className="bg-sudan-green-600 text-white px-4 py-2 rounded-lg hover:bg-sudan-green-700">
          {t('home')}
        </button>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-4 sm:py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 mb-4 sm:mb-8">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div className="flex flex-col sm:flex-row sm:items-center gap-2 sm:gap-4">
              <div className="flex items-center gap-2 text-sm sm:text-base">
                <MapPin className="h-5 w-5 text-sudan-green-600" />
                <span className="font-medium">{params.originName}</span>
                <span className="text-gray-400">→</span>
                <span className="font-medium">{params.destinationName}</span>
              </div>
              <div className="flex items-center gap-2 text-sm sm:text-base">
                <Clock className="h-5 w-5 text-gray-400" />
                <span className="text-gray-600">{params.date}</span>
              </div>
              <div className="flex items-center gap-2 text-sm sm:text-base">
                <Users className="h-5 w-5 text-gray-400" />
                <span className="text-gray-600">{params.passengers} {t('passengers')}</span>
              </div>
            </div>
            <button onClick={() => navigate('/')} className="bg-sudan-green-600 text-white px-4 py-2 rounded-lg hover:bg-sudan-green-700 text-sm sm:text-base">
              {t('modify.search')}
            </button>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          <div className="lg:col-span-1 order-2 lg:order-1">
            <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 lg:sticky lg:top-24">
              <h3 className="text-base sm:text-lg font-semibold mb-4 flex items-center">
                <Filter className="h-5 w-5 mr-2 rtl:mr-0 rtl:ml-2" />
                {t('filters')}
              </h3>
              <div className="mb-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">{t('price.range.sdg')}</label>
                <div className="grid grid-cols-2 gap-2">
                  <input
                    type="number"
                    placeholder={t('min')}
                    value={priceRange[0]}
                    onChange={(e) => setPriceRange([parseInt(e.target.value) || 0, priceRange[1]])}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 text-sm"
                  />
                  <input
                    type="number"
                    placeholder={t('max')}
                    value={priceRange[1]}
                    onChange={(e) => setPriceRange([priceRange[0], parseInt(e.target.value) || 100000])}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 text-sm"
                  />
                </div>
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <SortAsc className="inline h-4 w-4 mr-1 rtl:mr-0 rtl:ml-1" />
                  {t('sort.by')}
                </label>
                <select
                  value={sortBy}
                  onChange={(e) => setSortBy(e.target.value as 'time' | 'price' | 'duration')}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 text-sm sm:text-base"
                >
                  <option value="time">{t('departure.time')}</option>
                  <option value="price">{t('price')}</option>
                  <option value="duration">{t('duration')}</option>
                </select>
              </div>
            </div>
          </div>

          <div className="lg:col-span-3 order-1 lg:order-2">
            <div className="mb-4 sm:mb-6">
              <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-2">{t('available.trains')}</h2>
              {!loading && <p className="text-sm sm:text-base text-gray-600">{visibleTrips.length} {t('trains.found')}</p>}
            </div>

            {loading ? (
              <div className="text-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-sudan-green-600 mx-auto mb-4"></div>
                <p className="text-gray-600">{t('loading')}</p>
              </div>
            ) : error ? (
              <div className="bg-red-50 text-red-600 rounded-lg p-4">{error}</div>
            ) : visibleTrips.length === 0 ? (
              <div className="text-center py-12">
                <TrainIcon className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <p className="text-gray-500">{t('no.trains')}</p>
              </div>
            ) : (
              <div className="space-y-3 sm:space-y-4">
                {visibleTrips.map((trip) => (
                  <div key={trip.id} className="bg-white rounded-lg shadow-md p-4 sm:p-6 hover:shadow-lg transition-shadow">
                    <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
                      <div className="flex-1">
                        <div className="flex items-center gap-3 mb-3">
                          <h3 className="text-base sm:text-lg font-semibold text-gray-900">{trip.trainName}</h3>
                          <span className="text-xs text-gray-500">#{trip.trainNumber}</span>
                        </div>
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-3">
                          <div>
                            <p className="text-sm text-gray-600">{t('departure')}</p>
                            <p className="text-xs text-sudan-green-700 font-medium truncate" title={params.originName}>
                              {params.originName}
                            </p>
                            <p className="text-sm text-gray-500">{formatDateSafe(trip.departureISO)}</p>
                            <p className="font-semibold text-lg">{formatTimeSafe(trip.departureISO)}</p>
                          </div>
                          <div>
                            <p className="text-sm text-gray-600">{t('arrival')}</p>
                            <p className="text-xs text-sudan-green-700 font-medium truncate" title={params.destinationName}>
                              {params.destinationName}
                            </p>
                            <p className="text-sm text-gray-500">{formatDateSafe(trip.arrivalISO)}</p>
                            <p className="font-semibold text-lg">{formatTimeSafe(trip.arrivalISO)}</p>
                          </div>
                          <div>
                            <p className="text-sm text-gray-600">{t('duration')}</p>
                            <p className="font-medium mt-6 sm:mt-8">{formatDurationSafe(trip.departureISO, trip.arrivalISO)}</p>
                          </div>
                        </div>
                        <p className="text-xs sm:text-sm text-gray-600">{trip.availableSeats} {t('seats.available')}</p>
                      </div>

                      <div className="flex flex-row sm:flex-col items-center sm:items-end justify-between gap-4 sm:gap-2">
                        <div className="sm:text-right">
                          <p className="text-xl sm:text-2xl font-bold text-gray-900">
                            {trip.price > 0 ? `${Math.round(trip.price)} ${trip.currency === 'SDG' ? t('sdg') : trip.currency}` : '—'}
                          </p>
                          <p className="text-sm text-gray-600">{t('per.person')}</p>
                          {trip.fareClass && trip.price > 0 && (
                            <p className="text-[11px] text-gray-500 mt-0.5">
                              {t('starting.from')} {t((trip.fareClass + '.class').toLowerCase()) || trip.fareClass}
                            </p>
                          )}
                          {(trip.fareScope === 'trip' || trip.fareScope === 'segment') && (
                            <span className="inline-block mt-1 px-2 py-0.5 rounded-full bg-sudan-gold-100 text-sudan-gold-800 text-[10px]">
                              {trip.fareScope === 'trip' ? t('fare.scope.trip') : t('fare.scope.segment')}
                            </span>
                          )}
                        </div>
                        <button
                          onClick={() => handleBooking(trip)}
                          disabled={trip.availableSeats <= 0}
                          className="bg-sudan-green-600 text-white px-4 sm:px-6 py-2 rounded-lg font-medium hover:bg-sudan-green-700 transition-colors text-sm sm:text-base whitespace-nowrap disabled:opacity-50"
                        >
                          {t('book.now')}
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
