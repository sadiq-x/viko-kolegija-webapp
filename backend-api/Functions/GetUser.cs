using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_api.Functions
{
    public class GetUserFunction
    {
        private readonly IUserRepository _userRepository;

        public GetUserFunction(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Function("authGetUser")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "get/profile")] HttpRequestData req)
        {
            var userProfileDto = await req.ReadFromJsonAsync<UserProfileRequestDTO>();

            if (userProfileDto is null || !userProfileDto.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new { message = "Fields incorrect." });
                return BadRequest;
            }

            var user = await _userRepository.authGetUser(userProfileDto); //Checking request body with database
            if (user is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new { message = "User not found." }); //Reponse a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new { user });
            return response; //Return the response, with username and token Jwt Authorization 
        }

    }
}