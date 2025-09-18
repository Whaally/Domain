using System.Collections.ObjectModel;
using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class SagaContext : ISagaContext
{
    private readonly IServiceProvider _services;
    
    private SagaContext() { throw new Exception($"The private parameterless constructor for type `{nameof(SagaContext)}` should not be used."); }
    public SagaContext(
        IServiceProvider services,
        EventMetadata metadata)
    {
        _services = services;
        Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes);
    }
    
    public string? AggregateId { get; init; }
    
    public string? TransactionId { get; init; }
    public ActivityContext? ParentContext { get; init; }
    public IReadOnlyDictionary<string, object> Attributes { get; init; } 
        = new Dictionary<string, object>();
    
    public IAggregateHandlerFactory Factory 
        => _services.GetRequiredService<IAggregateHandlerFactory>();
}
