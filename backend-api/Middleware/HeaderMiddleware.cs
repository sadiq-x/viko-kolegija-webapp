using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using backend_api.Services;

namespace backend_api.Middleware
{
    public class HeaderValidationMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            // Get the Http request and get value Bearer
            var httpRequest = await context.GetHttpRequestDataAsync();
            var tokenWithBearer = httpRequest?.Headers.GetValues("Authorization").FirstOrDefault();
            var token = tokenWithBearer?.StartsWith("Bearer ") == true
            ? tokenWithBearer.Substring("Bearer ".Length).Trim()
            : tokenWithBearer;

            // Headers verification of "Authorization"
            if (httpRequest == null || string.IsNullOrEmpty(token))
            {   //If the parameter are null or empty are be rejected and give Unauthorized
                var BadResponse = httpRequest?.CreateResponse(HttpStatusCode.Unauthorized);
                if (BadResponse != null)
                {
                    await BadResponse.WriteStringAsync("Unauthorized: Missing token.");
                    context.GetInvocationResult().Value = BadResponse; //Interrupts the function execution here
                }
                // Stop the middleware here
                return;
            };

            var isValidToken = JwtAuth.ValidateToken(token); //Assuming ValidateToken is a method in your JwtAuthService
            if (!isValidToken)
            {
                var BadResponse = httpRequest.CreateResponse(HttpStatusCode.Unauthorized);
                await BadResponse.WriteStringAsync("Unauthorized: Invalid token.");
                context.GetInvocationResult().Value = BadResponse; //Interrupts the function execution here
                return; // Stops further execution of the function
            };
            context.Items["Token"] = token; //Save token in a local in a Function
            await next(context);  // If validation passes, continue to the next handler, only call next if token is valid
        }
    }
}
