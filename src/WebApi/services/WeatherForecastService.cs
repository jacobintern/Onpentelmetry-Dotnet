using System.Diagnostics;
using WebApi.interfaces;
using WebApi.Models;

namespace WebApi.services;

public class WeatherForecastService(
    ActivitySource activitySource,
    IWeatherForecastRepository repository) : IWeatherForecastService
{
    private readonly ActivitySource _activitySource = activitySource;
    private readonly IWeatherForecastRepository _repository = repository;

    public IEnumerable<WeatherForecastModel> List()
    {
        using var activity = _activitySource.StartActivity("service-span");

        return _repository.List();
    }
}