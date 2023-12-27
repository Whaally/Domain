using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Saga;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Command;
using Whaally.Domain.Service;

namespace Whaally.Domain.Saga;

public class SagaContext : ISagaContext
{
    private readonly IServiceProvider _services;

    private SagaContext() { throw new Exception($"The private parameterless constructor for type `{nameof(SagaContext)}` should not be used."); }
    public SagaContext(IServiceProvider services)
    {
        _services = services;
    }

    public IReadOnlyCollection<ICommandEnvelope> Commands => _commands.AsReadOnly();
    private List<ICommandEnvelope> _commands = new();
    public IAggregateHandlerFactory Factory => _services.GetRequiredService<IAggregateHandlerFactory>();

    public ActivityContext Activity { get; init; }

    // ToDo: I do not know about situations in which no aggregateId would be provided,
    // though I do not have a decent way to supply the aggregate id.
    public string? AggregateId { get; init; }

    public void StageCommand(string aggregateId, ICommand command)
    {
        _commands.Add(new CommandEnvelope(
            command,
            new CommandMetadata
            {
                SourceActivity = Activity,
                Timestamp = DateTime.UtcNow,
                AggregateId = aggregateId
            }));
    }

    public async Task<IResultBase> EvaluateService(IService service)
    {
        var evaluationAgent = _services.GetRequiredService<IEvaluationAgent>();

        var result = await evaluationAgent.EvaluateService(
            new ServiceEnvelope<IService>(
                service,
                new ServiceMetadata
                {
                    SourceActivity = Activity,
                    Timestamp = DateTime.UtcNow
                }));

        if (result.IsSuccess)
            _commands.AddRange(result.Value);

        return result.ToResult();
    }
}