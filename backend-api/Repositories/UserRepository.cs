using backend_api.Context;
using backend_api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Repositories
{
    public interface IUserRepository
    {
        Task<bool> authUserLogin(UserLoginRequestModel t); //Task to check if the login is valid
        Task<bool> authUserRegister(UserRegisterRequestModel u,EntityRegisterRequestModel e); //Task to crete a new user
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<MasterDbContext> _readContextFactory;
        public UserRepository(IDbContextFactory<MasterDbContext> readContextFactory)
        {
            _readContextFactory = readContextFactory;
        }

        public async Task<bool> authUserLogin(UserLoginRequestModel t)
        {
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();


                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> authUserRegister(UserRegisterRequestModel u, EntityRegisterRequestModel e)
        {
            try
            {
                using var dbContext = _readContextFactory.CreateDbContext();
                

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}