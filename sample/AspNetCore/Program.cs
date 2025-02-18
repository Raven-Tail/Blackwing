using ASPNETCore.WeatherForecasts;
using Ravitor.Contracts;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddRavitor(); // Add default IMediator/ISender implementation
builder.Services.AddRavitorHandlers(); // Add generated handlers

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapGet("api/weather", (ISender sender) => sender.Send(new GetWeatherForecasts())).WithTags("WeatherForecast"); // this method will be intercepted

app.Run();
