using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class UpdateEvent
    {
        public readonly IEventsRepository _eventsRepository;

        public UpdateEvent(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        [Function("updateEvents")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "update/event")] HttpRequestData req, FunctionContext executionContext)
        {
            var updateEventDTO = await req.ReadFromJsonAsync<EventEditAdminRequestDTO>();

            if (updateEventDTO is null) 
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

            updateEventDTO.AdminId = userId;

            if (updateEventDTO == null || !updateEventDTO.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.OK);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = updateEventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() 
                });
                return BadRequest;
            }

            var eventUpdated = await _eventsRepository.updateEvent_Admin(updateEventDTO); 
            if (!eventUpdated.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = eventUpdated.Message
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