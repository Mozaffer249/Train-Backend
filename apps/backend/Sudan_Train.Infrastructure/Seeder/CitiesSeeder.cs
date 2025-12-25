using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Infrastructure.Seeder
{
    public class CitiesSeeder
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<CitiesSeeder> _logger;

        public CitiesSeeder(ApplicationDBContext context, ILogger<CitiesSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Check if cities already exist
                if (await _context.Cities.AnyAsync())
                {
                    _logger.LogInformation("Cities already seeded. Skipping...");
                    return;
                }

                _logger.LogInformation("Seeding cities...");

                var cities = new List<City>
                {
                    // Major Sudanese Cities with approximate coordinates
                    new City
                    {
                        NameEn = "Khartoum",
                        NameAr = "الخرطوم",
                        Latitude = 15.5007,
                        Longitude = 32.5599
                    },
                    new City
                    {
                        NameEn = "Omdurman",
                        NameAr = "أم درمان",
                        Latitude = 15.6442,
                        Longitude = 32.4777
                    },
                    new City
                    {
                        NameEn = "Khartoum North",
                        NameAr = "الخرطوم بحري",
                        Latitude = 15.6393,
                        Longitude = 32.5363
                    },
                    new City
                    {
                        NameEn = "Port Sudan",
                        NameAr = "بورتسودان",
                        Latitude = 19.6158,
                        Longitude = 37.2164
                    },
                    new City
                    {
                        NameEn = "Kassala",
                        NameAr = "كسلا",
                        Latitude = 15.4609,
                        Longitude = 36.3990
                    },
                    new City
                    {
                        NameEn = "El Obeid",
                        NameAr = "الأبيض",
                        Latitude = 13.1833,
                        Longitude = 30.2167
                    },
                    new City
                    {
                        NameEn = "Nyala",
                        NameAr = "نيالا",
                        Latitude = 12.0488,
                        Longitude = 24.8810
                    },
                    new City
                    {
                        NameEn = "Wad Madani",
                        NameAr = "ود مدني",
                        Latitude = 14.4011,
                        Longitude = 33.5196
                    },
                    new City
                    {
                        NameEn = "El Fasher",
                        NameAr = "الفاشر",
                        Latitude = 13.6286,
                        Longitude = 25.3533
                    },
                    new City
                    {
                        NameEn = "Atbara",
                        NameAr = "عطبرة",
                        Latitude = 17.7027,
                        Longitude = 33.9868
                    }
                };

                await _context.Cities.AddRangeAsync(cities);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully seeded {cities.Count} cities.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding cities.");
                throw;
            }
        }
    }
}
