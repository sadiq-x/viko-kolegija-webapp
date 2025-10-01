using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;


namespace backend_api.Functions
{
    public class RegisterUserFunctions
    {
        private readonly IUserRepository _userRepository;
        public RegisterUserFunctions(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Function("authRegister")] //Function to do login
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/register")] HttpRequestData req)
        {
            var registerUserDto = await req.ReadFromJsonAsync<UserRegisterRequestDTO>();

            if (registerUserDto is null || !registerUserDto.IsValid())
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new
                {
                    message = "Fill the form correctly.",
                    error = registerUserDto?.Validate().Select(e => e.ErrorMessage) ?? new List<string>()
                });
                return badResponse;
            }

            var registerUser = await _userRepository.authUserRegister(registerUserDto);

            if (!registerUser.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteAsJsonAsync(new { message = $"{registerUser.Message}." });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { message = "Register Success." });
            return response;
        }
    }
}