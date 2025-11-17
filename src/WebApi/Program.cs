using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using System.Diagnostics;
using WebApi;
using WebApi.interfaces;
using WebApi.repositories;
using WebApi.services;

var builder = WebApplication.CreateBuilder(args);

// 綁定設定檔
builder.Services.Configure<Config>(builder.Configuration);
var moduleName = builder.Configuration.GetValue<string>("ModuleName")
    ?? throw new Exception("ModuleName 未設定");

// 服務名稱（會顯示在 Grafana / Tempo
var endpoint = Environment.GetEnvironmentVariable("OTLP_ENDPOINT")
    ?? throw new Exception("OTLP_ENDPOINT 未設定");

// 設定 OpenTelemetry 日誌
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.ParseStateValues = true;
    logging.IncludeFormattedMessage = true;
    logging.AddOtlpExporter(exporter =>
    {
        exporter.Endpoint = new Uri(endpoint);
        exporter.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
    });
});

// 設定 OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(moduleName))
    .WithTracing(t =>
    {
        t.AddSource(moduleName);
        t.AddHttpClientInstrumentation();
        t.AddAspNetCoreInstrumentation();
        t.AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(endpoint);
            o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
        // Debug 用
        // t.AddConsoleExporter();
    })
    .WithMetrics(m =>
    {
        m.AddRuntimeInstrumentation();
        m.AddAspNetCoreInstrumentation();
        m.AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(endpoint);
            o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    });

// 註冊 ActivitySource
builder.Services.AddSingleton(_ =>
{
    var config = builder.Configuration.Get<Config>();
    return new ActivitySource(moduleName);
});

// 註冊 Repository
builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

// 註冊 Service
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

builder.Services.AddControllers();

var app = builder.Build();

var otel = app.Services.GetRequiredService<TracerProvider>();
if (otel == null)
{
    throw new Exception("OpenTelemetry 初始化失敗");
}
else
{
    Console.WriteLine("OpenTelemetry 初始化成功");
}

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "Hello OpenTelemetry!");
app.Run();