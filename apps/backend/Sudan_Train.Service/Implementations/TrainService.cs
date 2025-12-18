using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class TrainService : ITrainService
    {
        private readonly ITrainRepository _trainRepository;
        private readonly ICoachRepository _coachRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly ITripRepository _tripRepository;

        public TrainService(
            ITrainRepository trainRepository,
            ICoachRepository coachRepository,
            ISeatRepository seatRepository,
            ITripRepository tripRepository)
        {
            _trainRepository = trainRepository;
            _coachRepository = coachRepository;
            _seatRepository = seatRepository;
            _tripRepository = tripRepository;
        }

        public async Task<TrainDto> CreateTrainAsync(string trainNumber, string nameEn, string nameAr, CoachClass type)
        {
            var train = new Train
            {
                TrainNumber = trainNumber,
                NameEn = nameEn,
                NameAr = nameAr,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            await _trainRepository.AddAsync(train);

            return new TrainDto
            {
                Id = train.Id,
                TrainNumber = train.TrainNumber,
                NameEn = train.NameEn ?? "",
                NameAr = train.NameAr ?? "",
                Type = train.Type.ToString(),
                CoachesCount = 0,
                TotalCapacity = 0,
                CreatedAt = train.CreatedAt
            };
        }

        public async Task<TrainDto?> GetTrainByIdAsync(int id)
        {
            var train = await _trainRepository.GetTableNoTracking()
                .Include(t => t.Coaches)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (train == null)
                return null;

            return new TrainDto
            {
                Id = train.Id,
                TrainNumber = train.TrainNumber,
                NameEn = train.NameEn ?? "",
                NameAr = train.NameAr ?? "",
                Type = train.Type.ToString(),
                CoachesCount = train.Coaches.Count,
                TotalCapacity = train.Coaches.Sum(c => c.Capacity),
                CreatedAt = train.CreatedAt
            };
        }

        public async Task<List<TrainDto>> GetAllTrainsAsync(string? searchTerm = null)
        {
            var query = _trainRepository.GetTableNoTracking()
                .Include(t => t.Coaches)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(t =>
                    t.TrainNumber.ToLower().Contains(lowerSearch) ||
                    (t.NameEn != null && t.NameEn.ToLower().Contains(lowerSearch)) ||
                    (t.NameAr != null && t.NameAr.Contains(searchTerm)));
            }

            var trains = await query
                .OrderBy(t => t.TrainNumber)
                .Select(t => new TrainDto
                {
                    Id = t.Id,
                    TrainNumber = t.TrainNumber,
                    NameEn = t.NameEn ?? "",
                    NameAr = t.NameAr ?? "",
                    Type = t.Type.ToString(),
                    CoachesCount = t.Coaches.Count,
                    TotalCapacity = t.Coaches.Sum(c => c.Capacity),
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return trains;
        }

        public async Task<TrainDto> UpdateTrainAsync(int id, string trainNumber, string nameEn, string nameAr, CoachClass type)
        {
            var train = await _trainRepository.GetTableNoTracking()
                .Include(t => t.Coaches)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (train == null)
                throw new KeyNotFoundException($"Train with ID {id} not found");

            train.TrainNumber = trainNumber;
            train.NameEn = nameEn;
            train.NameAr = nameAr;
            train.Type = type;
            train.UpdatedAt = DateTime.UtcNow;

            await _trainRepository.UpdateAsync(train);

            return new TrainDto
            {
                Id = train.Id,
                TrainNumber = train.TrainNumber,
                NameEn = train.NameEn ?? "",
                NameAr = train.NameAr ?? "",
                Type = train.Type.ToString(),
                CoachesCount = train.Coaches.Count,
                TotalCapacity = train.Coaches.Sum(c => c.Capacity),
                CreatedAt = train.CreatedAt
            };
        }

        public async Task<bool> DeleteTrainAsync(int id)
        {
            var train = await _trainRepository.GetByIdAsync(id);
            if (train == null)
                return false;

            await _trainRepository.DeleteAsync(train);
            return true;
        }

        public async Task<List<CoachDto>> BulkCreateCoachesAsync(int trainId, int numberOfCoaches, CoachClass coachClass, int capacityPerCoach, bool autoGenerateSeats)
        {
            var coaches = new List<Coach>();
            var maxSequence = await _coachRepository.GetTableNoTracking()
                .Where(c => c.TrainId == trainId)
                .OrderByDescending(c => c.Sequence)
                .Select(c => c.Sequence)
                .FirstOrDefaultAsync();

            int startSequence = maxSequence + 1;

            for (int i = 0; i < numberOfCoaches; i++)
            {
                var coach = new Coach
                {
                    TrainId = trainId,
                    CoachNumber = $"C{startSequence + i}",
                    Class = coachClass,
                    Capacity = capacityPerCoach,
                    Sequence = startSequence + i,
                    CreatedAt = DateTime.UtcNow
                };
                coaches.Add(coach);
            }

            await _coachRepository.AddRangeAsync(coaches);

            // Auto-generate seats if requested
            if (autoGenerateSeats)
            {
                foreach (var coach in coaches)
                {
                    await GenerateSeatsForCoachAsync(coach.Id, capacityPerCoach);
                }
            }

            return coaches.Select(c => new CoachDto
            {
                Id = c.Id,
                TrainId = c.TrainId,
                CoachNumber = c.CoachNumber,
                Class = c.Class.ToString(),
                Capacity = c.Capacity,
                Sequence = c.Sequence,
                SeatsCount = autoGenerateSeats ? capacityPerCoach : 0
            }).ToList();
        }

        public async Task<List<CoachDto>> GetCoachesByTrainAsync(int trainId)
        {
            var coaches = await _coachRepository.GetTableNoTracking()
                .Include(c => c.Seats)
                .Where(c => c.TrainId == trainId)
                .OrderBy(c => c.Sequence)
                .Select(c => new CoachDto
                {
                    Id = c.Id,
                    TrainId = c.TrainId,
                    CoachNumber = c.CoachNumber,
                    Class = c.Class.ToString(),
                    Capacity = c.Capacity,
                    Sequence = c.Sequence,
                    SeatsCount = c.Seats.Count
                })
                .ToListAsync();

            return coaches;
        }

        public async Task<List<SeatDto>> GetSeatsByCoachAsync(int coachId)
        {
            var seats = await _seatRepository.GetTableNoTracking()
                .Where(s => s.CoachId == coachId)
                .OrderBy(s => s.SeatNumber)
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    CoachId = s.CoachId,
                    SeatNumber = s.SeatNumber,
                    IsWindow = s.IsWindow,
                    IsAccessible = s.IsAccessible
                })
                .ToListAsync();

            return seats;
        }

        public async Task<bool> IsTrainNumberUniqueAsync(string trainNumber, int? excludeId = null)
        {
            var query = _trainRepository.GetTableNoTracking()
                .Where(t => t.TrainNumber == trainNumber);

            if (excludeId.HasValue)
                query = query.Where(t => t.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> TrainHasActiveTripsAsync(int trainId)
        {
            var now = DateTime.UtcNow;
            return await _tripRepository.GetTableNoTracking()
                .AnyAsync(t => t.TrainId == trainId && t.DepartureTime > now && t.Status != "Cancelled");
        }

        private async Task GenerateSeatsForCoachAsync(int coachId, int capacity)
        {
            var seats = new List<Seat>();
            for (int seatNum = 1; seatNum <= capacity; seatNum++)
            {
                seats.Add(new Seat
                {
                    CoachId = coachId,
                    SeatNumber = seatNum.ToString(),
                    IsWindow = (seatNum % 4 == 1 || seatNum % 4 == 2), // 50% window seats
                    IsAccessible = seatNum == 1 // First seat is accessible
                });
            }

            await _seatRepository.AddRangeAsync(seats);
        }
    }
}

