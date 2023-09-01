using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>()
    .AddPolicyHandler(GetPolicy());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["Rabbitmq:Host"], Convert.ToUInt16(builder.Configuration["Rabbitmq:Port"]), "/", h =>
        {
            h.Username(builder.Configuration["Rabbitmq:User"]);
            h.Password(builder.Configuration["Rabbitmq:Passw"]);
        });

        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

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