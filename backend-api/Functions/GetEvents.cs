using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class GetEvents
    {
        public readonly IEventsRepository _eventsRepository;

        public GetEvents(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        [Function("getAllEvents")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/events")] HttpRequestData req)
        {
            var events = await _eventsRepository.getEvents();
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Events not found."
                }); 
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); 
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

            if (string.IsNullOrEmpty(token))
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); 
                await BadResponse.WriteAsJsonAsync(new
                {
                    Message = "Token don't receive"
                }); 
                return BadResponse;
            }

            var (userId, userName) = JwtAuth.DecoderUserIdUsername(token); 

            var eventById = new EventByCreateByIdRequestDTO
            {
                CreateById = userId
            };

            if (eventById is null || !eventById.IsValid() || eventById.CreateById <= 0 || eventById.CreateById is null) 
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); 
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Wrong parameters.",
                    Error = eventById?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var events = await _eventsRepository.getEventsByCreateById(eventById); 
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Events not found."
                }); 
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); 
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                events
            });
            return response; 
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
                    Error = eventsTopicDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadRequest;
            }

            var events = await _eventsRepository.getEventsByTopics(eventsTopicDTO); 
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Events not found."
                });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                events
            });
            return response; 
        }

        [Function("getEventsByEntityId")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run3(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/events/byEntityId")] HttpRequestData req, FunctionContext executionContext)
        {
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

            var eventsEntityDTO = new EventStudentByEntityIdRequestDTO
            {
                EntityId = userId
            };

            if (eventsEntityDTO is null || !eventsEntityDTO.IsValid() || eventsEntityDTO.EntityId <= 0) 
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); 
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Wrong parameters.",
                    Error = eventsEntityDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var events = await _eventsRepository.getEventsStudentByEntityId(eventsEntityDTO); 
            if (events is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Events not found."
                }); 
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                events
            });
            return response; 
        }

        [Function("getEventsByEventId")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run4(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/events/byEventId/{eventId:int}")] HttpRequestData req, int eventId)
        {
            var eventDTO = new EventByEventIdRequestDTO { Id = eventId };

            if (eventDTO is null || !eventDTO.IsValid() || eventDTO.Id <= 0)
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); 
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Wrong parameters.",
                    Error = eventDTO?.Validate().Select(e => e.ToString()) ?? new List<string>()
                });
                return BadResponse;
            }

            var eventResponse = await _eventsRepository.getEventsByEventId(eventDTO); 
            if (eventResponse is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK);
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Events not found."
                }); 
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); 
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                eventResponse
            });
            return response;
        }

    }
}