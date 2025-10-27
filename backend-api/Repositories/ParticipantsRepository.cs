using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IParticipantsEventsRepository
    {
        Task<List<ParticipantsListFromEventIdResponseDTO>?> getParticipantsFromEventId(ParticipantsListFromEventIdRequestDTO t);
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
    }
}