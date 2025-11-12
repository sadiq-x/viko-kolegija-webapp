using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class CreateParticipantsEventInEvent
    {
        public readonly IParticipantsEventsRepository _participantsEventsRepository;

        public CreateParticipantsEventInEvent(IParticipantsEventsRepository participantsEventsRepository)
        {
            _participantsEventsRepository = participantsEventsRepository;
        }

        [Function("insertParticipantsEventInEvent")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insert/participant/event")] HttpRequestData req, FunctionContext executionContext)
        {
            var createParticipantDTO = await req.ReadFromJsonAsync<ParticipantsEventInsertInEventIdRequestDTO>();

            if (createParticipantDTO is null) //Verify the Dto
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

            createParticipantDTO.EntityId = userId;

            if (createParticipantDTO == null || !createParticipantDTO.IsValid() || createParticipantDTO.EntityId is null)
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = createParticipantDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                });
                return BadRequest;
            }

            var participantCreated = await _participantsEventsRepository.insertParticipantsEventInEvent(createParticipantDTO); //Checking request body with database
            if (!participantCreated.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = participantCreated.Message
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true
            });
            return response; //Return
        }

    }
}