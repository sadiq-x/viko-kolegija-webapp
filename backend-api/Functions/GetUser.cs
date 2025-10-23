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
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/profile")] HttpRequestData req, FunctionContext executionContext)
        {
            executionContext.Items.TryGetValue("Token", out var userObj); //Get Item Token from Context Function
            var token = userObj as string; //Transform token object into string

            if (string.IsNullOrEmpty(token)) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive" }); //Response send with msg
                return BadResponse;
            }

            var (userId, userName) = JwtAuth.DecoderUserIdUsername(token); //Information of decoded token
            var userProfileDto = new UserProfileRequestDTO //use the userModel for fill the data from token
            {
                EntityId = userId,
                Username = userName
            };

            if (userProfileDto == null || string.IsNullOrEmpty(userProfileDto.Username) || userProfileDto.EntityId == null || userProfileDto.EntityId <= 0) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new { message = "Username and EntityId don't find in JWT token" }); //Response send with msg
                return BadResponse;
            }

            var user = await _userRepository.authGetUser(userProfileDto); //Checking request body with database
            if (user is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new { message = "User not found." }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new { user });
            return response; //Return the response, with username and token Jwt Authorization 
        }

    }
}