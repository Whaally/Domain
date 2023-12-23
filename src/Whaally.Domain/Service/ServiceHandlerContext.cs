using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Command;

namespace Whaally.Domain.Service;

public class ServiceHandlerContext : IServiceHandlerContext
{
    readonly IServiceProvider _services;
    private List<ICommandEnvelope> _commands = new List<ICommandEnvelope>(0);

    /// <summary>
    /// Access to the commands which have previously been issued. Includes the commands
    /// issued by spawned sub-services as well.
    /// </summary>
    public IReadOnlyCollection<ICommandEnvelope> Commands => _commands.AsReadOnly();

    /// <summary>
    /// Allow retrieving <seealso cref="IAggregateHandler">IAggregateHandler</c> instances directly through the <c cref="IAggregateHandlerFactory">IAggregateHandlerFactory</c>.
    /// </summary>
    /// <remarks>
    /// Note that commands directly issued to the <c>IAggregateHandler</c> are evaluated directly and as such do
    /// not benefit from the compositional system services use.
    /// </remarks>
    public IAggregateHandlerFactory Factory => _services.GetRequiredService<IAggregateHandlerFactory>();

    public ActivityContext Activity { get; init; }

    public ServiceHandlerContext(IServiceProvider services)
    {
            _services = services;
        }

    /// <summary>
    /// Evaluates a service and when successfull, adds the resulting operations to the current commands basket.
    /// </summary>
    /// <param name="service">The service to evaluate</param>
    /// <returns>An <c>IResultBase</c> signalling evaluation state</returns>
    public async Task<IResultBase> EvaluateService<TService>(TService service)
        where TService : class, IService
    {
            var evaluationAgent = _services.GetRequiredService<IEvaluationAgent>();

            var result = await evaluationAgent.EvaluateService(
                new ServiceEnvelope<TService>(
                    service,
                    new ServiceMetadata
                    {
                        SourceActivity = Activity
                    }));

            if (result.IsSuccess)
                _commands.AddRange(result.Value);

            return result.ToResult();
        }

    /// <summary>
    /// Adds a command to the commands basket for future evaluation.
    /// </summary>
    /// <param name="command">The command to add to the current commands basket</param>
    public void StageCommand<TCommand>(string aggregateId, TCommand command)
        where TCommand : class, ICommand
        => _commands.Add(new CommandEnvelope(
            command,
            new CommandMetadata
            {
                AggregateId = aggregateId,
                Timestamp = DateTime.UtcNow,
                SourceActivity = Activity
            }));
}