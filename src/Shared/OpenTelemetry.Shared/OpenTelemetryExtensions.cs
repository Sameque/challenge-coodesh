using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Shared;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddSharedOpenTelemetry(this WebApplicationBuilder builder, string? serviceName = null)
    {
        var otelEndpoint = builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317";
        serviceName ??= builder.Configuration["OpenTelemetry:ServiceName"] ?? "unknown";

        var resource = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(serviceName: serviceName))
            .WithTracing(tracing => tracing
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(opts =>
                {
                    opts.Endpoint = new Uri(otelEndpoint);
                    opts.Protocol = OtlpExportProtocol.Grpc;
                }))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter());

        return builder;
    }
}