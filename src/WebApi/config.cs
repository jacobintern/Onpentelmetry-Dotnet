namespace WebApi;

public class Config
{
    public required LoggingOptions Logging { get; set; }
    public required string AllowedHosts { get; set; }
    public required string ModuleName { get; set; }
}

public class LoggingOptions
{
    public required LogLevelOptions LogLevel { get; set; }
}

public class LogLevelOptions
{
    public required string Default { get; set; }
    public required string MicrosoftAspNetCore { get; set; }
    public required string OpenTelemetry { get; set; }
    public required string OpenTelemetryExporter { get; set; }
}