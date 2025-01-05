using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class ServiceHandlerContext : IServiceHandlerContext
{
    private Dictionary<string, CommandEnvelope> _envelopes = new();
    private readonly IServiceProvider _services;
    private readonly IEvaluationAgent _evaluationAgent;
    private readonly DomainContext _domainContext;

    private readonly Activity? _activity;
    
    public ServiceHandlerContext(IServiceProvider services, ServiceMetadata metadata)
    {
        _services = services;
        _evaluationAgent = services.GetRequiredService<IEvaluationAgent>();
        _domainContext = services.GetRequiredService<DomainContext>();
        
        _activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: $"Evaluate {metadata.ServiceType?.Name}",
            parentContext: metadata.ParentContext ?? default,
            tags: new Dictionary<string, object?>
            {
                
            });
        
        ParentContext = _activity?.Context;
    }

    public string? TransactionId { get; init; }
    public ActivityContext? ParentContext { get; init; }
    public Result Result { get; init; } = new();
    public IReadOnlyDictionary<string, object> Attributes { get; init; } 
        = new Dictionary<string, object>();
    
    public IReadOnlyCollection<CommandEnvelope> Commands 
        => _envelopes.Values.ToList().AsReadOnly();

    public IAggregateHandlerFactory Factory 
        => _services.GetRequiredService<IAggregateHandlerFactory>();

    /// <summary>
    /// Evaluates a service and when successfull, adds the resulting operations to the current commands basket.
    /// </summary>
    /// <param name="service">The service to evaluate</param>
    /// <returns>An <c>IResultBase</c> signalling evaluation state</returns>
    public virtual async Task InvokeService<TService>(TService service)
        where TService : class, IService
    {
        var result = await _evaluationAgent.Evaluate(
            new ServiceEnvelope(
                new ServiceMetadata
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    ServiceType = service.GetType(),
                    Attributes = new Dictionary<string, object>(Attributes),
                    ParentContext = ParentContext,
                    TransactionId = TransactionId
                }, service));

        Result.WithReasons(result.Reasons);
        
        if (!result.IsSuccess) return;
        
        foreach (var envelope in result.Value)
        {
            StageCommands(
                envelope.Metadata.AggregateId, 
                envelope.Messages.ToArray());
        }
    }

    /// <summary>
    /// Adds a command to the commands basket for future evaluation.
    /// </summary>
    /// <param name="command">The command to add to the current commands basket</param>
    public virtual void StageCommands(string aggregateId, params ICommand[] commands)
    {
        foreach (var command in commands) _activity?.AddEvent(new ActivityEvent($"Stage {command.GetType().Name}"));
        
        var aggregateType = _domainContext.GetCommonAggregateType(commands);
        
        if (!_envelopes.TryGetValue(aggregateId, out var envelope))
        {
            envelope = new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = aggregateId,
                    AggregateType = aggregateType,
                    Attributes = new Dictionary<string, object>(Attributes),
                    ParentContext = ParentContext,
                    CreatedAt = DateTimeOffset.UtcNow,
                    TransactionId = TransactionId
                });
        }

        if (envelope.Metadata.AggregateType != aggregateType)
            throw new Exception("Aggregate types do not match");
        
        envelope = envelope with
        {
            Messages = [..envelope.Messages, ..commands]
        };

        _envelopes.Remove(aggregateId);
        _envelopes.Add(aggregateId, envelope);
    }

    public void Dispose()
    {
        _evaluationAgent.Dispose();
        _activity?.Dispose();
    }
}
