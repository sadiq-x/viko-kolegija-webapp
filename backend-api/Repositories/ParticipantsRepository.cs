using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IParticipantsEventsRepository
    {
        Task<List<ParticipantsListFromEventIdResponseDTO>?> getParticipantsFromEventId(ParticipantsListFromEventIdRequestDTO t);
        Task<(bool Success, string? Message)> insertGradeParticipantsEvent(ParticipantsEventGradeRequestDTO t);
    }

    public class ParticipantsEventsRepository : IParticipantsEventsRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;

        public ParticipantsEventsRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<List<ParticipantsListFromEventIdResponseDTO>?> getParticipantsFromEventId(ParticipantsListFromEventIdRequestDTO t)
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
                    .Where(pe => pe.EventId == t.EventId) // filtra pelo evento
                    .Select(u => new ParticipantsListFromEventIdResponseDTO
                    {
                        Id = u.Id,
                        EntityId = u.EntityId,
                        Name = u.Entity.Name,
                        Email = u.Entity.Email,
                        Status = u.Status,
                        Grade = u.Grade,
                        Comments = u.Comments,

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

                // 4) Atualizar comentários apenas se vierem preenchidos (senão deixa como está
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

    }
}