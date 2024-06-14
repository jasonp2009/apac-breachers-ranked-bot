using ApacBreachersRanked;
using ApacBreachersRanked.Api.Middleware;
using ApacBreachersRanked.Application;
using ApacBreachersRanked.Infrastructure;
using ApacBreachersRanked.Infrastructure.SQS;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.UseSqsPublisher(builder.Configuration);
builder.Services.AddDiscordClient(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMatchQueueListenderService();

builder.Host.UseSerilog((a, cfg) =>
{
    cfg.WriteTo.Console(new CompactJsonFormatter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseMiddleware<DiscordAuthMiddleware>();

app.Run();
