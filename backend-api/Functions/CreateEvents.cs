using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class CreateCoursesFunction
    {
        public readonly IEventsRepository _eventsRepository;

        public CreateCoursesFunction(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        [Function("createEvents")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/events")] HttpRequestData req)
        {
            var createEvent = await req.ReadFromJsonAsync<EventCreateRequestDTO>();

            if (createEvent is null || !createEvent.IsValid()) //Verify if email and password are null, and reject the login
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Wrong parameters.",
                    error = createEvent?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var eventCreated = await _eventsRepository.createEvent(createEvent); //Checking request body with database
            if (!eventCreated.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    eventCreated.Success,
                    message = eventCreated.Message
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                eventCreated.Success,
            });
            return response; //Return
        }
    }
}