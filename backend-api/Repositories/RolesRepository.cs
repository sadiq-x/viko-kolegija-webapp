using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IRolesRepository
    {
        Task<RolesResponseDTO?> authGetRoles(RolesRequestDTO t);
        Task<(bool Success, string? Message)> updateUserRole(RolesUpdateUserRequestDTO t);
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

                return result; 
            }
            catch
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> updateUserRole(RolesUpdateUserRequestDTO t)
        {
            if (t.Id <= 0) return (false, "Id field empty.");
            if (string.IsNullOrWhiteSpace(t.Email)) return (false, "Email field empty.");
            if (string.IsNullOrWhiteSpace(t.Type)) return (false, "Type field empty.");
            if (t.EntityId <= 0) return (false, "EntityId field empty.");

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                var isAdmin = await dbContext.Entities
                    .AsNoTracking()
                    .AnyAsync(e => e.Id == t.EntityId && e.Roles.Type == "Admin");

                if (!isAdmin)
                    return (false, "User is not Admin.");

                var entity = await dbContext.Entities
                    .FirstOrDefaultAsync(e => e.Id == t.Id && e.Email == t.Email);

                if (entity is null)
                    return (false, "User not found.");

                var role = await dbContext.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Type == t.Type);

                if (role is null)
                    return (false, "Role type not found.");

                entity.RoleId = role.Id;

                await dbContext.SaveChangesAsync();

                return (true, "User role updated successfully.");
            }
            catch (Exception)
            {
                return (false, "User role updated unsuccessfully.");
            }
        }
    }
}