using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Saga;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Aggregate;
using Whaally.Domain.Saga;
using Whaally.Domain.Service;

namespace Whaally.Domain;

public class DomainOptions
{
    /// <summary>
    ///     An assembly to load to be able to discover relevant domain types.
    /// </summary>
    public string Assembly { get; set; } = "";

    /// <summary>
    ///     Supplies an aggregate factory, able to instantiate aggregates of the requested type.
    ///
    ///     Has a singleton lifetime.
    /// </summary>
    public Func<IServiceProvider, IAggregateFactory> AggregateFactory 
        = _ => new DefaultAggregateFactory();

    /// <summary>
    ///     Supplies an aggregate handler factory.
    ///
    ///     Has a singleton lifetime.
    /// </summary>
    public Func<IServiceProvider, IAggregateHandlerFactory> AggregateHandlerFactory 
        = services => new DefaultAggregateHandlerFactory(
            services, 
            services.GetRequiredService<IAggregateFactory>());

    /// <summary>
    ///     Instantiates a service handler context, used when evaluating a service handler.
    ///
    ///     Has a transient lifetime.
    /// </summary>
    public Func<IServiceProvider, IServiceHandlerContext> ServiceHandlerContext
        = services => new ServiceHandlerContext(services);

    /// <summary>
    ///     Instantiates an evaluate agent, coordinating operations across nodes.
    ///
    ///     Has a transient lifetime.
    /// </summary>
    public Func<IServiceProvider, IEvaluationAgent> EvaluationAgent
        = services => new DefaultEvaluationAgent(services);

    /// <summary>
    ///     Instantiates a saga context, used when evaluating sagas.
    ///
    ///     Has a transient lifetime.
    /// </summary>
    public Func<IServiceProvider, ISagaContext> SagaContext
        = services => new SagaContext(services);

    /// <summary>
    ///     Supplies command handlers used in this domain.
    ///
    ///     Left empty, the domain will attempt to discover relevant handlers through reflection.
    /// </summary>
    public Func<IEnumerable<Type>> CommandHandlerTypes
        = () => GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                && !q.IsGenericType
                && q.GetInterfaces()
                    .Any(w => w.IsGenericType
                              && w.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

    /// <summary>
    ///     Supplies event handlers used in this domain.
    ///
    ///     Left empty, the domain will attempt to discover relevant handlers through reflection.
    /// </summary>
    public Func<IEnumerable<Type>> EventHandlerTypes
        = () => GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                && !q.IsGenericType
                && q.GetInterfaces()
                    .Any(w => w.IsGenericType
                              && w.GetGenericTypeDefinition() == typeof(IEventHandler<,>)));

    /// <summary>
    ///     Supplies service handlers in this domain.
    ///
    ///     Left empty, the domain will attempt to discover relevant handlers through reflection.
    /// </summary>
    public Func<IEnumerable<Type>> ServiceHandlerTypes
        = () => GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                && !q.IsGenericType
                && q.GetInterfaces()
                    .Any(w => w.IsGenericType
                              && w.GetGenericTypeDefinition() == typeof(IServiceHandler<>)));

    /// <summary>
    ///     Sagas used in this domain.
    ///
    ///     Left empty, the domain will attempt to discover relevant sagas through reflection.
    /// </summary>
    public Func<IEnumerable<Type>> SagaTypes
        = () => GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                && !q.IsGenericType
                && q.GetInterfaces()
                    .Any(w => w.IsGenericType
                              && w.GetGenericTypeDefinition() == typeof(ISaga<>)));

    /// <summary>
    ///     Snapshots used through this domain.
    ///
    ///     Left empty, the domain will attempt to discover relevant sagas through reflection.
    /// </summary>
    public Func<IEnumerable<Type>> SnapshotFactoryTypes
        = () => GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                && !q.IsGenericType
                && q.GetInterfaces()
                    .Any(w => w.IsGenericType
                              && w.GetGenericTypeDefinition() == typeof(ISnapshotFactory<,>)));
    
    private static IEnumerable<Assembly> GetAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            yield return assembly;
        }
    }
}
