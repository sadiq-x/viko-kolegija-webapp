using System.Net;
using System.Text.Json;
using backend_api.Models;
using backend_api.Repositories;
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
            // Usa o helper do runtime que já lê e aplica as opções globais de Json (camelCase incluído)
            var registerUserDto = await req.ReadFromJsonAsync<UserRegisterRequestModel>();

            if (registerUserDto is null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { message = "Fill the form correctly." });
                return badResponse;
            }

            var registerUser = await _userRepository.authUserRegister(registerUserDto);

            if (!registerUser)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("Username or Email need to be different.");
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { msg = "Register Success" });
            return response;
        }
    }
}