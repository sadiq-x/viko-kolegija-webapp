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
    public class GetTeachers
    {
        private readonly IUserRepository _userRepository;

        public GetTeachers(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Function("getTeachers")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/teachers")] HttpRequestData req, FunctionContext executionContext)
        {
            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token))
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive" });
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token);

            if (userId <= 0 || userId is null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await badResponse.WriteAsJsonAsync(new { message = "Invalid token." });
                return badResponse;
            }

            var teacherDTO = new TeacherRequestDTO
            {
                EntityId = userId,
            };

            if (teacherDTO == null || !teacherDTO.IsValid() || teacherDTO.EntityId is null)
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = teacherDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() 
                });
                return BadRequest;
            }

            var teachers = await _userRepository.GetTeachers(teacherDTO);
            if (teachers is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Teachers not found."
                });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); 
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                teachers
            });
            return response; 
        }
    }
}