var client = new HttpClient();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();

var app = builder.Build();
// callback URL is http://localhost:5031/webhook/order/new
app.MapPost("/webhook/order/new", (object payload, ILogger<Program> logger) =>
{
    logger.LogInformation("Waiting for Smarphone and Tablet orders: {payload}", payload);
});
app.Run();