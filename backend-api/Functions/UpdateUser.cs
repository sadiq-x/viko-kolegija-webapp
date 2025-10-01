using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class UpdateUserFunctions
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserFunctions(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Function("authUpdatePassword")] //Function to do login
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/update/user")] HttpRequestData req) //Create the Http req and res
        {
            var userUpdateDto = await req.ReadFromJsonAsync<UserUpdateRequestDTO>();

            if (userUpdateDto is null || !userUpdateDto.IsValid()) //Verify if email and password are null, and reject the login
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    message = "Fields incorrect.",
                    error = userUpdateDto?.Validate().Select(e => e.ToString()) ?? new List<string>()
                }); 
                return BadResponse;
            }

            var user = await _userRepository.authUpdateUser(userUpdateDto); //Checking request body with database
            if (!user.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteStringAsync($"${user.Message}."); //Reponse a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new { user.Message });
            return response; //Return the response, with username and token Jwt Authorization 
        }
    }
}