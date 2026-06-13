import { useEffect, useState } from 'react';
import { boardingApi, tripsApi } from '../services/api';
import { Trip, TripManifest } from '../types/infrastructure';
import { AR } from '../i18n/ar';
import { showConfirm, showError, showSuccess, extractErrorMessage } from '../utils/alerts';

function fmtDeparture(iso: string): string {
  const d = new Date(iso);
  if (isNaN(d.getTime())) return iso;
  return d.toLocaleString('ar-EG', { dateStyle: 'medium', timeStyle: 'short' });
}

// Staff-facing boarding page. Two input paths into the same backend
// endpoint: a scanner modal (manual paste — works with any QR-reader app on
// the phone) and per-row "Board" buttons on the manifest table.
const BoardingPage = () => {
  const [trips, setTrips] = useState<Trip[]>([]);
  const [tripId, setTripId] = useState<number | ''>('');
  const [manifest, setManifest] = useState<TripManifest | null>(null);
  const [loadingTrips, setLoadingTrips] = useState(false);
  const [loadingManifest, setLoadingManifest] = useState(false);
  const [scanOpen, setScanOpen] = useState(false);
  const [scanPayload, setScanPayload] = useState('');

  useEffect(() => {
    const today = new Date().toISOString().slice(0, 10);
    setLoadingTrips(true);
    tripsApi.getAll({ date: today })
      .then(setTrips)
      .catch((err) => showError(AR.common.errorTitle, extractErrorMessage(err)))
      .finally(() => setLoadingTrips(false));
  }, []);

  const loadManifest = async (id: number) => {
    setLoadingManifest(true);
    try {
      const data = await boardingApi.getManifest(id);
      setManifest(data);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
      setManifest(null);
    } finally {
      setLoadingManifest(false);
    }
  };

  useEffect(() => {
    if (tripId === '') { setManifest(null); return; }
    loadManifest(tripId);
  }, [tripId]);

  const onBoard = async (ticketId: number) => {
    try {
      await boardingApi.boardTicket(ticketId);
      if (tripId !== '') loadManifest(tripId);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  const onNoShow = async (ticketId: number) => {
    const ok = await showConfirm(AR.boarding.markNoShow, '');
    if (!ok) return;
    try {
      await boardingApi.markNoShow(ticketId);
      if (tripId !== '') loadManifest(tripId);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  const onScanSubmit = async () => {
    if (!scanPayload.trim()) return;
    try {
      const result = await boardingApi.scan(scanPayload.trim());
      await showSuccess(`${AR.boarding.boarded} — ${result.ticketNumber || result.ticketId}`);
      setScanPayload('');
      setScanOpen(false);
      if (tripId !== '') loadManifest(tripId);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  const onDepart = async () => {
    if (tripId === '') return;
    const ok = await showConfirm(AR.boarding.confirmDepart, '');
    if (!ok) return;
    try {
      await boardingApi.departTrip(tripId);
      loadManifest(tripId);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  const onArrive = async () => {
    if (tripId === '') return;
    const ok = await showConfirm(AR.boarding.confirmArrive, '');
    if (!ok) return;
    try {
      await boardingApi.arriveTrip(tripId);
      loadManifest(tripId);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{AR.boarding.title}</h1>
        <p className="text-gray-600">{AR.boarding.subtitle}</p>
      </div>

      <div className="admin-card p-4 mb-4 flex flex-wrap items-center gap-3">
        <label className="text-sm text-gray-700">{AR.boarding.pickTrip}</label>
        <select
          value={tripId}
          onChange={(e) => setTripId(e.target.value ? Number(e.target.value) : '')}
          className="border rounded-md px-3 py-1.5 text-sm min-w-[280px]"
        >
          <option value="">{AR.common.selectPlaceholder}</option>
          {trips.map((t) => (
            <option key={t.id} value={t.id}>
              #{t.id} — {t.routeName || ''} — {fmtDeparture(t.departureTime)}
            </option>
          ))}
        </select>
        {loadingTrips && <span className="text-sm text-gray-500">{AR.common.loading}</span>}

        <div className="ms-auto flex gap-2">
          <button className="admin-button" onClick={() => setScanOpen(true)}>
            {AR.boarding.scanQr}
          </button>
          {manifest && manifest.status === 'Scheduled' && (
            <button className="admin-button-secondary" onClick={onDepart}>{AR.boarding.departTrip}</button>
          )}
          {manifest && manifest.status === 'Departed' && (
            <button className="admin-button-secondary" onClick={onArrive}>{AR.boarding.arriveTrip}</button>
          )}
        </div>
      </div>

      {scanOpen && (
        <div className="fixed inset-0 bg-black/40 z-50 flex items-center justify-center">
          <div className="bg-white rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-bold mb-3">{AR.boarding.scanQr}</h2>
            <p className="text-sm text-gray-600 mb-2">{AR.boarding.scanPaste}</p>
            <textarea
              value={scanPayload}
              onChange={(e) => setScanPayload(e.target.value)}
              rows={4}
              className="w-full border rounded-md p-2 text-sm font-mono"
              placeholder="{...} or TICKET-NUMBER"
            />
            <div className="flex justify-end gap-2 mt-4">
              <button className="admin-button-secondary" onClick={() => setScanOpen(false)}>{AR.common.cancel}</button>
              <button className="admin-button" onClick={onScanSubmit}>{AR.boarding.submit}</button>
            </div>
          </div>
        </div>
      )}

      {manifest && (
        <>
          <div className="admin-card p-4 mb-4 flex flex-wrap gap-6 text-sm">
            <span><strong>{manifest.routeNameAr || manifest.routeNameEn}</strong></span>
            <span>{AR.boarding.from}: {manifest.originStationAr || manifest.originStationEn}</span>
            <span>{AR.boarding.to}: {manifest.destinationStationAr || manifest.destinationStationEn}</span>
            <span>{AR.status[manifest.status] || manifest.status}</span>
            <span className="ms-auto">
              {AR.boarding.totalBoarded}: <strong>{manifest.boardedCount}</strong> / {manifest.totalTickets}
            </span>
          </div>

          <div className="admin-card overflow-hidden">
            <table className="min-w-full text-sm">
              <thead className="bg-gray-50 text-gray-700">
                <tr>
                  <th className="px-3 py-2 text-start">{AR.boarding.coach}</th>
                  <th className="px-3 py-2 text-start">{AR.boarding.seat}</th>
                  <th className="px-3 py-2 text-start">{AR.boarding.passenger}</th>
                  <th className="px-3 py-2 text-start">{AR.boarding.idNumber}</th>
                  <th className="px-3 py-2 text-start">{AR.boarding.from}</th>
                  <th className="px-3 py-2 text-start">{AR.boarding.to}</th>
                  <th className="px-3 py-2 text-start">{AR.common.actions}</th>
                </tr>
              </thead>
              <tbody>
                {loadingManifest && (
                  <tr><td colSpan={7} className="px-3 py-6 text-center text-gray-500">{AR.common.loading}</td></tr>
                )}
                {!loadingManifest && manifest.rows.length === 0 && (
                  <tr><td colSpan={7} className="px-3 py-6 text-center text-gray-500">{AR.common.none}</td></tr>
                )}
                {manifest.rows.map((r) => (
                  <tr key={r.ticketId} className="border-t">
                    <td className="px-3 py-2">{r.coachNumber || '—'}</td>
                    <td className="px-3 py-2 font-mono">{r.seatNumber || '—'}</td>
                    <td className="px-3 py-2">{r.passengerNameAr || r.passengerNameEn || '—'}</td>
                    <td className="px-3 py-2 font-mono text-xs">{r.idNumber}</td>
                    <td className="px-3 py-2">{r.boardingStationAr || r.boardingStationEn}</td>
                    <td className="px-3 py-2">{r.alightingStationAr || r.alightingStationEn}</td>
                    <td className="px-3 py-2">
                      <span className="me-2 px-2 py-0.5 rounded text-xs bg-gray-100">{AR.status[r.status] || r.status}</span>
                      {r.status === 'Issued' && (
                        <>
                          <button className="admin-button text-xs px-2 py-1 me-1" onClick={() => onBoard(r.ticketId)}>{AR.boarding.board}</button>
                          <button className="admin-button-secondary text-xs px-2 py-1" onClick={() => onNoShow(r.ticketId)}>{AR.boarding.noShow}</button>
                        </>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
};

export default BoardingPage;
