using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class UpdatePasswordFunctions
    {
        private readonly IUserRepository _userRepository;
        public UpdatePasswordFunctions(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Function("authUpdatePassword")] 
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/update/password")] HttpRequestData req, FunctionContext executionContext) //Create the Http req and res
        {
            var userUpdatePasswordDto = await req.ReadFromJsonAsync<UserUpdatePasswordRequestDTO>();

            if (userUpdatePasswordDto is null)
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

            userUpdatePasswordDto.EntityId = userId;

            if (userUpdatePasswordDto == null || !userUpdatePasswordDto.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = userUpdatePasswordDto?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadRequest;
            }

            var user = await _userRepository.authUpdateUserPassword(userUpdatePasswordDto);
            if (!user.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new { user.Success, user.Message }); 
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { user.Success, user.Message }); 
            return response;
        }
    }
}