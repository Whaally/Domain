using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

/// <summary>
///     Object holding type information from event handlers.
///
///     Information includes type of the event handler, type of the aggregate it interacts with, and the event type itself.
/// </summary>
/// <param name="HandlerType"></param>
public record EventHandlerMeta(Type HandlerType)
{
    public Type? AggregateType { get; init; }
    public Type? EventType { get; init; }

    public static EventHandlerMeta From(Type type)
    {
        // ReSharper disable once SimplifyLinqExpressionUseAll
        if (!type.IsClass
            || !type.GetInterfaces().Any(q => q == typeof(IEventHandler)))
            throw new ArgumentException($"Expected {nameof(type)} to be a class and implement {nameof(IEventHandler)}");
        
        var @interface = type
            .GetInterfaces()
            .SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IEventHandler<,>));

        if (@interface == null)
            return new EventHandlerMeta(type);

        var genericArguments = @interface.GetGenericArguments();
        
        return new EventHandlerMeta(type)
        {
            AggregateType = genericArguments[0],
            EventType = genericArguments[1]
        };
    }

    internal static EventHandlerMeta From<TEventHandler>()
        where TEventHandler : IEventHandler
        => From(typeof(TEventHandler));
}
