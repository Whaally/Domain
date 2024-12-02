using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

/// <summary>
///     Object holding information about service handler types.
///
///     Information includes the type of the service handler, and type of the service object itself.
/// </summary>
/// <param name="HandlerType"></param>
public record ServiceHandlerMeta(Type HandlerType)
{
    public Type? ServiceType { get; init; }
    
    public static ServiceHandlerMeta From(Type type)
    {
        // ReSharper disable once SimplifyLinqExpressionUseAll
        if (!type.IsClass
            || !type.GetInterfaces().Any(q => q == typeof(IServiceHandler)))
            throw new ArgumentException($"Expected {nameof(type)} to be a class and implement {nameof(IServiceHandler)}");
        
        var @interface = type
            .GetInterfaces()
            .SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IServiceHandler<>));
        
        if (@interface == null)
            return new ServiceHandlerMeta(type);
        
        var genericArguments = @interface.GetGenericArguments();
        
        return new ServiceHandlerMeta(type)
        {
            ServiceType = genericArguments[0]
        };
    }
    
    internal static ServiceHandlerMeta From<TServiceHandler>()
        where TServiceHandler : IServiceHandler
        => From(typeof(TServiceHandler));
}
