using System.Collections.ObjectModel;
using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class SagaContext : ISagaContext
{
    private readonly IServiceProvider _services;
    private readonly IEvaluationAgent _evaluationAgent;
    private readonly DomainContext _domainContext;
    
    private readonly Dictionary<string, CommandEnvelope> _envelopes = new();
    
    private SagaContext() { throw new Exception($"The private parameterless constructor for type `{nameof(SagaContext)}` should not be used."); }
    public SagaContext(
        IServiceProvider services,
        IEventMetadata metadata)
    {
        _services = services;
        _evaluationAgent = services.GetRequiredService<IEvaluationAgent>();
        _domainContext = services.GetRequiredService<DomainContext>();
        
        Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes);
    }
    
    public string? AggregateId { get; init; }
    public string? TransactionId { get; init; }
    public ActivityContext? ParentContext { get; init; }
    public IReadOnlyDictionary<string, object> Attributes { get; init; } 
        = new Dictionary<string, object>();
    
    public IReadOnlyList<ICommandEnvelope> Commands 
        => _envelopes.Values.ToList().AsReadOnly();
    
    public IAggregateHandlerFactory Factory 
        => _services.GetRequiredService<IAggregateHandlerFactory>();

    public virtual void StageCommands(string aggregateId, params ICommand[] commands)
    {
        var aggregateType = _domainContext.GetCommonAggregateType(commands);
        
        if (!_envelopes.TryGetValue(aggregateId, out var envelope))
            envelope = new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = aggregateId,
                    AggregateType = aggregateType,
                    Attributes = new Dictionary<string, object>(Attributes),
                    CreatedAt = DateTimeOffset.UtcNow,
                    ParentContext = ParentContext,
                    TransactionId = TransactionId
                });

        if (envelope.Metadata.AggregateType != aggregateType)
            throw new Exception("Aggregate types do not match");
        
        envelope = envelope with
        {
            Messages = [..envelope.Messages, ..commands]
        };

        _envelopes.Remove(aggregateId);
        _envelopes.Add(aggregateId, envelope);
    }
    
    public virtual async Task<IResultBase> EvaluateService(IService service)
    {
        var result = await _evaluationAgent.Evaluate(
            new ServiceEnvelope(
                new ServiceMetadata
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    ServiceType = service.GetType(),
                    TransactionId = TransactionId
                }, service));

        if (!result.IsSuccess) return result.ToResult();
        
        foreach (var envelope in result.Value)
            StageCommands(
                envelope.Metadata.AggregateId, 
                envelope.Messages.ToArray());

        return result.ToResult();
    }
}
