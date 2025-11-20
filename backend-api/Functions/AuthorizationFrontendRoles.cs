using System.Net;
using backend_api.Models;
using backend_api.Repositories;
using backend_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_api.Functions
{
    public class AuthorizationFrontendRolesFunction
    {
        private readonly IRolesRepository _rolesRepository;

        public AuthorizationFrontendRolesFunction(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        [Function("authFrontendRoles")]
        [Produces("application/json")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "auth/roles")] HttpRequestData req, FunctionContext executionContext)
        {
            executionContext.Items.TryGetValue("Token", out var userObj); 

            var token = userObj as string; 

            if (string.IsNullOrEmpty(token))
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest);  
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive" });  
                return BadResponse;
            }

            var (userId, username, roleType) = JwtAuth.Decoder(token); 

            var roleTypeDto = new RolesRequestDTO 
            {
                EntityId = userId,
                Username = username,
                Type = roleType
            };

            if (roleTypeDto == null || string.IsNullOrEmpty(roleTypeDto.Type) || string.IsNullOrEmpty(roleTypeDto.Username) || roleTypeDto.EntityId == null || roleTypeDto.EntityId <= 0) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.NotFound); 
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Role don't find in JWT token"
                });  
                return BadResponse;
            }

            var roleVerify = await _rolesRepository.authGetRoles(roleTypeDto);  
            if (roleVerify is null || string.IsNullOrEmpty(roleVerify.Type))
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); 
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Role not found."
                });  
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);  
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                roleVerify
            });
            return response;  
        }

    }
}