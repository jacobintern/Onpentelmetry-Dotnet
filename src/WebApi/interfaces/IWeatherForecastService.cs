namespace WebApi.interfaces;

public interface IWeatherForecastService
{
    IEnumerable<Models.WeatherForecastModel> List();
}