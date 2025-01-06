using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ICommandHandlerContext : IContext, IProvideAggregateInstance
{
    /// <summary>
    ///     The optimistic result of this commands evaluation
    /// 
    ///     Used for further evaluation in case of successful command evaluation
    /// </summary>
    public IReadOnlyCollection<IEvent> Events { get; }
    
    /// <summary>
    ///     The aggregate result of this evaluation
    ///
    ///     Determines the success or failure of the evaluation of this command
    /// </summary>
    public IResultBase Result { get; }
 
    /// <summary>
    ///     Set the result for the evaluation of this command
    /// </summary>
    /// <param name="result"></param>
    public void WithResult(IResultBase result);
    
    /// <summary>
    ///     Stages an event as the optimistic result of this command.
    /// </summary>
    /// <param name="event">The event staged as a result of command evaluation</param>
    public void StageEvent(IEvent @event);

    /// <summary>
    ///     Immediately invokes the provided command in the context of the current commands' execution.
    /// </summary>
    /// <param name="command"></param>
    public void EvaluateCommand(ICommand command);

    /// <summary>
    ///     Immediately invokes the provided commands in the context of the current commands' execution
    /// </summary>
    /// <param name="commands"></param>
    public void EvaluateCommands(params ICommand[] commands)
        => commands.ToList().ForEach(EvaluateCommand);
}

public interface ICommandHandlerContext<TAggregate>
    : ICommandHandlerContext, IProvideAggregateInstance<TAggregate>
    where TAggregate : class, IAggregate
{
    // This method is merely a workaround to deal with IEvent objects.
    // Later on they cannot be dealt with.
    public void StageEvent(Type eventType, IEvent @event) =>
        GetType()
            .GetMethod(nameof(StageEvent))!
            .MakeGenericMethod(eventType)
            .Invoke(this, [@event]);

    void ICommandHandlerContext.StageEvent(IEvent @event) =>
        GetType()
            .GetMethod(nameof(StageEvent))!
            .MakeGenericMethod(@event.GetType())
            .Invoke(this, [@event]);

    void ICommandHandlerContext.EvaluateCommand(ICommand command) =>
        GetType()
            .GetMethod(nameof(EvaluateCommand))!
            .MakeGenericMethod(command.GetType())
            .Invoke(this, [command]);
    
    public void StageEvent<TEvent>(TEvent @event)
        where TEvent : class, IEvent;

    public void EvaluateCommand<TCommand>(TCommand command)
        where TCommand : class, ICommand;
}
