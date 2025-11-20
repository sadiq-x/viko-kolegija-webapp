using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class LoginUserFunctions
    {
        private readonly IUserRepository _userRepository;
        public LoginUserFunctions(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Function("authLogin")] 
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/login")] HttpRequestData req) 
        {
            var loginUserDto = await req.ReadFromJsonAsync<UserLoginRequestDTO>();

            if (loginUserDto is null || !loginUserDto.IsValid())
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadResponse.WriteAsJsonAsync(new
                {
                    message = "Wrong Credentials.",
                    error = loginUserDto?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var user = await _userRepository.authUserLogin(loginUserDto); 
            if (user is null || string.IsNullOrEmpty(user.Username) || user.EntityId <= 0 || string.IsNullOrEmpty(user.RoleType))
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Login not found."
                });
                return notFoundResponse;
            }

            string token = JwtAuth.GenerateToken(user.Username, user.EntityId, user.RoleType);
            var response = req.CreateResponse(HttpStatusCode.OK); 
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                user,
                token
            });
            return response;
        }
    }
}