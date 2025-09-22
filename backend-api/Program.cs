using System.Text.Json;
using System.Text.Json.Serialization;
using backend_api.Context;
using backend_api.Interceptors;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();



//Adding the connection string sql
builder.Services.AddDbContextFactory<MasterDbContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=AppDb;User Id=sa;Password=MyStrongPassword123;Encrypt=True;TrustServerCertificate=True;"));

//Adding the camelcase json
builder.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
{
    jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    jsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddSingleton<RemoveAliasInterceptor>();

//Adding the interface of car repository
//builder.Services.AddSingleton<ICarRepository, CarRepository>();
builder.Services.AddHttpClient();


builder.Build().Run();

