using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// 服務名稱（會顯示在 Grafana / Tempo）
const string serviceName = "demo.Api";
const string serviceVersion = "1.0.0";

// 設定 OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
        .AddTelemetrySdk())
    // --- Tracing pipeline ---
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o =>
        {
            // Grafana Tempo 或 OTLP Collector
            o.Endpoint = new Uri("http://tempo:4317");
        }))
    // --- Metrics pipeline ---
    .WithMetrics(meterProviderBuilder => meterProviderBuilder
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri("http://tempo:4317");
        }));

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "Hello OpenTelemetry!");
app.Run();