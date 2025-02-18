using ASPNETCore.WeatherForecasts;
using Microsoft.AspNetCore.Mvc;
using Blackwing.Contracts;

namespace ASPNETCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public ValueTask<IEnumerable<WeatherForecast>> Get()
    {
        // this method will be intercepted
        return _mediator.Send(new GetWeatherForecasts());
    }
}
