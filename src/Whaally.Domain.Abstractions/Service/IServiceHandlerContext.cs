using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface IServiceHandlerContext : IContext, IDisposable
{
    /// <summary>
    ///     The optimistic result of the evaluation of this service.
    /// </summary>
    public IReadOnlyCollection<CommandEnvelope> Commands { get; }
    
    /// <summary>
    ///     The failure or success reasons for this services invocation.
    /// </summary>
    public IResultBase Result { get; }
    
    /// <summary>
    ///     Set the result for the evaluation of this service. 
    /// </summary>
    /// <param name="result"></param>
    public void WithResult(IResultBase result);
    
    /// <summary>
    ///     Stages a command as the optimistic result of this service.
    /// </summary>
    /// <param name="command">The command staged as a result of service evaluation</param>
    public void StageCommands(string aggregateId, params ICommand[] command);
    
    /// <summary>
    ///     Evaluates a service and stages the resulting commands as the optimistic result of this service.
    /// </summary>
    /// <param name="service">The service to be evaluated as part of service evaluation</param>
    /// <returns>A Result object indicating success or error states.</returns>
    public Task InvokeService<TService>(TService service)
        where TService : class, IService;
    
    /// <summary>
    ///     The AggregateHandlerFactory provides access to AggregateHandler instances.
    /// </summary>
    public IAggregateHandlerFactory Factory { get; }
}
