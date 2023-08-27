using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Service;

namespace Whaally.Domain
{
    public class Domain
    {
        readonly IServiceProvider _services;

        public Domain(IServiceProvider services)
        {
            _services = services;
        }

        /*
         * Operations:
         * 
         * - GetAggregate => AggregateHandler
         * - Evaluate(Command)
         * - Evaluate(Service)
         * - Preview(Command)
         * - Preview(Service)
         */

        public Task<IAggregateHandler<TAggregate>> GetAggregate<TAggregate>(string id)
            where TAggregate : class, IAggregate, new()
            => Task.FromResult(
                _services
                    .GetRequiredService<IAggregateHandlerFactory>()
                    .Instantiate<TAggregate>(id));

        public async Task<IResult<IEventEnvelope[]>> EvaluateCommand<TCommand>(string aggregateId, TCommand command)
            where TCommand : class, ICommand
        {
            var factory = _services.GetRequiredService<IAggregateHandlerFactory>();
            var aggregateType = _services.GetRelatedAggregateTypeForCommand(command.GetType());

            if (aggregateType == null) throw new Exception($"Aggregate type could not be resolved for command {command.GetType().FullName}");

            var handler = factory.Instantiate(
                aggregateType,
                aggregateId);

            var result = await handler.Evaluate(command);

            if (result.IsFailed) return result;

            await handler.Confirm(result.Value);

            return result;
        }

        public async Task<IResult<IEventEnvelope[]>> EvaluateService<TService>(TService service)
            where TService : class, IService
        {
            var evaluationAgent = _services.GetRequiredService<IEvaluationAgent>();

            var evalResult = await evaluationAgent.EvaluateService(
                new ServiceEnvelope<TService>(
                    service,
                    new ServiceMetadata()));

            if (evalResult.IsFailed) 
                return Result.Fail<IEventEnvelope[]>(evalResult.Errors);

            var applyResult = await evaluationAgent.EvaluateCommands(evalResult.Value);

            return applyResult;
        }
    }
}
