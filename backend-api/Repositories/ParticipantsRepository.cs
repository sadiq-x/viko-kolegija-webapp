using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IParticipantsEventsRepository
    {
        Task<List<ParticipantsListFromEventIdResponseDTO>?> getParticipantsFromEventId_User(ParticipantsListFromEventIdUserRequestDTO t);
        Task<List<ParticipantsListFromEventIdResponseDTO>?> getParticipantsFromEventId_Teacher(ParticipantsListFromEventIdTeacherRequestDTO t);
        Task<(bool Success, string? Message)> insertGradeParticipantsEvent(ParticipantsEventGradeRequestDTO t);
        Task<(bool Success, string? Message)> updateStatusParticipantsEvent(ParticipantsEventUpdateStatusRequestDTO t);
        Task<(bool Success, string? Message)> insertParticipantsEventInEvent(ParticipantsEventInsertInEventIdRequestDTO t);
        Task<(bool Success, string? Message)> insertParticipantsEventParticipantDescription(ParticipantsEventInsertParticipantDescriptionRequestDTO t);
        Task<(bool Success, string? Message)> cancelParticipantsEvent(ParticipantsEventCancelRequestDTO t);
    }

    public class ParticipantsEventsRepository : IParticipantsEventsRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;

        public ParticipantsEventsRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<List<ParticipantsListFromEventIdResponseDTO>?> getParticipantsFromEventId_User(ParticipantsListFromEventIdUserRequestDTO t)
        {
            if (t.EventId <= 0) return null;
            if (t.EntityId <= 0) return null;
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var eventExist = await dbContext.Events.AsNoTracking().AnyAsync(u => u.Id == t.EventId);

                if (!eventExist)
                {
                    return null;
                }

                var result = await dbContext.ParticipantsEvents
                    .AsNoTracking()
                    .Where(pe => pe.EventId == t.EventId && pe.EntityId == t.EntityId)
                    .Select(u => new ParticipantsListFromEventIdResponseDTO
                    {
                        Id = u.Id,
                        EntityId = u.EntityId,
                        Name = u.Entity.Name,
                        Email = u.Entity.Email,
                        Status = u.Status,
                        Grade = u.Grade,
                        Comments = u.Comments,
                        ParticipantDescription = u.ParticipantDescription
                    })
                    .ToListAsync();
                Console.WriteLine(eventExist.ToString());
                Console.WriteLine(result.ToString());
                if (result.Count == 0)
                    return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ParticipantsListFromEventIdResponseDTO>?> getParticipantsFromEventId_Teacher(ParticipantsListFromEventIdTeacherRequestDTO t)
        {
            if (t.EventId <= 0) return null;
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var eventExist = await dbContext.Events.AsNoTracking().AnyAsync(u => u.Id == t.EventId);

                if (!eventExist)
                {
                    return null;
                }

                var result = await dbContext.ParticipantsEvents
                    .AsNoTracking()
                    .Where(pe => pe.EventId == t.EventId)
                    .Select(u => new ParticipantsListFromEventIdResponseDTO
                    {
                        Id = u.Id,
                        EntityId = u.EntityId,
                        Name = u.Entity.Name,
                        Email = u.Entity.Email,
                        Status = u.Status,
                        Grade = u.Grade,
                        Comments = u.Comments,
                        ParticipantDescription = u.ParticipantDescription
                    })
                    .ToListAsync();


                if (result.Count == 0)
                    return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> insertGradeParticipantsEvent(ParticipantsEventGradeRequestDTO t)
        {
            if (t.Id <= 0) return (false, "Id field empty.");
            if (t.EventId <= 0) return (false, "EventId field empty.");
            if (string.IsNullOrEmpty(t.Grade)) return (false, "Grade field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var eventExist = await dbContext.Events.AsNoTracking().AnyAsync(u => u.Id == t.EventId);

                if (!eventExist)
                    return (false, "Event not found.");

                var participantEvent = await dbContext.ParticipantsEvents
                    .FirstOrDefaultAsync(pe => pe.Id == t.Id && pe.EventId == t.EventId);

                if (participantEvent is null)
                {
                    return (false, "Participant for this event not found.");
                }

                participantEvent.Grade = t.Grade;

                if (!string.IsNullOrWhiteSpace(t.Comments))
                {
                    participantEvent.Comments = t.Comments;
                }

                await dbContext.SaveChangesAsync();

                return (true, "Grade updated successfully.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> updateStatusParticipantsEvent(ParticipantsEventUpdateStatusRequestDTO t)
        {
            if (t.Id <= 0) return (false, "Id field empty.");
            if (t.EventId <= 0) return (false, "EventId field empty.");
            if (t.EntityId <= 0) return (false, "EntityId field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var participantEvent = await dbContext.ParticipantsEvents
                    .SingleOrDefaultAsync(pe =>
                        pe.Id == t.Id &&
                        pe.EventId == t.EventId &&
                        pe.EntityId == t.EntityId);

                if (participantEvent is null)
                {
                    return (false, "Participant for this event not found.");
                }

                participantEvent.Status = false;

                await dbContext.SaveChangesAsync();

                return (true, "Status updated successfully.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> insertParticipantsEventInEvent(ParticipantsEventInsertInEventIdRequestDTO t)
        {
            if (t.EventId <= 0) return (false, "EventId field empty.");
            if (t.EntityId <= 0 || t.EntityId is null) return (false, "EntityId field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var eventExists = await dbContext.Events
                .AsNoTracking()
                .AnyAsync(e => e.Id == t.EventId);

                if (!eventExists)
                {
                    return (false, "Event not found.");
                }

                var entityExists = await dbContext.Entities
                .AsNoTracking()
                .AnyAsync(e => e.Id == t.EntityId);

                if (!entityExists)
                {
                    return (false, "Entity not found.");
                }

                var alreadyExists = await dbContext.ParticipantsEvents
                .AsNoTracking()
                .AnyAsync(pe => pe.EventId == t.EventId && pe.EntityId == t.EntityId);

                if (alreadyExists)
                {
                    return (false, "Participant already registered in this event.");
                }

                var participant = new ParticipantsEvents
                {
                    EventId = t.EventId,
                    EntityId = t.EntityId.Value,
                    Status = true,
                    Grade = "",
                    Comments = ""
                };

                await dbContext.ParticipantsEvents.AddAsync(participant);
                await dbContext.SaveChangesAsync();

                return (true, "Participant inserted successfully.");
            }
            catch (Exception)
            {
                return (false, "Error inserting participant.");
            }
        }
        public async Task<(bool Success, string? Message)> insertParticipantsEventParticipantDescription(ParticipantsEventInsertParticipantDescriptionRequestDTO t)
        {
            if (t.EventId <= 0) return (false, "EventId field empty.");
            if (t.EntityId is null || t.EntityId <= 0) return (false, "EntityId field empty.");
            if (string.IsNullOrWhiteSpace(t.ParticipantDescription)) return (false, "ParticipantDescription field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                var eventExists = await dbContext.Events
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.EventId);
                if (!eventExists)
                    return (false, "Event not found.");

                var entityExists = await dbContext.Entities
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.EntityId);
                if (!entityExists)
                    return (false, "Entity not found.");

                var participant = await dbContext.ParticipantsEvents
                    .FirstOrDefaultAsync(pe => pe.EventId == t.EventId && pe.EntityId == t.EntityId);

                if (participant is null)
                    return (false, "Participant not registered in this event.");

                participant.ParticipantDescription = t.ParticipantDescription;

                await dbContext.SaveChangesAsync();

                return (true, "Participant description updated successfully.");
            }
            catch (Exception)
            {
                return (false, "Error updating participant description.");
            }
        }

        public async Task<(bool Success, string? Message)> cancelParticipantsEvent(ParticipantsEventCancelRequestDTO t)
        {
            if (t.EntityId <= 0) return (false, "Entity field empty.");
            if (t.EventId <= 0) return (false, "EventId field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var participantEvent = await dbContext.ParticipantsEvents
                    .FirstOrDefaultAsync(pe => pe.EventId == t.EventId && pe.EntityId == t.EntityId);

                if (participantEvent is null)
                {
                    return (false, "Participant is not registered in this event.");
                }

                dbContext.ParticipantsEvents.Remove(participantEvent);
                await dbContext.SaveChangesAsync();

                return (true, "Participant removed from event successfully.");
            }
            catch (Exception)
            {
                return (false, "Participant removed from event unsuccessfully.");
            }
        }


    }
}