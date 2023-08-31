using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionSvcHttpClient>()
    .AddPolicyHandler(GetPolicy());


builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await app.InitDb();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

});


app.Run();


static IAsyncPolicy<HttpResponseMessage> GetPolicy() =>
    HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));