using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Whaally.Domain.Abstractions;

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
            .AddSingleton<DomainContext>(services => 
                new DomainContext(
                    services,
                    commandHandlerTypes: commandHandlers,
                    eventHandlerTypes: eventHandlers,
                    serviceHandlerTypes: serviceHandlers,
                    sagaTypes: sagaHandlers,
                    snapshotFactoryTypes: snapshotFactories))
            .AddSingleton(options.AggregateHandlerFactory)
            .AddSingleton(options.AggregateFactory)
            .AddTransient(options.EvaluationAgent)
            .AddSingleton(options.ContextFactory);
        
        return services;
    }
}
