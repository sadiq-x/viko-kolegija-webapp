using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IUserRepository
    {
        Task<UserLoginResponseModel?> authUserLogin(UserLoginRequestModel t); //Task to check if the login is valid
        Task<bool> authUserRegister(UserRegisterRequestModel e); //Task to crete a new user
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;
        public UserRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<UserLoginResponseModel?> authUserLogin(UserLoginRequestModel t)
        {
            Console.WriteLine($"[LOGIN] Username='{t?.Username}' PasswordHash='{t?.PasswordHash}'");
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
                .Select(join => new UserLoginResponseModel
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

        public async Task<bool> authUserRegister(UserRegisterRequestModel t)
        {
            Console.WriteLine("=== Debug UserRegisterRequestModel ===");
            Console.WriteLine($"Username: {t?.Username}");
            Console.WriteLine($"PasswordHash: {t?.PasswordHash}");
            Console.WriteLine($"ConfirmPasswordHash: {t?.ConfirmPasswordHash}");
            Console.WriteLine($"Name: {t?.Name}");
            Console.WriteLine($"Email: {t?.Email}");
            Console.WriteLine($"NumberPhone: {t?.NumberPhone}");
            Console.WriteLine($"Address: {t?.Address}");
            Console.WriteLine($"Auth: {t?.Auth}");
            Console.WriteLine("=====================================");

            //Validation of string required
            if (string.IsNullOrWhiteSpace(t.Email)) return false;
            if (string.IsNullOrWhiteSpace(t.Username)) return false;
            if (string.IsNullOrWhiteSpace(t.Address)) return false;
            if (string.IsNullOrWhiteSpace(t.PasswordHash)) return false;

            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();

                //Check if the Email or Username exist in database
                if (await dbContext.Set<Users>().AsNoTracking().AnyAsync(u => u.Username == t.Username)) return false;
                if (await dbContext.Set<Entities>().AsNoTracking().AnyAsync(e => e.Email == t.Email)) return false;


                var EntitiesDTO = new Entities
                {
                    Name = t.Name.Trim(),
                    Email = t.Email.Trim().ToLowerInvariant(),
                    NumberPhone = t.NumberPhone.Trim(),
                    Address = t.Address.ToString().Trim(),
                    Auth = false,
                    RoleId = 5
                };
                dbContext.Add(EntitiesDTO);
                var confirmEntities = await dbContext.SaveChangesAsync();

                if (confirmEntities == 0)
                {
                    return false;
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
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}