namespace Whaally.Domain.Abstractions.Generated;

public interface IOperationMetadata
{
    public string Namespace { get; }
    public string Name { get; }
    public string Description { get; }
}

public interface IServiceMetadata : IOperationMetadata
{
    IEnumerable<ISagaMetadata> CallingSagas { get; }
    IEnumerable<IServiceMetadata> CallingServices { get; }
    
    IEnumerable<IServiceMetadata> Invoked { get; }
    IEnumerable<ICommandMetadata> Staged { get; }
}

public interface ICommandMetadata : IOperationMetadata
{
    IEnumerable<IServiceMetadata> CallingServices { get; }
    IEnumerable<ICommandMetadata> CallingCommands { get; }
    
    IEnumerable<ICommandMetadata> Invoked { get; }
    IEventMetadata[] Staged { get; }
}

public interface IEventMetadata : IOperationMetadata
{
    IEnumerable<ICommandMetadata> CallingCommands { get; }
    
    IEnumerable<IEventMetadata> Invoked { get; }
    
    IEnumerable<ISagaMetadata> Triggers { get; }
}

public interface ISagaMetadata : IOperationMetadata
{
    IEventMetadata Trigger { get; }
    
    IEnumerable<IServiceMetadata> Invoked { get; }
    IEnumerable<ICommandMetadata> Staged { get; }
}

public interface IAggregateMetadata : IOperationMetadata
{
    
}