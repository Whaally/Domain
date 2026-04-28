using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class ServiceHandlerContext : IServiceHandlerContext
{
    private readonly IServiceProvider _services;

    private readonly Activity? _activity;
    
    public ServiceHandlerContext(IServiceProvider services, ServiceMetadata metadata)
    {
        _services = services;
        
        _activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: $"Evaluate {metadata.ServiceType?.Name}",
            parentContext: metadata.ParentContext ?? default,
            tags: new Dictionary<string, object?>());
        
        ParentContext = _activity?.Context;
    }

    public string? TransactionId { get; init; }
    public ActivityContext? ParentContext { get; init; }
    
    public IReadOnlyDictionary<string, object> Attributes { get; init; } 
        = new Dictionary<string, object>();
    
    public IAggregateHandlerFactory Factory 
        => _services.GetRequiredService<IAggregateHandlerFactory>();

    public void Dispose()
    {
        _activity?.Dispose();
    }
}
