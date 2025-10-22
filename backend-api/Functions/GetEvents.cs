using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class GetCoursesFunction
    {
        public readonly IEventsRepository _eventsRepository;

        public GetCoursesFunction(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        [Function("getAllEvents")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/all/topics")] HttpRequestData req)
        {
            var events = await _eventsRepository.getAllEvents(); //Checking request body with database
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Events not found."
                }); //Reponse a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                events
            });
            return response; //Return
        }

        [Function("getEventsById")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "get/byId/topics")] HttpRequestData req)
        {
            var eventById = await req.ReadFromJsonAsync<EventListByIdRequestDTO>();


            if (eventById is null || !eventById.IsValid()) //Verify if email and password are null, and reject the login
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Wrong parameters.",
                    error = eventById?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var events = await _eventsRepository.getEventsById(eventById); //Checking request body with database
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Events not found."
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                events
            });
            return response; //Return
        }
    }
}