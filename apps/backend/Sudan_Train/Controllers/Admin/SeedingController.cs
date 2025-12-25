using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Services.Seeding;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Admin
{
    /// <summary>
    /// Admin-only controller for seeding geography data from Google API
    /// </summary>
    [ApiController]
    [Route(Router.Admin + "/Seeding")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public class SeedingController : ControllerBase
    {
        private readonly IGeographySeeder _geographySeeder;

        public SeedingController(IGeographySeeder geographySeeder)
        {
            _geographySeeder = geographySeeder;
        }

        /// <summary>
        /// Seed all geography data (regions, states, cities) from Google API
        /// </summary>
        /// <returns>Complete seeding result with counts for each entity type</returns>
        [HttpPost("Geography")]
        public async Task<IActionResult> SeedGeography()
        {
            var result = await _geographySeeder.SeedAllGeography();
            return Ok(new
            {
                success = true,
                message = "Geography seeding completed",
                data = result
            });
        }

        /// <summary>
        /// Seed only regions from Google API
        /// </summary>
        [HttpPost("Regions")]
        public async Task<IActionResult> SeedRegions()
        {
            var result = await _geographySeeder.SeedRegionsFromGoogle();
            return Ok(new
            {
                success = true,
                message = "Region seeding completed",
                data = result
            });
        }

        /// <summary>
        /// Seed only states from Google API
        /// </summary>
        [HttpPost("States")]
        public async Task<IActionResult> SeedStates()
        {
            var result = await _geographySeeder.SeedStatesFromGoogle();
            return Ok(new
            {
                success = true,
                message = "State seeding completed",
                data = result
            });
        }

        /// <summary>
        /// Seed only cities from Google API
        /// </summary>
        [HttpPost("Cities")]
        public async Task<IActionResult> SeedCities()
        {
            var result = await _geographySeeder.SeedCitiesFromGoogle();
            return Ok(new
            {
                success = true,
                message = "City seeding completed",
                data = result
            });
        }
    }
}
