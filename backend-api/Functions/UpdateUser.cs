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

        [Function("authUpdateUser")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "update/profile")] HttpRequestData req, FunctionContext executionContext) //Create the Http req and res
        {
            var userUpdateDto = await req.ReadFromJsonAsync<UserUpdateRequestDTO>();

            if (userUpdateDto is null)
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadResponse.WriteAsJsonAsync(new { message = "Invalid request body." });
                return BadResponse;
            }

            executionContext.Items.TryGetValue("Token", out var userObj);

            var token = userObj as string;

            if (string.IsNullOrEmpty(token))
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive." });
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token);

            if (userId <= 0 || userId is null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await badResponse.WriteAsJsonAsync(new { message = "Invalid token: userId not found." });
                return badResponse;
            }

            userUpdateDto.EntityId = userId;

            if (userUpdateDto == null || !userUpdateDto.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = userUpdateDto?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadRequest;
            }

            var user = await _userRepository.authUpdateUser(userUpdateDto);
            if (!user.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    user.Message
                });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                Success = true
            });
            return response;
        }
    }
}