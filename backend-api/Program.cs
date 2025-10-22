using System.Text.Json;
using System.Text.Json.Serialization;
using backend_api.Context;
using backend_api.Interceptors;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using backend_api.Middleware;
using backend_api.Repositories;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

//Get the Connection String from local.settings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["Values:DefaultConnection"];

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' não configurada.");

//Middleware for Authorization
builder.UseWhen<HeaderValidationMiddleware>(context =>
{
    return context.FunctionDefinition.InputBindings.Values.Any(v => v.Type == "httpTrigger")
    && context.FunctionDefinition.Name != "authLogin" && context.FunctionDefinition.Name != "authRegister";
});

//Adding the connection string sql
builder.Services.AddDbContextFactory<MasterDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.PropertyNameCaseInsensitive = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton<RemoveAliasInterceptor>();

//Adding the interfaces of repositories
builder.Services.AddSingleton<IUserRepository, UserRepository>(); //User repository 
builder.Services.AddSingleton<ITopicsRepository, TopicsRepository>(); //Topics repository 
builder.Services.AddSingleton<IEventsRepository, EventsRepository>(); //Events repository
builder.Services.AddHttpClient();

builder.Build().Run();

