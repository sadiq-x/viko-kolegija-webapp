using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface ITopicsRepository
    {
        Task<List<TopicsResponseDTO>?> getTopics();
    }

    public class TopicsRepository : ITopicsRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;

        public TopicsRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<List<TopicsResponseDTO>?> getTopics()
        {
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var result = await dbContext.Topics
                    .AsNoTracking()
                    .Select(x => new TopicsResponseDTO
                    {
                        Id = x.Id,
                        Type = x.Type,
                        Description = x.Description
                    }).ToListAsync();

                if (result == null || result.Count == 0) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}