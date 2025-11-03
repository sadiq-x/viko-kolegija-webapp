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
            var insertGrade = await req.ReadFromJsonAsync<ParticipantsEventGradeRequestDTO>();

            if (insertGrade == null || !insertGrade.IsValid())
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Fields incorrect.",
                    error = insertGrade?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var participantsEvent = await _participantsEventsRepository.insertGradeParticipantsEvent(insertGrade); //Checking request body with database
            if (!participantsEvent.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = participantsEvent.Message
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