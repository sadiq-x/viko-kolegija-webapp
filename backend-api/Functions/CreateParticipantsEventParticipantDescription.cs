using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class CreateParticipantsEventParticipantDescription
    {
        public readonly IParticipantsEventsRepository _participantsEventsRepository;

        public CreateParticipantsEventParticipantDescription(IParticipantsEventsRepository participantsEventsRepository)
        {
            _participantsEventsRepository = participantsEventsRepository;
        }

        [Function("insertParticipantsEventParticipantDescription")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insert/participant/description")] HttpRequestData req, FunctionContext executionContext)
        {
            var createParticipantDTO = await req.ReadFromJsonAsync<ParticipantsEventInsertParticipantDescriptionRequestDTO>();

            if (createParticipantDTO is null) 
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

            createParticipantDTO.EntityId = userId;

            Console.WriteLine(createParticipantDTO.ToString());

            if (createParticipantDTO == null || !createParticipantDTO.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = createParticipantDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() 
                });
                return BadRequest;
            }

            var participantCreated = await _participantsEventsRepository.insertParticipantsEventParticipantDescription(createParticipantDTO); //Checking request body with database
            if (!participantCreated.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = participantCreated.Message
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