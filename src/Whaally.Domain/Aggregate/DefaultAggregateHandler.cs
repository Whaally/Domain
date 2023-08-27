using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Command;
using Whaally.Domain.Event;

namespace Whaally.Domain.Aggregate
{
    public class DefaultAggregateHandler<TAggregate> : IAggregateHandler<TAggregate>
        where TAggregate : class, IAggregate, new()
    {
        private readonly IServiceProvider _services;

        private TAggregate _aggregate = new();
        public TAggregate Aggregate
        {
            get => _aggregate;
            init => _aggregate = value;
        }

        public string Id { get; init; }

        // ToDo: Use an options pattern to supply mandatory/optional parameters
        public DefaultAggregateHandler(IServiceProvider services, string id)
        {
            _services = services;
            Id = id;
        }

        public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands)
        {
            var events = new List<IEventEnvelope>(commands.Length);
            var results = new List<IResultBase>();

            TAggregate intermediateState = _aggregate;

            // ToDo: Check whether the commands have in fact been intended for the present aggregate

            foreach (var cmd in commands)
            {
                // ToDo: Extract the command handler instantiation to some other component
                /*
                 * The following things happen:
                 * 
                 * 1. The aggregate ID is set on the command (must be refactored to remove dependency)
                 * 2. Command handler is retrieved, and an appropriate command context instance is created for evaluation
                 * 3. The command is evaluated
                 * 4. Results are extracted from the context and evaluated against a temporary state of the aggregate.
                 */

                ICommandEnvelope command = cmd;

                var commandHandlerType = typeof(ICommandHandler<,>)
                    .MakeGenericType(
                        typeof(TAggregate),
                        command.Message.GetType());

                var commandHandler = (ICommandHandler)_services.GetRequiredService(commandHandlerType);
                var commandContext = new CommandHandlerContext<TAggregate>(
                    !string.IsNullOrWhiteSpace(command.Metadata.AggregateId)
                        ? command.Metadata.AggregateId
                        : Id)
                {
                    Aggregate = intermediateState
                };

                results.Add(commandHandler.Evaluate(commandContext, command.Message));

                var intermediateEvents = commandContext.Events.ToList();

                foreach (var intermediateEvent in intermediateEvents)
                {
                    IEventEnvelope @event = intermediateEvent;

                    var eventHandlerType = typeof(IEventHandler<,>)
                        .MakeGenericType(
                            typeof(TAggregate),
                            @event.Message.GetType());

                    var eventHandler = (IEventHandler)_services.GetRequiredService(eventHandlerType);

                    var eventContext = new EventHandlerContext<TAggregate>(
                        !string.IsNullOrWhiteSpace(command.Metadata.AggregateId)
                            ? command.Metadata.AggregateId
                            : Id)
                    {
                        Aggregate = intermediateState
                    };

                    intermediateState = eventHandler
                        .Apply(eventContext, @event.Message);

                    events.Add(@event);
                }
            }

            var result = Result.Ok().WithReasons(results.SelectMany(result => result.Reasons));

            return Task.FromResult<IResult<IEventEnvelope[]>>(
                result.IsSuccess
                    ? result.ToResult(events.ToArray())
                    : result);
        }

        /*
         * Note that in this method the continuation happens sequentially, and is awaited.
         * In production environments the confirm method should likely return after the changes had been applied
         * to free up the aggregate handler for other operations.
         */
        public async Task<IResultBase> Confirm(params IEventEnvelope[] events)
        {
            await Apply(events);

            var evaluationAgent = _services.GetRequiredService<IEvaluationAgent>();

            foreach (var @event in events)
            {
                await Task.Run(async () =>
                {
                    var commands = await evaluationAgent.EvaluateSaga(@event);

                    if (commands.IsFailed) return;

                    var events = await evaluationAgent.EvaluateCommands(commands.Value);

                    if (events.IsFailed) return;

                    await evaluationAgent.EvaluateEvents(events.Value);
                });
            }

            return Result.Ok();
        }

        public Task<IResultBase> Apply(params IEventEnvelope[] events)
        {
            if (events == null) return Task.FromResult<IResultBase>(Result.Ok());

            TAggregate intermediateState = _aggregate;

            foreach (var @event in events)
            {
                var eventHandlerType = typeof(IEventHandler<,>)
                    .MakeGenericType(
                        typeof(TAggregate),
                        @event.Message.GetType());

                var eventHandler = (IEventHandler)_services.GetRequiredService(eventHandlerType);

                var eventContext = new EventHandlerContext<TAggregate>(
                    !string.IsNullOrWhiteSpace(@event.Metadata.AggregateId)
                        ? @event.Metadata.AggregateId
                        : Id)
                {
                    Aggregate = intermediateState
                };

                intermediateState = eventHandler.Apply(
                    eventContext,
                    @event.Message);
            }

            _aggregate = intermediateState;

            return Task.FromResult<IResultBase>(Result.Ok());
        }

        public Task<TSnapshot> Snapshot<TSnapshot>()
            where TSnapshot : ISnapshot
            => Task.FromResult(_services.GetRequiredService<ISnapshotFactory<TAggregate, TSnapshot>>().Instantiate(_aggregate));
    }
}
