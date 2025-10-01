using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IUserRepository
    {
        Task<UserLoginResponseDTO?> authUserLogin(UserLoginRequestDTO t); //Task to check if the login is valid
        Task<(bool Success, string? Message)> authUserRegister(UserRegisterRequestDTO t); //Task to crete a new user
        Task<(bool Success, string? Message)> authUpdateUserPassword(UserUpdatePasswordRequestDTO t); //Task to update user password
        Task<(bool Success, string? Message)> authUpdateUser(UserUpdateRequestDTO t); //Task to update user 
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;
        public UserRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<UserLoginResponseDTO?> authUserLogin(UserLoginRequestDTO t)
        {
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                var result = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Username == t.Username && u.PasswordHash == t.PasswordHash)
                .Join(dbContext.Entities,
                    u => u.EntityId,
                    e => e.Id,
                    (u, e) => new { u, e })
                .Select(join => new UserLoginResponseDTO
                {
                    EntityId = join.u.EntityId,
                    Username = join.u.Username
                })
                .FirstOrDefaultAsync();

                if (result is null) return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> authUserRegister(UserRegisterRequestDTO t)
        {
            if (string.IsNullOrWhiteSpace(t.Username)) return (false, "Username field empty.");
            if (string.IsNullOrWhiteSpace(t.PasswordHash)) return (false, "Password field empty.");
            if (string.IsNullOrWhiteSpace(t.Name)) return (false, "Name field empty.");
            if (string.IsNullOrWhiteSpace(t.Email)) return (false, "Email field empty.");
            if (string.IsNullOrWhiteSpace(t.NumberPhone)) return (false, "Number phone field empty.");
            if (string.IsNullOrWhiteSpace(t.Address)) return (false, "Address field empty.");

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                //Check if the Email or Username exist in database
                if (await dbContext.Set<Users>().AsNoTracking().AnyAsync(u => u.Username == t.Username)) return (false, "Username already exist.");
                if (await dbContext.Set<Entities>().AsNoTracking().AnyAsync(e => e.Email == t.Email)) return (false, "Email already exist.");

                var EntitiesDTO = new Entities
                {
                    Name = t.Name.Trim(),
                    Email = t.Email.Trim().ToLowerInvariant(),
                    NumberPhone = t.NumberPhone.Trim(),
                    Address = t.Address.ToString().Trim(), //Need to be tested
                    Auth = false,
                    RoleId = 5 //Set the Unauthorized
                };

                dbContext.Entities.Add(EntitiesDTO);
                var confirmEntities = await dbContext.SaveChangesAsync();

                if (confirmEntities == 0)
                {
                    return (false, "Entity not created in database.");
                }

                var UsersDTO = new Users
                {
                    EntityId = EntitiesDTO.Id,
                    Username = t.Username,
                    PasswordHash = t.PasswordHash,
                };

                dbContext.Add(UsersDTO);
                var confirmUsers = await dbContext.SaveChangesAsync();

                if (confirmUsers == 0)
                {
                    return (false, "User not created in database.");
                }

                return (true, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool Success, string? Message)> authUpdateUserPassword(UserUpdatePasswordRequestDTO t)
        {
            if (t.Id <= 0) return (false, "Id field empty.");
            if (string.IsNullOrWhiteSpace(t.Username)) return (false, "Username field empty.");
            if (string.IsNullOrWhiteSpace(t.PasswordHash)) return (false, "Password field empty.");

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                var user = await dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == t.Id && u.Username == t.Username);

                if (user is null)
                    return (false, "User not found.");

                user.PasswordHash = t.PasswordHash;

                await dbContext.SaveChangesAsync();
                return (true, "Password update successfully.");
            }
            catch (Exception)
            {
                return (false, "Password update unsuccessful.");
            }
        }

        public async Task<(bool Success, string? Message)> authUpdateUser(UserUpdateRequestDTO t)
        {
            if (t.Id <= 0) return (false, "Id field empty.");
            if (string.IsNullOrWhiteSpace(t.Username)) return (false, "Username field empty.");
            if (string.IsNullOrWhiteSpace(t.Name)) return (false, "Name field empty.");
            if (string.IsNullOrWhiteSpace(t.Email)) return (false, "Email field empty.");
            if (string.IsNullOrWhiteSpace(t.NumberPhone)) return (false, "Number phone field empty.");
            if (string.IsNullOrWhiteSpace(t.Address)) return (false, "Address field empty.");

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                var user = await dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == t.Id && u.Username == t.Username);

                if (user is null)
                    return (false, "User not found.");

                var entity = await dbContext.Entities
                .SingleOrDefaultAsync(e => e.Id == user.EntityId);

                if (entity is null)
                    return (false, "Entity not found.");

                entity.Name = t.Name.Trim();
                entity.Email = t.Email.Trim().ToLowerInvariant();

                if (!string.IsNullOrWhiteSpace(t.Image))
                    entity.Image = t.Image;

                entity.NumberPhone = t.NumberPhone.Trim();
                entity.Address = t.Address.Trim();

                await dbContext.SaveChangesAsync();
                return (true, "User update successfully.");
            }
            catch (Exception)
            {
                return (false, "User update unsuccessful.");
            }
        }
    }
}