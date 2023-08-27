using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Service;

namespace Whaally.Domain
{
    public static class ServiceProviderExtensions
    {
        public static Type? GetRelatedAggregateTypeForCommand(this IServiceProvider serviceProvider, Type commandType)
        {
            Type serviceType = null!;
            Type genericTypeDefinition = null!;

            if (commandType.IsAssignableTo(typeof(IEvent)))
            {
                serviceType = typeof(IEventHandler);
                genericTypeDefinition = typeof(IEventHandler<,>);
            }
            else if (commandType.IsAssignableTo(typeof(ICommand)))
            {
                serviceType = typeof(ICommandHandler);
                genericTypeDefinition = typeof(ICommandHandler<,>);
            }
            else throw new NotImplementedException("Unknown what sort of command should be matched to an aggregate type");

            var handlers = serviceProvider.GetServices(serviceType)
                .Select(q => (
                    handler: q!.GetType(),
                    handlerInterface: q.GetType()
                        .GetInterfaces()
                        .SingleOrDefault(q =>
                            q.IsGenericType
                            && q.GetGenericTypeDefinition() == genericTypeDefinition)));

            var handler = handlers.SingleOrDefault(q => q.handlerInterface?.GenericTypeArguments.Last() == commandType);

            return handler
                .handlerInterface
                ?.GenericTypeArguments
                .First();
        }

        public static Type? GetRelatedAggregateTypeForCommand<TCommand>(this IServiceProvider serviceProvider)
            where TCommand : class, ICommand
        {
            return serviceProvider.GetRelatedAggregateTypeForCommand(typeof(TCommand));
        }

        public static ICommandHandler GetCommandHandlerForCommand(this IServiceProvider serviceProvider, Type commandType)
        {
            var aggregateType = serviceProvider.GetRelatedAggregateTypeForCommand(commandType);

            if (aggregateType == null) throw new Exception("aggregate type not found in DI container");

            var requestedCommandHandlerType = typeof(ICommandHandler<,>)
                .MakeGenericType(aggregateType, commandType);

            return (ICommandHandler)serviceProvider
                .GetRequiredService(requestedCommandHandlerType);
        }

        public static ICommandHandler GetCommandHandlerForCommand<TCommand>(this IServiceProvider serviceProvider)
            where TCommand : class, ICommand
        {
            return serviceProvider.GetCommandHandlerForCommand(typeof(TCommand));
        }

        public static IServiceHandler GetServiceHandlerForService(this IServiceProvider serviceProvider, Type serviceType)
        {
            return serviceProvider
                .GetServices<IServiceHandler>()
                .Single(q => q
                    .GetType()
                    .GetInterface(typeof(IServiceHandler<>)
                        .MakeGenericType(serviceType)
                        .Name) != null);
        }
    }
}
