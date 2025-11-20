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

        [Function("authRegister")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/register")] HttpRequestData req)
        {
            var registerUserDto = await req.ReadFromJsonAsync<UserRegisterRequestDTO>();

            if (registerUserDto is null || !registerUserDto.IsValid())
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Fill the form correctly.",
                    error = registerUserDto?.Validate().Select(e => e.ErrorMessage) ?? new List<string>()
                });
                return badResponse;
            }

            var registerUser = await _userRepository.authUserRegister(registerUserDto);

            if (!registerUser.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = registerUser.Message
                });
                return notFoundResponse;
            }


            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                Message = registerUser.Message
            });
            return response;
        }
    }
}