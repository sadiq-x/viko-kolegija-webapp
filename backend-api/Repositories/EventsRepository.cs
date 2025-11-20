using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;


namespace backend_api.Repositories
{
    public interface IEventsRepository
    {
        Task<List<EventListResponseDTO>?> getEvents(); //Get the list of all events 
        Task<List<EventListResponseDTO>?> getEventsByCreateById(EventByCreateByIdRequestDTO t); //Get the list of all events with a specific CreateByID
        Task<List<EventListResponseDTO>?> getEventsByTopics(EventListByTopicsRequestDTO t); //Get the list of all events with a specific Topic
        Task<List<EventListStudentResponseDTO>?> getEventsStudentByEntityId(EventStudentByEntityIdRequestDTO t); //Get the list of all events with a specific EntityId
        Task<EventListResponseDTO?> getEventsByEventId(EventByEventIdRequestDTO t); //Get the list of all events with a specific EventId
        Task<(bool Success, string? Message)> createEvent_Teacher(EventCreateTeacherRequestDTO t);
        Task<(bool Success, string? Message)> createEvent_Admin(EventCreateAdminRequestDTO t);
        Task<(bool Success, string? Message)> updateEventStatusToClose(EventChangeStatusRequestDTO t); //Update event with the status Close and set the DateClose
        Task<(bool Success, string? Message)> updateEventStatusToOngoing(EventChangeStatusRequestDTO t); //Update event with the status Ongoing
        Task<(bool Success, string? Message)> updateEvent_Admin(EventEditAdminRequestDTO t); 
    }

    public class EventsRepository : IEventsRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;

        public EventsRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<List<EventListResponseDTO>?> getEvents()
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
                        DateClose = x.DateClose!,
                        Status = x.StatusEvents.Type,
                        CreateBy = x.Entities.Name
                    }).ToListAsync();

                if (result == null || result.Count == 0) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EventListResponseDTO>?> getEventsByCreateById(EventByCreateByIdRequestDTO t)
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
                        DateClose = x.DateClose!,
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
                        DateClose = x.DateClose!,
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

        public async Task<List<EventListStudentResponseDTO>?> getEventsStudentByEntityId(EventStudentByEntityIdRequestDTO t)
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

        public async Task<EventListResponseDTO?> getEventsByEventId(EventByEventIdRequestDTO t)
        {
            if (t.Id <= 0) return null;
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var result = await dbContext.Events
                    .AsNoTracking()
                    .Where(x => x.Id == t.Id)

                    .Select(x => new EventListResponseDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        TopicName = x.Topics.Type,
                        DateCreate = x.DateCreate,
                        DateClose = x.DateClose!,
                        CreateBy = x.Entities.Name,
                        Status = x.StatusEvents.Type
                    }).FirstOrDefaultAsync();

                if (result is null) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> createEvent_Teacher(EventCreateTeacherRequestDTO t)
        {
            if (string.IsNullOrWhiteSpace(t.Name)) return (false, "Name field empty.");
            if (string.IsNullOrWhiteSpace(t.Description)) return (false, "Description field empty.");
            if (t.TopicsId <= 0) return (false, "TopicId field empty."); ;
            if (t.CreateById <= 0 || t.CreateById is null) return (false, "CreateById field empty."); ;
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var isTeacher = await dbContext.Entities
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.CreateById && e.Roles.Type == "Teacher");

                if (!isTeacher)
                    return (false, "Teacher is not Admin.");

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
                    CreateById = t.CreateById.Value,
                    DateCreate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    DateClose = null,
                    StatusId = 1
                };

                dbContext.Events.Add(eventCreate);
                await dbContext.SaveChangesAsync();

                return (true, "Event created successfully."); ;
            }
            catch
            {
                return (false, "Event created unsuccessfully."); ;
            }
        }

        public async Task<(bool Success, string? Message)> createEvent_Admin(EventCreateAdminRequestDTO t)
        {
            if (string.IsNullOrWhiteSpace(t.Name)) return (false, "Name field empty.");
            if (string.IsNullOrWhiteSpace(t.Description)) return (false, "Description field empty.");
            if (t.TopicsId <= 0) return (false, "TopicId field empty."); ;
            if (t.CreateById <= 0) return (false, "CreateById field empty.");
            if (t.AdminId <= 0 || t.AdminId is null) return (false, "AdminId field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var isAdmin = await dbContext.Entities
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.AdminId && e.Roles.Type == "Admin");

                if (!isAdmin)
                    return (false, "Admin is not Admin.");

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
                    DateClose = null,
                    StatusId = 1
                };

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
            if (t.Id <= 0) return (false, "Id field empty."); ;
            if (t.CreateById <= 0 || t.CreateById is null) return (false, "CreateById field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var result = await dbContext.Events
                    .SingleOrDefaultAsync(x => x.Id == t.Id && x.CreateById == t.CreateById && x.StatusId == 2);

                if (result is null)
                {
                    return (false, "Event not found."); ;
                }

                result.DateClose = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                result.StatusId = 3;

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

        public async Task<(bool Success, string? Message)> updateEvent_Admin(EventEditAdminRequestDTO t)
        {
            if (t.Id <= 0) return (false, "Id field empty.");
            if (string.IsNullOrWhiteSpace(t.Name)) return (false, "Name field empty.");
            if (string.IsNullOrWhiteSpace(t.Description)) return (false, "Description field empty.");
            if (string.IsNullOrWhiteSpace(t.Type)) return (false, "Type field empty.");
            if (t.AdminId is null || t.AdminId <= 0) return (false, "AdminId field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var isAdmin = await dbContext.Entities
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.AdminId && e.Roles.Type == "Admin");

                if (!isAdmin)
                    return (false, "User is not Admin.");

                var eventEntity = await dbContext.Events
                    .FirstOrDefaultAsync(e => e.Id == t.Id);

                if (eventEntity is null)
                    return (false, "Event not found.");

                var topic = await dbContext.Topics
                    .AsNoTracking()
                    .FirstOrDefaultAsync(tp => tp.Type == t.Type);

                if (topic is null)
                    return (false, "Topic not found.");

                var teacherEntity = await dbContext.Entities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Name == t.CreateBy);

                if (teacherEntity is null)
                    return (false, "Teacher not found.");

                eventEntity.Name = t.Name;
                eventEntity.Description = t.Description;
                eventEntity.TopicsId = topic.Id;
                eventEntity.CreateById = teacherEntity.Id;
                eventEntity.DateCreate = t.DateCreate.ToString();

                await dbContext.SaveChangesAsync();

                return (true, "Event updated successfully.");
            }
            catch (Exception)
            {
                return (false, "Error updating event.");
            }
        }
    }
}