using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

/// <summary>
///     Object holding type information about a saga.
///
///     Information includes the saga type itself, as well as the event type it responds to.
/// </summary>
/// <param name="HandlerType"></param>
public record SagaMeta(Type HandlerType)
{
    public Type? EventType { get; init; }

    public static SagaMeta From(Type type)
    {
        // ReSharper disable once SimplifyLinqExpressionUseAll
        if (!type.IsClass
            || !type.GetInterfaces().Any(q => q == typeof(ISaga)))
            throw new ArgumentException($"Expected {nameof(type)} to be a class and implement {nameof(ISaga)}");
        
        var @interface = type
            .GetInterfaces()
            .SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(ISaga<>));

        if (@interface == null)
            return new SagaMeta(type);

        var genericArguments = @interface.GetGenericArguments();
        
        return new SagaMeta(type)
        {
            EventType = genericArguments[0]
        };
    }

    internal static SagaMeta From<TSaga>()
        where TSaga : ISaga
        => From(typeof(TSaga));
}
