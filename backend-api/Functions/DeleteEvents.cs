using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class DeleteEvents
    {
        public readonly IEventsRepository _eventsRepository;

        public DeleteEvents(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        [Function("deleteEvents")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "delete/event")] HttpRequestData req)
        {
            var eventDTO = await req.ReadFromJsonAsync<EventCloseRequestDTO>();

            if (eventDTO == null || !eventDTO.IsValid() || eventDTO.Id <= 0 || eventDTO.CreateById <= 0)
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

            var eventStatus = await _eventsRepository.deleteEvent(eventDTO); //Checking request body with database
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