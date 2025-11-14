using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class UpdateStatusEvent
    {
        public readonly IEventsRepository _eventsRepository;

        public UpdateStatusEvent(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        [Function("updateEventsStatusClose")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "update/event/close")] HttpRequestData req, FunctionContext executionContext)
        {
            var eventDTO = await req.ReadFromJsonAsync<EventChangeStatusRequestDTO>();

            if (eventDTO is null) //Verify the Dto
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadResponse.WriteAsJsonAsync(new { message = "Invalid request body." });
                return BadResponse;
            }

            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token)) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Message = "Token don't receive"
                }); //Response send with msg
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token); //Information of decoded token

            if (userId <= 0 || userId is null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await badResponse.WriteAsJsonAsync(new { message = "Invalid token: userId not found." });
                return badResponse;
            }

            eventDTO.CreateById = userId;

            if (!eventDTO.IsValid() || eventDTO.Id <= 0 || eventDTO.CreateById <= 0 || eventDTO.CreateById is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Fields incorrect.",
                    error = eventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var eventStatus = await _eventsRepository.updateEventStatusToClose(eventDTO); //Checking request body with database
            if (!eventStatus.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = eventStatus.Message
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true,
            });
            return response; //Return
        }
    
        [Function("updateEventsStatusOngoing")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "update/event/ongoing")] HttpRequestData req, FunctionContext executionContext)
        {
            var eventDTO = await req.ReadFromJsonAsync<EventChangeStatusRequestDTO>();

            if (eventDTO is null) //Verify the Dto
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadResponse.WriteAsJsonAsync(new { message = "Invalid request body." });
                return BadResponse;
            }

            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token)) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Message = "Token don't receive"
                }); //Response send with msg
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token); //Information of decoded token

            if (userId <= 0 || userId is null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await badResponse.WriteAsJsonAsync(new { message = "Invalid token: userId not found." });
                return badResponse;
            }

            eventDTO.CreateById = userId;

            if (!eventDTO.IsValid() || eventDTO.Id <= 0 || eventDTO.CreateById <= 0 || eventDTO.CreateById is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Fields incorrect.",
                    error = eventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var eventStatus = await _eventsRepository.updateEventStatusToOngoing(eventDTO); //Checking request body with database
            if (!eventStatus.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = eventStatus.Message
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true,
            });
            return response; //Return
        }
    
    }
}