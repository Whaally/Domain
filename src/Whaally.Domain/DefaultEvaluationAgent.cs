using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultEvaluationAgent : IEvaluationAgent
{
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;
    private readonly IContextFactory _contextFactory;
    private readonly IAggregateHandlerFactory _handlerFactory;
    
    private readonly Activity? _activity;
    
    public DefaultEvaluationAgent(IServiceProvider services)
    {
        _services = services;
        _domainContext = _services.GetRequiredService<DomainContext>();
        _contextFactory = _services.GetRequiredService<IContextFactory>();
        _handlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();
        
        _activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: "DomainContext",
            tags: new Dictionary<string, object?> { });
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceEnvelope"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public async Task<IResult<ICommandEnvelope[]>> Evaluate(IServiceEnvelope serviceEnvelope)
    {
        if (serviceEnvelope.Messages.Count() != 1)
            throw new ArgumentException($"Expected {nameof(serviceEnvelope)} to contain one message");
        
        serviceEnvelope.Metadata.ParentContext = _activity?.Context;
        
        var serviceContext = _contextFactory.CreateServiceHandlerContext(serviceEnvelope.Metadata);
        
        var result = await _domainContext
            .GetServiceHandler(serviceEnvelope.Messages.Single().GetType())
            .Handle(
                serviceContext,
                serviceEnvelope.Messages.Single());
        
        return new Result<ICommandEnvelope[]>()
            .WithValue(serviceContext.Commands.ToArray())
            .WithReasons(result.Reasons);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commandEnvelopes)
    {
        List<IResult<IEventEnvelope>> results = [];
        
        foreach (var envelope in commandEnvelopes)
        {
            if (!envelope.Messages.Any()) continue;
            
            envelope.Metadata.ParentContext = _activity?.Context;
            
            var handler = _handlerFactory.Instantiate(
                GetCommonAggregateType(envelope),
                envelope.Metadata.AggregateId);
            
            results.Add(await handler.Evaluate(envelope));
        }

        return new Result<IEventEnvelope[]>()
            .WithValue(results
                .Select(q => q.Value)
                .ToArray())
            .WithReasons(results.SelectMany(q => q.Reasons));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResultBase> Apply(params IEventEnvelope[] eventEnvelopes)
    {
        List<IResultBase> results = new(eventEnvelopes.Length);
        
        foreach (var envelope in eventEnvelopes)
        {
            if (!envelope.Messages.Any()) continue;

            envelope.Metadata.ParentContext = _activity?.Context;
            
            var handler = _handlerFactory.Instantiate(
                GetCommonAggregateType(envelope),
                envelope.Metadata.AggregateId);
            
            results.Add(await handler.Apply(envelope));
        }
        
        return new Result()
            .WithReasons(results.SelectMany(q => q.Reasons));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelope"></param>
    /// <returns></returns>
    public Task<IResultBase> Continue(IEventEnvelope eventEnvelope)
    {
        eventEnvelope.Metadata.ParentContext = _activity?.Context;
        
        foreach (var @event in eventEnvelope.Messages)
        {
            foreach (var saga in _domainContext.GetSaga(@event.GetType()))
            {
                _ = Task.Run(() => Invoke(saga, eventEnvelope));
            }
        }
        
        return Task.FromResult<IResultBase>(Result.Ok());
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="saga"></param>
    /// <param name="eventEnvelope"></param>
    /// <returns></returns>
    public async Task<IResultBase> Invoke(ISaga saga, IEventEnvelope eventEnvelope)
    {
        if (eventEnvelope.Messages.Count() != 1)
            throw new ArgumentException($"Expected {nameof(eventEnvelope)} to contain one message");

        eventEnvelope.Metadata.ParentContext = _activity?.Context;
        
        return await saga.Evaluate(
            _contextFactory.CreateSagaContext(eventEnvelope.Metadata), 
            eventEnvelope.Messages.Single());
    }
    
    public void Dispose()
    {
        _activity?.Dispose();
    }
    
    private Type GetCommonAggregateType(ICommandEnvelope commandEnvelope)
    {
        var commandTypes = commandEnvelope.Messages
            .Select(q => q.GetType())
            .ToList();
        
        var aggregateType = _domainContext.CommandHandlers
            .Where(q => q.CommandType != null
                        && commandTypes.Contains(q.CommandType))
            .Select(q => q.AggregateType)
            .Distinct()
            .SingleOrDefault();
        
        if (aggregateType == null)
            throw new Exception("Single envelope contains commands registered with different aggregate types");
        
        return aggregateType;
    }
    
    private Type GetCommonAggregateType(IEventEnvelope eventEnvelope)
    {
        var eventTypes = eventEnvelope.Messages
            .Select(q => q.GetType())
            .ToList();
        
        var aggregateType = _domainContext.EventHandlers
            .Where(q => q.EventType != null
                        && eventTypes.Contains(q.EventType))
            .Select(q => q.AggregateType)
            .Distinct()
            .SingleOrDefault();
        
        if (aggregateType == null)
            throw new Exception("Single envelope contains events registered with different aggregate types");
        
        return aggregateType;
    }
}
