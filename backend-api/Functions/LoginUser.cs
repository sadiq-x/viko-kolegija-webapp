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

        [Function("authLogin")] //Function to do login
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/login")] HttpRequestData req) //Create the Http req and res
        {
            var loginUserDto = await req.ReadFromJsonAsync<UserLoginRequestDTO>();

            if (loginUserDto is null || !loginUserDto.IsValid()) //Verify if email and password are null, and reject the login
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    message = "Wrong Credentials.",
                    error = loginUserDto?.Validate().Select(e => e.ToString()) ?? new List<string>()
                }); 
                return BadResponse;
            }

            var user = await _userRepository.authUserLogin(loginUserDto); //Checking request body with database
            if (user is null)
            {
                // Parse the request body to extract username and password
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteStringAsync("Login not found."); //Reponse a message if the error exist
                return notFoundResponse;
            }

            string token = JwtAuth.GenerateToken(user.Username, user.EntityId); //Create token with username and id
            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new { user, token });
            return response; //Return the response, with username and token Jwt Authorization 
        }
    }
}