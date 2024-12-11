using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public static class IMessageExtensions
{
    public static Type GetCommonAggregateType(this DomainContext domainContext, IEnumerable<ICommand> commands)
    {
        var commandTypes = commands
            .Select(q => q.GetType())
            .ToList();
        
        var aggregateType = domainContext.CommandHandlers
            .Where(q => q.CommandType != null
                        && commandTypes.Contains(q.CommandType))
            .Select(q => q.AggregateType)
            .Distinct()
            .SingleOrDefault();
        
        if (aggregateType == null)
            throw new Exception("Single envelope contains commands registered with different aggregate types");
        
        return aggregateType;
    }
    
    public static Type GetCommonAggregateType(this DomainContext domainContext, IEnumerable<IEvent> events)
    {
        var eventTypes = events
            .Select(q => q.GetType())
            .ToList();
        
        var aggregateType = domainContext.EventHandlers
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
