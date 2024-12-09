using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class SagaContext : ISagaContext
{
    private readonly IServiceProvider _services;
    private readonly IEvaluationAgent _evaluationAgent;
    
    private SagaContext() { throw new Exception($"The private parameterless constructor for type `{nameof(SagaContext)}` should not be used."); }
    public SagaContext(IServiceProvider services)
    {
        _services = services;
        _evaluationAgent = services.GetRequiredService<IEvaluationAgent>();
    }

    public IReadOnlyCollection<ICommandEnvelope> Commands => _commands.AsReadOnly();
    private List<ICommandEnvelope> _commands = new();
    public IAggregateHandlerFactory Factory => _services.GetRequiredService<IAggregateHandlerFactory>();

    public IDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
    public ActivityContext? ParentContext { get; init; }
    public string? AggregateId { get; init; }

    public virtual void StageCommand(string aggregateId, ICommand command)
    {
        _commands.Add(new CommandEnvelope(
            command,
            new CommandMetadata
            {
                CreatedAt = DateTimeOffset.UtcNow,
                AggregateId = aggregateId
            }));
    }

    public virtual async Task<IResultBase> EvaluateService(IService service)
    {
        var result = await _evaluationAgent.Evaluate(
            new ServiceEnvelope<IService>(
                service,
                new ServiceMetadata
                {
                    CreatedAt = DateTimeOffset.UtcNow
                }));

        if (result.IsSuccess)
            _commands.AddRange(result.Value);

        return result.ToResult();
    }
}
