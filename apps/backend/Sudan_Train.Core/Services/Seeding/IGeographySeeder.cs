using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sudan_Train.Core.Services.Seeding
{
    public interface IGeographySeeder
    {
        /// <summary>
        /// Seed Sudan regions from Google API
        /// </summary>
        Task<SeedingResult> SeedRegionsFromGoogle();

        /// <summary>
        /// Seed states from Google API for existing regions
        /// </summary>
        Task<SeedingResult> SeedStatesFromGoogle();

        /// <summary>
        /// Seed cities from Google API for existing states
        /// </summary>
        Task<SeedingResult> SeedCitiesFromGoogle();

        /// <summary>
        /// Seed all geography data (regions, states, cities) from Google
        /// </summary>
        Task<CompleteSeedingResult> SeedAllGeography();
    }

    public class SeedingResult
    {
        public int Added { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }

    public class CompleteSeedingResult
    {
        public SeedingResult Regions { get; set; } = new SeedingResult();
        public SeedingResult States { get; set; } = new SeedingResult();
        public SeedingResult Cities { get; set; } = new SeedingResult();
        public int TotalAdded => Regions.Added + States.Added + Cities.Added;
        public int TotalSkipped => Regions.Skipped + States.Skipped + Cities.Skipped;
        public int TotalFailed => Regions.Failed + States.Failed + Cities.Failed;
    }
}
