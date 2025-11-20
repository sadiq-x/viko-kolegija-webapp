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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/profile")] HttpRequestData req, FunctionContext executionContext)
        {
            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token))
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive." });
                return BadResponse;
            }

            var (userId, userName) = JwtAuth.DecoderUserIdUsername(token);
            var userProfileDto = new UserProfileRequestDTO
            {
                EntityId = userId,
                Username = userName
            };

            if (userProfileDto == null || string.IsNullOrEmpty(userProfileDto.Username) || userProfileDto.EntityId == null || userProfileDto.EntityId <= 0) //Verify if entity model don't are false and token don't are empty
            {
                var badResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await badResponse.WriteAsJsonAsync(new { message = "Invalid token: userId not found." });
                return badResponse;
            }

            var user = await _userRepository.authGetUser(userProfileDto);
            if (user is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Users not found."
                });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                user
            });
            return response;
        }

        [Function("getUser")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/users")] HttpRequestData req, FunctionContext executionContext)
        {
            executionContext.Items.TryGetValue("Token", out var userObj); 
            var token = userObj as string; 

            if (string.IsNullOrEmpty(token)) 
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive." });
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token);

            var requestDTO = new UserGetAllRequestDTO
            {
                EntityId = userId
            };

            if (requestDTO is null || !requestDTO.IsValid() || requestDTO.EntityId <= 0)
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Invalid request body.",
                    Error = requestDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() 
                });
                return BadRequest;
            }

            var usersResponse = await _userRepository.GetUsers(requestDTO);
            if (usersResponse is null || usersResponse.Count == 0)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Users not found."
                });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                usersResponse
            });
            return response;
        }

    }
}