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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insert/participant/grade")] HttpRequestData req)
        {
            var insertGradeDTO = await req.ReadFromJsonAsync<ParticipantsEventGradeRequestDTO>();

            if (insertGradeDTO == null || !insertGradeDTO.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = insertGradeDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                }); //Response a message if the error exist
                return BadRequest;
            }

            var participantsEvent = await _participantsEventsRepository.insertGradeParticipantsEvent(insertGradeDTO); //Checking request body with database
            if (!participantsEvent.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = participantsEvent.Message
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