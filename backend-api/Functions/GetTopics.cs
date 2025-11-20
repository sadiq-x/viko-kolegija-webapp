using System.Net;
using backend_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend_api.Functions
{
    public class GetTopicsFunction
    {
        public readonly ITopicsRepository _topicsRepository;

        public GetTopicsFunction(ITopicsRepository topicsRepository)
        {
            _topicsRepository = topicsRepository;
        }

        [Function("getTopics")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/topics")] HttpRequestData req)
        {
            var topics = await _topicsRepository.getTopics(); 
            if (topics is null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.OK); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Topics not found."
                }); 
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); 
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                topics
            });
            return response; 
        }
    }
}