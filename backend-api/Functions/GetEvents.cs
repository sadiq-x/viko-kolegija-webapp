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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/events")] HttpRequestData req)
        {
            var events = await _eventsRepository.getAllEvents(); //Checking request body with database
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Events not found."
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

        [Function("getEventsByCreatedById")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/events/byCreatedById")] HttpRequestData req, FunctionContext executionContext)
        {
            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token)) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Message = "Token don't receive"
                }); //Response send with msg
                return BadResponse;
            }

            var (userId, userName) = JwtAuth.DecoderUserIdUsername(token); //Information of decoded token

            var eventById = new EventListByCreateByIdRequestDTO
            {
                CreateById = userId
            };

            if (eventById is null || !eventById.IsValid() || eventById.CreateById <= 0 || eventById.CreateById is null) //Verify if email and password are null, and reject the login
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Wrong parameters.",
                    Error = eventById?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var events = await _eventsRepository.getEventsByCreateById(eventById); //Checking request body with database
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Events not found."
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

        [Function("getEventsByTopic")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "get/events/byTopics")] HttpRequestData req)
        {
            var eventsTopicDTO = await req.ReadFromJsonAsync<EventListByTopicsRequestDTO>();

            if (eventsTopicDTO is null || !eventsTopicDTO.IsValid())
            {
                var BadRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await BadRequest.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Fields incorrect.",
                    Error = eventsTopicDTO?.Validate().Select(e => e.ToString()) ?? new List<string>() //Check all required annotations
                });
                return BadRequest;
            }

            var events = await _eventsRepository.getEventsByTopics(eventsTopicDTO); //Checking request body with database
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
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

        [Function("getEventsByEntityId")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run3(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/events/byEntityId")] HttpRequestData req, FunctionContext executionContext)
        {
            executionContext.Items.TryGetValue("Token", out var userObj);
            var token = userObj as string;

            if (string.IsNullOrEmpty(token)) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Message = "Token don't receive"
                }); //Response send with msg
                return BadResponse;
            }

            var userId = JwtAuth.DecoderUserId(token); //Information of decoded token

            var eventsEntityDTO = new EventListByEntityIdRequestDTO
            {
                EntityId = userId
            };

            if (eventsEntityDTO is null || !eventsEntityDTO.IsValid() || eventsEntityDTO.EntityId <= 0) //Verify if email and password are null, and reject the login
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Wrong parameters.",
                    Error = eventsEntityDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var events = await _eventsRepository.getEventsByEntityId(eventsEntityDTO); //Checking request body with database
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
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