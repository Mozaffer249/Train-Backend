using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Infrastructure.Seeder
{
    public class InfrastructureSeeder
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<InfrastructureSeeder> _logger;

        public InfrastructureSeeder(ApplicationDBContext context, ILogger<InfrastructureSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Check if already seeded
                if (await _context.Cities.AnyAsync())
                {
                    _logger.LogInformation("Infrastructure data already seeded");
                    return;
                }

                _logger.LogInformation("Starting infrastructure seeding...");

                var cities = await SeedCitiesAsync();
                var stations = await SeedStationsAsync(cities);
                var routes = await SeedRoutesAsync(stations);
                var trains = await SeedTrainsAsync();
                await SeedTripsAsync(trains, routes);

                _logger.LogInformation("Infrastructure seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding infrastructure data");
                throw;
            }
        }

        private async Task<List<City>> SeedCitiesAsync()
        {
            var cities = new List<City>
            {
                new City { NameEn = "Khartoum", NameAr = "الخرطوم", Latitude = 15.5007, Longitude = 32.5599 },
                new City { NameEn = "Omdurman", NameAr = "أم درمان", Latitude = 15.6442, Longitude = 32.4777 },
                new City { NameEn = "Khartoum North", NameAr = "الخرطوم بحري", Latitude = 15.6393, Longitude = 32.5363 },
                new City { NameEn = "Atbara", NameAr = "عطبرة", Latitude = 17.7027, Longitude = 33.9868 },
                new City { NameEn = "Port Sudan", NameAr = "بورتسودان", Latitude = 19.6158, Longitude = 37.2164 },
                new City { NameEn = "Kassala", NameAr = "كسلا", Latitude = 15.4609, Longitude = 36.3990 },
                new City { NameEn = "Gedaref", NameAr = "القضارف", Latitude = 14.0352, Longitude = 35.3833 },
                new City { NameEn = "El Obeid", NameAr = "الأبيض", Latitude = 13.1833, Longitude = 30.2167 },
                new City { NameEn = "Nyala", NameAr = "نيالا", Latitude = 12.0488, Longitude = 24.8810 },
                new City { NameEn = "Wad Madani", NameAr = "ود مدني", Latitude = 14.4011, Longitude = 33.5196 }
            };

            await _context.Cities.AddRangeAsync(cities);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} cities", cities.Count);
            return cities;
        }

        private async Task<List<Station>> SeedStationsAsync(List<City> cities)
        {
            var stations = new List<Station>
            {
                new Station { Code = "KHR", NameEn = "Khartoum Central Station", NameAr = "محطة الخرطوم المركزية", CityId = cities[0].Id, Latitude = 15.5007, Longitude = 32.5599, AddressEn = "Downtown Khartoum", AddressAr = "وسط الخرطوم", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Station { Code = "OMD", NameEn = "Omdurman Station", NameAr = "محطة أم درمان", CityId = cities[1].Id, Latitude = 15.6448, Longitude = 32.4777, AddressEn = "Omdurman Center", AddressAr = "مركز أم درمان", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Station { Code = "BHR", NameEn = "Bahri Station", NameAr = "محطة بحري", CityId = cities[2].Id, Latitude = 15.5888, Longitude = 32.5342, AddressEn = "Bahri District", AddressAr = "حي بحري", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Station { Code = "ATB", NameEn = "Atbara Station", NameAr = "محطة عطبرة", CityId = cities[3].Id, Latitude = 17.7028, Longitude = 33.9789, AddressEn = "Atbara Center", AddressAr = "مركز عطبرة", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Station { Code = "PSD", NameEn = "Port Sudan Station", NameAr = "محطة بورتسودان", CityId = cities[4].Id, Latitude = 19.6158, Longitude = 37.2163, AddressEn = "Port Sudan Terminal", AddressAr = "محطة بورتسودان", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Station { Code = "KSL", NameEn = "Kassala Station", NameAr = "محطة كسلا", CityId = cities[5].Id, Latitude = 15.4509, Longitude = 36.4000, AddressEn = "Kassala City", AddressAr = "مدينة كسلا", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Station { Code = "GDF", NameEn = "Gedaref Station", NameAr = "محطة القضارف", CityId = cities[6].Id, Latitude = 14.0355, Longitude = 35.3836, AddressEn = "Gedaref Center", AddressAr = "مركز القضارف", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            await _context.Stations.AddRangeAsync(stations);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} stations", stations.Count);
            return stations;
        }

        private async Task<List<Route>> SeedRoutesAsync(List<Station> stations)
        {
            var routes = new List<Route>
            {
                new Route { NameEn = "Khartoum to Atbara Route", NameAr = "خط الخرطوم إلى عطبرة", OriginStationId = stations[0].Id, DestinationStationId = stations[3].Id, DistanceKm = 350, CreatedAt = DateTime.UtcNow },
                new Route { NameEn = "Atbara to Port Sudan Route", NameAr = "خط عطبرة إلى بورتسودان", OriginStationId = stations[3].Id, DestinationStationId = stations[4].Id, DistanceKm = 450, CreatedAt = DateTime.UtcNow },
                new Route { NameEn = "Khartoum to Kassala Route", NameAr = "خط الخرطوم إلى كسلا", OriginStationId = stations[0].Id, DestinationStationId = stations[5].Id, DistanceKm = 480, CreatedAt = DateTime.UtcNow },
                new Route { NameEn = "Khartoum to Gedaref Route", NameAr = "خط الخرطوم إلى القضارف", OriginStationId = stations[0].Id, DestinationStationId = stations[6].Id, DistanceKm = 410, CreatedAt = DateTime.UtcNow },
                new Route { NameEn = "Kassala to Port Sudan Route", NameAr = "خط كسلا إلى بورتسودان", OriginStationId = stations[5].Id, DestinationStationId = stations[4].Id, DistanceKm = 520, CreatedAt = DateTime.UtcNow }
            };

            await _context.Routes.AddRangeAsync(routes);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} routes", routes.Count);
            return routes;
        }

        private async Task<List<Train>> SeedTrainsAsync()
        {
            var trains = new List<Train>
            {
                new Train { TrainNumber = "TR-101", NameEn = "Express One", NameAr = "إكسبرس واحد", Type = CoachClass.First, CreatedAt = DateTime.UtcNow },
                new Train { TrainNumber = "TR-102", NameEn = "Regional Two", NameAr = "إقليمي اثنان", Type = CoachClass.Second, CreatedAt = DateTime.UtcNow },
                new Train { TrainNumber = "TR-103", NameEn = "Local Three", NameAr = "محلي ثلاثة", Type = CoachClass.Third, CreatedAt = DateTime.UtcNow }
            };

            await _context.Trains.AddRangeAsync(trains);
            await _context.SaveChangesAsync();

            foreach (var train in trains)
            {
                int coaches = train.TrainNumber == "TR-101" ? 5 : train.TrainNumber == "TR-102" ? 4 : 3;
                int seatsPerCoach = train.TrainNumber == "TR-101" ? 40 : train.TrainNumber == "TR-102" ? 50 : 60;
                await CreateCoachesAndSeats(train, coaches, seatsPerCoach);
            }

            _logger.LogInformation("Seeded {Count} trains with coaches and seats", trains.Count);
            return trains;
        }

        private async Task CreateCoachesAndSeats(Train train, int numberOfCoaches, int seatsPerCoach)
        {
            var coaches = new List<Coach>();
            for (int i = 1; i <= numberOfCoaches; i++)
            {
                coaches.Add(new Coach { TrainId = train.Id, CoachNumber = $"C{i}", Class = train.Type, Capacity = seatsPerCoach, Sequence = i, CreatedAt = DateTime.UtcNow });
            }

            await _context.Coaches.AddRangeAsync(coaches);
            await _context.SaveChangesAsync();

            var seats = new List<Seat>();
            foreach (var coach in coaches)
            {
                for (int seatNum = 1; seatNum <= seatsPerCoach; seatNum++)
                {
                    seats.Add(new Seat { CoachId = coach.Id, SeatNumber = seatNum.ToString(), IsWindow = (seatNum % 4 == 1 || seatNum % 4 == 2), IsAccessible = (seatNum == 1 && coach.Sequence == 1) });
                }
            }

            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        private async Task SeedTripsAsync(List<Train> trains, List<Route> routes)
        {
            var today = DateTime.UtcNow.Date;
            var trips = new List<Trip>();

            for (int day = 0; day < 7; day++)
            {
                var tripDate = today.AddDays(day);
                trips.Add(new Trip { TrainId = trains[0].Id, RouteId = routes[0].Id, DepartureTime = tripDate.AddHours(6), ArrivalTime = tripDate.AddHours(10).AddMinutes(30), Status = "Scheduled", CreatedAt = DateTime.UtcNow });
                trips.Add(new Trip { TrainId = trains[1].Id, RouteId = routes[1].Id, DepartureTime = tripDate.AddHours(8), ArrivalTime = tripDate.AddHours(14), Status = "Scheduled", CreatedAt = DateTime.UtcNow });
                trips.Add(new Trip { TrainId = trains[2].Id, RouteId = routes[2].Id, DepartureTime = tripDate.AddHours(15), ArrivalTime = tripDate.AddHours(21), Status = "Scheduled", CreatedAt = DateTime.UtcNow });
            }

            await _context.Trip.AddRangeAsync(trips);
            await _context.SaveChangesAsync();

            foreach (var trip in trips)
            {
                var seats = await _context.Coaches.Where(c => c.TrainId == trip.TrainId).SelectMany(c => c.Seats).ToListAsync();
                var tripSeats = seats.Select(s => new TripSeat { TripId = trip.Id, SeatId = s.Id, Status = SeatStatus.Available }).ToList();
                await _context.TripSeats.AddRangeAsync(tripSeats);
            }
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} trips with seat availability", trips.Count);
        }
    }
}

