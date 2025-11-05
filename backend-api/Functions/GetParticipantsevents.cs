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

        [Function("getParticipantFromEvent")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/participants/{eventId:int}")] HttpRequestData req, int eventId)
        {
            var participantEventDTO = new ParticipantsListFromEventIdRequestDTO { EventId = eventId };

            if (participantEventDTO == null || participantEventDTO.EventId <= 0 || !participantEventDTO.IsValid())
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "EventId not found.",
                    error = participantEventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var participantsEvent = await _participantsEventsRepository.getParticipantsFromEventId(participantEventDTO); //Checking request body with database
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