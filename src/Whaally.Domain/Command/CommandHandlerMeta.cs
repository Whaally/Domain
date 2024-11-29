using Whaally.Domain.Abstractions.Command;

namespace Whaally.Domain;

/// <summary>
///     Object holding metadata about command handlers.
/// 
///     Information includes type about the command handler itself, the type of the aggregate and the command type.
/// </summary>
/// <param name="HandlerType"></param>
public record CommandHandlerMeta(Type HandlerType)
{
    public Type? AggregateType { get; init; }
    public Type? CommandType { get; init; }
    
    public static CommandHandlerMeta From(Type type)
    {
        // ReSharper disable once SimplifyLinqExpressionUseAll
        if (!type.IsClass
            || !type.GetInterfaces().Any(q => q == typeof(ICommandHandler)))
            throw new ArgumentException($"Expected {nameof(type)} to be a class and implement {nameof(ICommandHandler)}");
        
        var @interface = type
            .GetInterfaces()
            .SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

        if (@interface == null)
            return new CommandHandlerMeta(type);

        var genericArguments = @interface.GetGenericArguments();
        
        return new CommandHandlerMeta(type)
        {
            AggregateType = genericArguments[0],
            CommandType = genericArguments[1]
        };
    }

    internal static CommandHandlerMeta From<TCommandHandler>()
        where TCommandHandler : ICommandHandler
        => From(typeof(TCommandHandler));
}
