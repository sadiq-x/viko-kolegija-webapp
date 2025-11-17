using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface ITopicsRepository
    {
        Task<List<TopicsResponseDTO>?> getTopics();
        Task<(bool Success, string? Message)> createTopics(TopicsCreateRequestDTO t);
        Task<(bool Success, string? Message)> deleteTopics(TopicsDeleteRequestDTO t);
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

        public async Task<(bool Success, string? Message)> createTopics(TopicsCreateRequestDTO t)
        {
            if (string.IsNullOrEmpty(t.Type)) return (false, "Type field empty.");
            if (string.IsNullOrEmpty(t.Description)) return (false, "Description field empty.");
            if (t.EntityId <= 0) return (false, "EntityId field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var isAdmin = await dbContext.Entities
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.EntityId && e.Roles.Type == "Admin");

                if (!isAdmin)
                    return (false, "User is not Admin.");

                var topicExists = await dbContext.Topics
                    .AsNoTracking()
                    .AnyAsync(tp => tp.Type == t.Type);

                if (topicExists)
                    return (false, "This topic already exist.");

                var newTopic = new Topics
                {
                    Type = t.Type.Trim(),
                    Description = t.Description
                };

                dbContext.Topics.Add(newTopic);
                await dbContext.SaveChangesAsync();

                return (true, "Topic created successfully.");
            }
            catch (Exception)
            {
                return (false, "Topic created unsuccessfully.");
            }

        }

        public async Task<(bool Success, string? Message)> deleteTopics(TopicsDeleteRequestDTO t)
        {
            if (t.Id <= 0) return (false, "EntityId field empty.");
            if (string.IsNullOrEmpty(t.Type)) return (false, "Type field empty.");
            if (string.IsNullOrEmpty(t.Description)) return (false, "Description field empty.");
            if (t.EntityId <= 0) return (false, "EntityId field empty.");
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var isAdmin = await dbContext.Entities
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.EntityId && e.Roles.Type == "Admin");

                if (!isAdmin)
                    return (false, "User is not Admin.");

                var topicExists = await dbContext.Topics
                    .AsNoTracking()
                    .FirstOrDefaultAsync(tp => tp.Type == t.Type && tp.Id == t.Id);

                if (topicExists is null)
                    return (false, "Topic not found.");

                dbContext.Topics.Remove(topicExists);
                await dbContext.SaveChangesAsync();

                return (true, "Topic delete successfully.");
            }
            catch (Exception)
            {
                return (false, "Topic delete unsuccessfully.");
            }

        }

    }
}