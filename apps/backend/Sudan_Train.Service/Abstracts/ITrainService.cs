using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Service.Abstracts
{
    public interface ITrainService
    {
        Task<TrainDto> CreateTrainAsync(string trainNumber, string nameEn, string nameAr);
        Task<TrainDto?> GetTrainByIdAsync(int id);
        Task<List<TrainDto>> GetAllTrainsAsync(string? searchTerm = null);
        Task<TrainDto> UpdateTrainAsync(int id, string trainNumber, string nameEn, string nameAr);
        Task<bool> DeleteTrainAsync(int id);
        Task<List<CoachDto>> BulkCreateCoachesAsync(int trainId, int numberOfCoaches, CoachClass coachClass, int capacityPerCoach, bool autoGenerateSeats);
        Task<List<CoachDto>> GetCoachesByTrainAsync(int trainId);
        Task<CoachDto?> GetCoachByIdAsync(int coachId);
        Task<CoachDto?> UpdateCoachAsync(int coachId, string? coachNumber, CoachClass? coachClass, int? sequence);
        Task<List<SeatDto>> GetSeatsByCoachAsync(int coachId);
        Task<bool> IsTrainNumberUniqueAsync(string trainNumber, int? excludeId = null);
        Task<bool> TrainHasActiveTripsAsync(int trainId);
    }
}

