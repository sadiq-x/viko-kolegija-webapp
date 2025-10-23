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
            executionContext.Items.TryGetValue("Token", out var userObj); //Get Item Token from Context Function

            var token = userObj as string; //Transform token object into string

            if (string.IsNullOrEmpty(token)) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.BadRequest); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new { message = "Token don't receive" }); //Response send with msg
                return BadResponse;
            }

            var (userId, username, roleType) = JwtAuth.Decoder(token); //Information of decoded token
            Console.WriteLine("Role type -  " + roleType.ToString());
            Console.WriteLine("username -  " + username.ToString());
            var roleTypeDto = new RolesRequestDTO //use the userModel for fill the data from token
            {
                EntityId = userId,
                Username = username,
                Type = roleType
            };

            if (roleTypeDto == null || string.IsNullOrEmpty(roleTypeDto.Type) || string.IsNullOrEmpty(roleTypeDto.Username) || roleTypeDto.EntityId == null || roleTypeDto.EntityId <= 0) //Verify if entity model don't are false and token don't are empty
            {
                var BadResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await BadResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Role don't find in JWT token"
                }); //Response send with msg
                return BadResponse;
            }

            var roleVerify = await _rolesRepository.authGetRoles(roleTypeDto); //Checking request body with database
            if (roleVerify is null || string.IsNullOrEmpty(roleVerify.Type))
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound); //Create a response to send
                await notFoundResponse.WriteAsJsonAsync(new
                {
                    Success = false,
                    message = "Role not found."
                }); //Response a message if the error exist
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK); //Create a response to send
            await response.WriteAsJsonAsync(new
            {
                Success = true,
                roleVerify
            });
            return response; //Return the response, with username and token Jwt Authorization 
        }

    }
}