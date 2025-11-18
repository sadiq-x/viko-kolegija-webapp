using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class CreateEvents
    {
        public readonly IEventsRepository _eventsRepository;

        public CreateEvents(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        [Function("createEventsTeacher")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/events/teacher")] HttpRequestData req, FunctionContext executionContext)
        {
            var eventDTO = await req.ReadFromJsonAsync<EventCreateTeacherRequestDTO>();

            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token))
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadResponse.WriteAsJsonAsync(new
                {
                    Message = "Token don't receive"
                });
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token);

            eventDTO!.CreateById = userId;

            if (eventDTO is null || !eventDTO.IsValid())
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Wrong parameters.",
                    error = eventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var eventResponse = await _eventsRepository.createEvent_Teacher(eventDTO);
            if (!eventResponse.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    eventResponse.Success,
                    message = eventResponse.Message
                });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                eventResponse.Success,
            });
            return response;
        }

        [Function("createEventsAdmin")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run2(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/events/admin")] HttpRequestData req, FunctionContext executionContext)
        {
            var eventDTO = await req.ReadFromJsonAsync<EventCreateAdminRequestDTO>();

            if (eventDTO is null)
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
                await badResponse.WriteAsJsonAsync(new { message = "Invalid token." });
                return badResponse;
            }

            eventDTO.AdminId = userId;

            if (eventDTO == null || !eventDTO.IsValid() || eventDTO.AdminId is null)
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = eventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadRequest;
            }

            var eventResponse = await _eventsRepository.createEvent_Admin(eventDTO);
            if (!eventResponse.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    eventResponse.Success,
                    message = eventResponse.Message
                });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                eventResponse.Success,
            });
            return response;
        }

    }
}