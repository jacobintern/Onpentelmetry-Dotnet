namespace WebApi.interfaces;

public interface IWeatherForecastRepository
{
    IEnumerable<Models.WeatherForecastModel> List();
}