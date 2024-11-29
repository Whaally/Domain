using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Whaally.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(
        this IServiceCollection services,
        string assembly = "") => AddDomain(services, options =>
    {
        options.Assembly = assembly;
    });
    
    public static IServiceCollection AddDomain(
        this IServiceCollection services,
        Action<DomainOptions> configureOptions)
    {
        DomainOptions options = new();
        configureOptions(options);
        
        if (!string.IsNullOrWhiteSpace(options.Assembly))
            Assembly.Load(options.Assembly);

        var commandHandlers = options.CommandHandlerTypes();
        var eventHandlers = options.EventHandlerTypes();
        var serviceHandlers = options.ServiceHandlerTypes();
        var sagaHandlers = options.SagaTypes();
        var snapshotFactories = options.SnapshotFactoryTypes();

        // I wonder if I ever want to use DI with an event handler, but ahwell, who am I to say what to do?
        Type[] types = [
            ..commandHandlers,
            ..eventHandlers,
            ..serviceHandlers,
            ..sagaHandlers,
            ..snapshotFactories
        ];
        
        types
            .ToList()
            .ForEach(type => services.AddTransient(type));
        
        services
#pragma warning disable CS0618 // Type or member is obsolete
            .AddSingleton<Domain>(services => 
                new Domain(services)
                {
                    CommandHandlerTypes = commandHandlers,
                    EventHandlerTypes = eventHandlers,
                    ServiceHandlerTypes = serviceHandlers,
                    SagaTypes = sagaHandlers,
                    SnapshotFactoryTypes = snapshotFactories
                })
#pragma warning restore CS0618 // Type or member is obsolete
            .AddSingleton<DomainContext>(services => 
                new DomainContext(services)
                {
                    CommandHandlerTypes = commandHandlers,
                    EventHandlerTypes = eventHandlers,
                    ServiceHandlerTypes = serviceHandlers,
                    SagaTypes = sagaHandlers,
                    SnapshotFactoryTypes = snapshotFactories
                })
            .AddSingleton(options.AggregateHandlerFactory)
            .AddSingleton(options.AggregateFactory)
            .AddTransient(options.ServiceHandlerContext)
            .AddTransient(options.EvaluationAgent)
            .AddTransient(options.SagaContext);
        
        return services;
    }
}
