using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class CreateParticipantsEventGrade
    {
        public readonly IParticipantsEventsRepository _participantsEventsRepository;

        public CreateParticipantsEventGrade(IParticipantsEventsRepository participantsEventsRepository)
        {
            _participantsEventsRepository = participantsEventsRepository;
        }

        [Function("insertParticipantsEventGrade")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insert/participant/grade")] HttpRequestData req, FunctionContext executionContext)
        {
            var insertGradeDTO = await req.ReadFromJsonAsync<ParticipantsEventGradeRequestDTO>();

            if (insertGradeDTO is null)
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

            insertGradeDTO.EntityId = userId;

            if (insertGradeDTO == null || !insertGradeDTO.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = insertGradeDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadRequest;
            }

            var participantsEvent = await _participantsEventsRepository.insertGradeParticipantsEvent(insertGradeDTO);
            if (!participantsEvent.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = participantsEvent.Message
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