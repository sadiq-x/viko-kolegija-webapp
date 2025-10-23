using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IRolesRepository
    {
        Task<RolesResponseDTO> authGetRoles(RolesRequestDTO t); //Task to update user 
    }

    public class RolesRepository : IRolesRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;
        public RolesRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<RolesResponseDTO?> authGetRoles(RolesRequestDTO t)
        {
            if (string.IsNullOrWhiteSpace(t.Type)) return null;
            if (string.IsNullOrWhiteSpace(t.Username)) return null;
            if (t.EntityId <= 0) return null;

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                var result = await dbContext.Users
                    .AsNoTracking()
                    .Where(u =>
                        u.Username == t.Username &&
                        u.EntityId == t.EntityId &&
                        u.Entity.Roles.Type == t.Type)
                    .Select(u => new RolesResponseDTO
                    {
                        Type = u.Entity.Roles.Type
                    })
                    .FirstOrDefaultAsync();

                return result; // Se não encontrar devolve null
            }
            catch
            {
                throw;
            }
        }
    }
}