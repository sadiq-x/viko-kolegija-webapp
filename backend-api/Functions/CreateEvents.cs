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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/events")] HttpRequestData req, FunctionContext executionContext)
        {
            var eventDTO = await req.ReadFromJsonAsync<EventCreateRequestDTO>();

            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token)) 
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); 
                await BadResponse.WriteAsJsonAsync(new
                {
                    Message = "Token don't receive"
                }); 
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token); 

            eventDTO!.CreateById = userId;

            if (eventDTO is null || !eventDTO.IsValid())
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); 
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Wrong parameters.",
                    error = eventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var eventResponse = await _eventsRepository.createEvent(eventDTO); 
            if (!eventResponse.Success)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    eventResponse.Success,
                    message = eventResponse.Message
                }); 
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); 
            await response.WriteAsJsonAsync(new
            {
                eventResponse.Success,
            });
            return response; 
        }
    }
}