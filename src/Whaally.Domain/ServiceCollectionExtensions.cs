using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
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

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services, string assembly = "")
    {
        if (!string.IsNullOrWhiteSpace(assembly))
        {
            Assembly.Load(assembly);
        }
            
        return services
            .AddSingleton<Domain>()
            .AddSingleton<IAggregateHandlerFactory, DefaultAggregateHandlerFactory>()
            .AddTransient<IServiceHandlerContext, ServiceHandlerContext>()
            .AddTransient<IEvaluationAgent, DefaultEvaluationAgent>()
            .AddTransient<ISagaContext, SagaContext>()
            .LoadCommandHandlers()
            .LoadEventHandlers()
            .LoadServiceHandlers()
            .LoadSagas()
            .LoadSnapshots();
    }

    private static IEnumerable<Assembly> GetAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            yield return assembly;
        }
    }

    private static IServiceCollection LoadCommandHandlers(this IServiceCollection services)
    {
        var handlers = GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                        && !q.IsGenericType
                        && q.GetInterfaces()
                            .Any(w => w.IsGenericType
                                      && w.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

        foreach (var handler in handlers)
        {
            var genericArguments = handler
                .GetInterfaces()
                .SingleOrDefault(q => q.IsGenericType
                                      && q.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                ?.GetGenericArguments();

            if (genericArguments == null) continue;

            // At this point we're using the DI container as a metadata store.
            // Perhaps something we should refactor out at a later point to
            // improve performance.
            services.AddTransient(typeof(ICommandHandler), handler);

            services.AddTransient(
                typeof(ICommandHandler<,>)
                    .MakeGenericType(
                        genericArguments[0],
                        genericArguments[1]),
                handler);
        }

        return services;
    }

    private static IServiceCollection LoadEventHandlers(this IServiceCollection services)
    {
        var handlers = GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                        && !q.IsGenericType
                        && q.GetInterfaces()
                            .Any(w => w.IsGenericType
                                      && w.GetGenericTypeDefinition() == typeof(IEventHandler<,>)));

        foreach (var handler in handlers)
        {
            var genericArguments = handler
                .GetInterfaces()
                .SingleOrDefault(q => q.IsGenericType
                                      && q.GetGenericTypeDefinition() == typeof(IEventHandler<,>))
                ?.GetGenericArguments();

            if (genericArguments == null) continue;

            services.AddTransient(typeof(IEventHandler), handler);

            services.AddTransient(
                typeof(IEventHandler<,>)
                    .MakeGenericType(
                        genericArguments[0],
                        genericArguments[1]),
                handler);
        }

        return services;
    }

    private static IServiceCollection LoadServiceHandlers(this IServiceCollection services)
    {
        var handlers = GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                        && !q.IsGenericType
                        && q.GetInterfaces()
                            .Any(w => w.IsGenericType
                                      && w.GetGenericTypeDefinition() == typeof(IServiceHandler<>)));

        foreach (var handler in handlers)
        {
            var genericArguments = handler
                .GetInterfaces()
                .SingleOrDefault(q => q.IsGenericType
                                      && q.GetGenericTypeDefinition() == typeof(IServiceHandler<>))
                ?.GetGenericArguments();

            if (genericArguments == null) continue;

            services.AddTransient(
                typeof(IServiceHandler<>).MakeGenericType(genericArguments[0]),
                handler);
        }

        return services;
    }

    private static IServiceCollection LoadSagas(this IServiceCollection services)
    {
        var handlers = GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                        && !q.IsGenericType
                        && q.GetInterfaces()
                            .Any(w => w.IsGenericType
                                      && w.GetGenericTypeDefinition() == typeof(ISaga<>)));

        foreach (var handler in handlers)
        {
            var genericArguments = handler
                .GetInterfaces()
                .SingleOrDefault(q => q.IsGenericType
                                      && q.GetGenericTypeDefinition() == typeof(ISaga<>))
                ?.GetGenericArguments();

            if (genericArguments == null) continue;

            services.AddTransient(
                typeof(ISaga<>).MakeGenericType(genericArguments[0]),
                handler);
        }

        return services;
    }

    private static IServiceCollection LoadSnapshots(this IServiceCollection services)
    {
        var factories = GetAssemblies()
            .SelectMany(q => q.GetTypes())
            .Where(q => q.IsClass
                        && !q.IsGenericType
                        && q.GetInterfaces()
                            .Any(w => w.IsGenericType
                                      && w.GetGenericTypeDefinition() == typeof(ISnapshotFactory<,>)));

        foreach (var factory in factories)
        {
            var genericArguments = factory
                .GetInterfaces()
                .SingleOrDefault(q => q.IsGenericType
                                      && q.GetGenericTypeDefinition() == typeof(ISnapshotFactory<,>))
                ?.GetGenericArguments();

            if (genericArguments == null) continue;

            services.AddTransient(
                typeof(ISnapshotFactory<,>).MakeGenericType(
                    genericArguments[0],
                    genericArguments[1]),
                factory);
        }

        return services;
    }
}