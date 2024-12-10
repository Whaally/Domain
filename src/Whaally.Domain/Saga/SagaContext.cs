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
    
    private readonly Dictionary<string, CommandEnvelope> _envelopes = new();
    
    private SagaContext() { throw new Exception($"The private parameterless constructor for type `{nameof(SagaContext)}` should not be used."); }
    public SagaContext(
        IServiceProvider services,
        IEventMetadata metadata)
    {
        _services = services;
        _evaluationAgent = services.GetRequiredService<IEvaluationAgent>();
        
        Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes);
    }
    
    public string? AggregateId { get; init; }
    public ActivityContext? ParentContext { get; init; }
    public IReadOnlyDictionary<string, object> Attributes { get; init; } 
        = new Dictionary<string, object>();
    
    public IReadOnlyList<ICommandEnvelope> Commands 
        => _envelopes.Values.ToList().AsReadOnly();
    
    public IAggregateHandlerFactory Factory 
        => _services.GetRequiredService<IAggregateHandlerFactory>();

    
    public virtual void StageCommands(string aggregateId, params ICommand[] command)
    {
        if (!_envelopes.TryGetValue(aggregateId, out var envelope))
            envelope = new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = aggregateId
                });

        envelope = envelope with
        {
            Messages = [..envelope.Messages, ..command]
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
                    CreatedAt = DateTimeOffset.UtcNow
                }, service));

        if (!result.IsSuccess) return result.ToResult();
        
        foreach (var envelope in result.Value)
            StageCommands(
                envelope.Metadata.AggregateId, 
                envelope.Messages.ToArray());

        return result.ToResult();
    }
}
