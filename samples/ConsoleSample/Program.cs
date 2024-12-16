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
            // Explicitly adding this field here such that Jaeger does not apply clock skew adjustments
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

for (var i = 0; i < 10; i++) {
    await domain.Invoke(new AddItemService());
}
