using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;


namespace backend_api.Repositories
{
    public interface IEventsRepository
    {
        Task<List<EventListResponseDTO>?> getAllEvents();
        Task<List<EventListResponseDTO>?> getEventsByCreateById(EventListByCreateByIdRequestDTO t);
        Task<(bool Success, string? Message)> createEvent(EventCreateRequestDTO t);
        Task<(bool Success, string? Message)> updateEventStatusToClose(EventChangeStatusRequestDTO t);
        Task<(bool Success, string? Message)> updateEventStatusToOngoing(EventChangeStatusRequestDTO t);
        Task<List<EventListResponseDTO>?> getEventsByTopics(EventListByTopicsRequestDTO t);
        Task<List<EventListStudentResponseDTO>?> getEventsByEntityId(EventListByEntityIdRequestDTO t);
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
                        Status = x.StatusEvents.Type //To change

                    }).ToListAsync();

                if (result == null || result.Count == 0) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EventListResponseDTO>?> getEventsByCreateById(EventListByCreateByIdRequestDTO t)
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
                        Status = x.StatusEvents.Type
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
                    DateCreate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    StatusId = 1 //To change
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

        public async Task<(bool Success, string? Message)> updateEventStatusToClose(EventChangeStatusRequestDTO t)
        {
            //Input validations
            if (t.Id <= 0) return (false, "Id field empty."); ;
            if (t.CreateById <= 0 || t.CreateById is null) return (false, "CreateById field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var eventClose = await dbContext.Events
                    .SingleOrDefaultAsync(x => x.Id == t.Id && x.CreateById == t.CreateById && x.StatusId == 2);

                if (eventClose is null)
                {
                    return (false, "Event not found."); ;
                }

                eventClose.StatusId = 3;

                await dbContext.SaveChangesAsync();

                return (true, "Event status close successfully."); ;
            }
            catch
            {
                return (false, "Event status close unsuccessfully."); ;
            }
        }

        public async Task<(bool Success, string? Message)> updateEventStatusToOngoing(EventChangeStatusRequestDTO t)
        {
            //Input validations
            if (t.Id <= 0) return (false, "Id field empty."); ;
            if (t.CreateById <= 0 || t.CreateById is null) return (false, "CreateById field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var eventOngoing = await dbContext.Events
                    .SingleOrDefaultAsync(x => x.Id == t.Id && x.CreateById == t.CreateById && x.StatusId == 1);

                if (eventOngoing is null)
                {
                    return (false, "Event not found."); ;
                }

                eventOngoing.StatusId = 2;

                await dbContext.SaveChangesAsync();

                return (true, "Event status ongoing successfully."); ;
            }
            catch
            {
                return (false, "Event status ongoing unsuccessfully."); ;
            }
        }

        public async Task<List<EventListResponseDTO>?> getEventsByTopics(EventListByTopicsRequestDTO t)
        {
            if (string.IsNullOrEmpty(t.Topic)) return null;
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var result = await dbContext.Events
                    .AsNoTracking()
                    .Where(x => x.Topics.Type == t.Topic)
                    .Select(x => new EventListResponseDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        TopicName = x.Topics.Type,
                        DateCreate = x.DateCreate,
                        Status = x.StatusEvents.Type
                    }).ToListAsync();

                if (result is null || result.Count == 0) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EventListStudentResponseDTO>?> getEventsByEntityId(EventListByEntityIdRequestDTO t)
        {
            if (t.EntityId <= 0) return null;
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var result = await dbContext.ParticipantsEvents
                    .AsNoTracking()
                    .Where(x => x.EntityId == t.EntityId)
                    .Select(x => new EventListStudentResponseDTO
                    {
                        Id = x.Id,
                        EventId = x.EventId,
                        Name = x.Event.Name,
                        Description = x.Event.Description,
                        TopicName = x.Event.Topics.Type,
                        DateCreate = x.Event.DateCreate,
                        DateClose = x.Event.DateClose!,
                        Status = x.Event.StatusEvents.Type,
                        Grade = x.Grade!,
                        ParticipantDescription = x.ParticipantDescription!
                    }).ToListAsync();

                if (result is null || result.Count == 0) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}