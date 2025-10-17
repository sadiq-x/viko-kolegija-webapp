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

        [Function("authUpdateUser")] //Function to update user
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "update/profile")] HttpRequestData req) //Create the Http req and res
        {
            var userUpdateDto = await req.ReadFromJsonAsync<UserUpdateRequestDTO>();

            if (userUpdateDto is null || !userUpdateDto.IsValid()) //Verify if the properties are correctly fields
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send with boolean and message
                await BadResponse.WriteAsJsonAsync(new //Returns boolean and message
                {
                    Success = false,
                    message = "Fields incorrect.",
                    error = userUpdateDto?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                });
                return BadResponse;
            }

            var user = await _userRepository.authUpdateUser(userUpdateDto); //Update the user in database
            if (!user.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send with boolean and message
                await notFoundResponse.WriteAsJsonAsync(new { user.Success, user.Message }); //Returns boolean and message
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send with boolean and message
            await response.WriteAsJsonAsync(new { user.Success, user.Message }); //Returns boolean and message
            return response;
        }
    }
}