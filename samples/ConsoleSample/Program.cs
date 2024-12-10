using ConsoleSample.Domain;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Whaally.Domain;

Console.WriteLine("Hello, World!");

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder
        .CreateDefault()
        .AddService("console-sample")
        .AddAttributes(new Dictionary<string, object>
        {
            { "ip", "." }
        }))
    .AddSource("Whaally.Domain")
    .AddOtlpExporter()
    .Build();

var services = new ServiceCollection()
    .AddDomain(options =>
    {
        
    })
    .BuildServiceProvider();

var domain = services.GetRequiredService<DomainContext>();

await domain.Trigger(new AddItemService());
