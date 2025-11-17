using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.interfaces;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    IWeatherForecastService service,
    ActivitySource activitySource) : ControllerBase
{
    private readonly ActivitySource _activitySource = activitySource;
    private readonly IWeatherForecastService _service = service;
    [HttpGet]
    public IEnumerable<WeatherForecastModel> Get()
    {
        using var activity = _activitySource.StartActivity("controller-span");

        return _service.List();
    }
}