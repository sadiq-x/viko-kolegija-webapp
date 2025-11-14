using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class GetParticipantsEventsFunction
    {
        public readonly IParticipantsEventsRepository _participantsEventsRepository;

        public GetParticipantsEventsFunction(IParticipantsEventsRepository participantsEventsRepository)
        {
            _participantsEventsRepository = participantsEventsRepository;
        }

        [Function("getParticipantFromEventId-User")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/participants/{eventId:int}")] HttpRequestData req, int eventId, FunctionContext executionContext)
        {
            var participantEventDTO = new ParticipantsListFromEventIdUserRequestDTO { EventId = eventId };

            if (participantEventDTO is null) //Verify the Dto
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadResponse.WriteAsJsonAsync(new { message = "Invalid request body." });
                return BadResponse;
            }

            executionContext.Items.TryGetValue("Token", out var userObj); //Get Item Token from Context Function
            var token = userObj as string; //Transform token object into string

            if (string.IsNullOrEmpty(token)) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.Unauthorized); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive." }); //Response send with msg
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token);

            if (userId <= 0 || userId is null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await badResponse.WriteAsJsonAsync(new { message = "Invalid token: userId not found." });
                return badResponse;
            }

            participantEventDTO.EntityId = userId;

            if (participantEventDTO == null || !participantEventDTO.IsValid() || participantEventDTO.EntityId is null)
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = participantEventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                });
                return BadRequest;
            }

            var participantsEvent = await _participantsEventsRepository.getParticipantsFromEventId_User(participantEventDTO); //Checking request body with database
            if (participantsEvent is null || participantsEvent.Count == 0)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Participants not found."
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                participantsEvent
            });
            return response; //Return
        }
    
        [Function("getParticipantFromEventId-Teacher")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/participants/teacher/{eventId:int}")] HttpRequestData req, int eventId)
        {
            var participantEventDTO = new ParticipantsListFromEventIdTeacherRequestDTO { EventId = eventId };

            if (participantEventDTO is null || !participantEventDTO.IsValid() || participantEventDTO.EventId <= 0)
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Invalid request body.",
                    Error = participantEventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                });
                return BadRequest;
            }

            var participantsEvent = await _participantsEventsRepository.getParticipantsFromEventId_Teacher(participantEventDTO); //Checking request body with database
            if (participantsEvent is null || participantsEvent.Count == 0)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Participants not found."
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                participantsEvent
            });
            return response; //Return
        }
    
    }
}