using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LunchTimeMCP; // Namespace where RestaurantService + models live

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Register Azure Table Storage client
// This reuses the same storage account that Azure Functions uses (AzureWebJobsStorage)
builder.Services.AddSingleton(sp =>
{
    var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
        ?? throw new InvalidOperationException("AzureWebJobsStorage connection string not found");
    return new TableServiceClient(connectionString);
});

// Register domain services
builder.Services.AddSingleton<RestaurantService>();

// Configure MCP tool metadata
builder.ConfigureMcpTool("get_restaurants");
builder.ConfigureMcpTool("add_restaurant");
builder.ConfigureMcpTool("pick_random_restaurant");

builder.Build().Run();