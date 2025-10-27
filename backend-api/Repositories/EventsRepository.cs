using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IEventsRepository
    {
        Task<List<EventListResponseDTO>?> getAllEvents();
        Task<List<EventListResponseDTO>?> getEventsById(EventListByIdRequestDTO t);
        Task<(bool Success, string? Message)> createEvent(EventCreateRequestDTO t);
    }

    public class EventsRepository : IEventsRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;

        public EventsRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<List<EventListResponseDTO>?> getAllEvents()
        {
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var result = await dbContext.Events
                    .AsNoTracking()
                    .Select(x => new EventListResponseDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        TopicName = x.Topics.Type,
                        DateCreate = x.DateCreate,
                        Status = x.Status

                    }).ToListAsync();

                if (result == null || result.Count == 0) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EventListResponseDTO>?> getEventsById(EventListByIdRequestDTO t)
        {
            if (t.CreateById <= 0) return null;

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var result = await dbContext.Events
                    .AsNoTracking()
                    .Where(x => x.CreateById == t.CreateById)
                    .Select(x => new EventListResponseDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        TopicName = x.Topics.Type,
                        DateCreate = x.DateCreate,
                        Status = x.Status
                    }).ToListAsync();

                if (result == null || result.Count == 0) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> createEvent(EventCreateRequestDTO t)
        {
            //Input validations
            if (string.IsNullOrWhiteSpace(t.Name)) return (false, "Name field empty.");
            if (string.IsNullOrWhiteSpace(t.Description)) return (false, "Description field empty.");
            if (t.TopicsId <= 0) return (false, "TopicId field empty."); ;
            if (t.CreateById <= 0) return (false, "CreateById field empty."); ;
            if (string.IsNullOrWhiteSpace(t.DateCreate)) return (false, "DateCreate field empty.");

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                // Verify if the topicId exist in table Topics
                var topic = await dbContext.Topics
                .AsNoTracking()
                .AnyAsync(x => x.Id == t.TopicsId);

                if (!topic)
                    return (false, "Topic not found."); ;

                var eventCreate = new Events
                {
                    Name = t.Name.Trim(),
                    Description = t.Description.Trim(),
                    TopicsId = t.TopicsId,
                    CreateById = t.CreateById,
                    DateCreate = t.DateCreate,
                    Status = t.Status,
                };

                //Create the event in database
                dbContext.Events.Add(eventCreate);
                await dbContext.SaveChangesAsync();

                return (true, "Event created successfully."); ;
            }
            catch
            {
                return (false, "Event created unsuccessfully."); ;
            }
        }
    }
}