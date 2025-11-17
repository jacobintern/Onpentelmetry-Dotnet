using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

// 服務名稱（會顯示在 Grafana / Tempo）
const string serviceName = "demo.Api";
var endpoint = Environment.GetEnvironmentVariable("OTLP_ENDPOINT");
Console.WriteLine($"OTLP endpoint = {endpoint}");

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
    .ConfigureResource(r => r.AddService(serviceName))
    .WithTracing(t =>
    {
        t.AddSource(serviceName);
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