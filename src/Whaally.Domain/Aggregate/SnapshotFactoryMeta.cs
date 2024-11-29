using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain;

/// <summary>
///     Object holding metadata for a given snapshot factory.
///
///     Information held includes the factory type itself, the aggregate and the related snapshot type.
/// </summary>
/// <param name="FactoryType"></param>
public record SnapshotFactoryMeta(Type FactoryType)
{
    public Type? AggregateType { get; init; }
    public Type? SnapshotType { get; init; }

    public static SnapshotFactoryMeta From(Type type)
    {
        // ReSharper disable once SimplifyLinqExpressionUseAll
        if (!type.IsClass
            || !type.GetInterfaces().Any(q => q == typeof(ISnapshotFactory)))
            throw new ArgumentException($"Expected {nameof(type)} to be a class and implement {nameof(ISnapshotFactory)}");
        
        var @interface = type
            .GetInterfaces()
            .SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(ISnapshotFactory<,>));

        if (@interface == null)
            return new SnapshotFactoryMeta(type);

        var genericArguments = @interface.GetGenericArguments();
        
        return new SnapshotFactoryMeta(type)
        {
            AggregateType = genericArguments[0],
            SnapshotType = genericArguments[1]
        };
    }

    internal static SnapshotFactoryMeta From<TSnapshotFactory>()
        where TSnapshotFactory : ISnapshotFactory
        => From(typeof(TSnapshotFactory));
}
