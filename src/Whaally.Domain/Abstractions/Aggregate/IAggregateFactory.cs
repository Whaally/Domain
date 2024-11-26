namespace Whaally.Domain.Abstractions.Aggregate;

/// <summary>
///     The aggregate factory is responsible for providing new instances of a given aggregate type T
/// </summary>
public interface IAggregateFactory
{
    T Instantiate<T>()
        where T : class;
    
    // object Instantiate(Type t);
}