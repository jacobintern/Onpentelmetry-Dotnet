using System.Diagnostics;
using WebApi.interfaces;
using WebApi.Models;

namespace WebApi.repositories;

public class WeatherForecastRepository(ActivitySource activitySource) : IWeatherForecastRepository
{
    private readonly ActivitySource _activitySource = activitySource;
    private static readonly string[] Summaries =
    ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public IEnumerable<WeatherForecastModel> List()
    {
        using var activity = _activitySource.StartActivity("repository-span");

        return [.. Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })];
    }
}
