using Microsoft.CodeAnalysis;

namespace Whaally.Domain.Generators;

public interface IMetadataModel { }

public class AggregateMeta : IMetadataModel
{
    public AggregateMeta(INamedTypeSymbol aggregate) 
    {
        Aggregate = new ObjectMeta(aggregate.Name, aggregate.ContainingNamespace.ToDisplayString());
        
        // public string? XmlComment => Aggregate.GetDocumentationCommentXml(CultureInfo.InvariantCulture);
    }
    
    public ObjectMeta Aggregate { get; }
    
    
    /// <summary>
    ///     Commands ran against this aggregate
    /// </summary>
    public List<ObjectMeta> Commands { get; set; } = [];
    
    /// <summary>
    ///     Events targeted towards this aggregate
    /// </summary>
    public List<ObjectMeta> Events { get; set; } = [];
}

public class CommandMeta : IMetadataModel
{
    public CommandMeta(
        INamedTypeSymbol aggregate,
        INamedTypeSymbol command,
        INamedTypeSymbol handler,
        IEnumerable<INamedTypeSymbol> commands,
        IEnumerable<INamedTypeSymbol> events)
    {
        Aggregate = new ObjectMeta(aggregate.Name, aggregate.ContainingNamespace.ToDisplayString());
        Command = new ObjectMeta(command.Name, command.ContainingNamespace.ToDisplayString());
        Handler = new ObjectMeta(handler.Name, handler.ContainingNamespace.ToDisplayString());

        InvokedCommands = commands
            .Select(q => new ObjectMeta(q.Name, q.ContainingNamespace.ToDisplayString()))
            .ToList();
        
        Events = events
            .Select(q => new ObjectMeta(q.Name, q.ContainingNamespace.ToDisplayString()))
            .ToList();
        // public string? XmlComment => Command.GetDocumentationCommentXml(CultureInfo.InvariantCulture);
    }

    public ObjectMeta Aggregate { get; }
    public ObjectMeta Command { get; }
    public ObjectMeta Handler { get; }

    /// <summary>
    ///     Sagas calling this command
    /// </summary>
    public List<ObjectMeta> CallingSagas { get; set; } = [];
    
    /// <summary>
    ///     Services invoking this command
    /// </summary>
    public List<ObjectMeta> CallingServices { get; set; } = [];

    /// <summary>
    ///     Commands invoking this command
    /// </summary>
    public List<ObjectMeta> CallingCommands { get; set; } = [];

    /// <summary>
    ///     Commands invoked through this command
    /// </summary>
    public List<ObjectMeta> InvokedCommands { get; set; } = [];
    
    /// <summary>
    ///     Events emitted through this command
    /// </summary>
    public List<ObjectMeta> Events { get; set; } = [];
}

public class EventMeta : IMetadataModel
{
    public EventMeta(
        INamedTypeSymbol aggregate,
        INamedTypeSymbol @event,
        INamedTypeSymbol handler)
    {
        Aggregate = new ObjectMeta(aggregate.Name, aggregate.ContainingNamespace.ToDisplayString());
        Event = new ObjectMeta(@event.Name, @event.ContainingNamespace.ToDisplayString());
        Handler = new ObjectMeta(handler.Name, handler.ContainingNamespace.ToDisplayString());
        
        // public string? XmlComment => Event.GetDocumentationCommentXml(CultureInfo.InvariantCulture);
    }
    
    public ObjectMeta Aggregate { get; }
    public ObjectMeta Event { get; }
    public ObjectMeta Handler { get; }
    
    
    /// <summary>
    ///     Commands emitting this event
    /// </summary>
    public List<ObjectMeta> Commands { get; set; } = [];

    /// <summary>
    ///     Events invoking this event
    /// </summary>
    public List<ObjectMeta> CallingEvents { get; set; } = [];

    /// <summary>
    ///     Events this event invokes
    /// </summary>
    public List<ObjectMeta> InvokedEvents { get; set; } = [];
    
    /// <summary>
    ///     Sagas triggered by this event
    /// </summary>
    public List<ObjectMeta> Sagas { get; set; } = [];
}

public class SagaMeta : IMetadataModel
{
    public SagaMeta(
        INamedTypeSymbol @event,
        INamedTypeSymbol handler,
        IEnumerable<INamedTypeSymbol> services,
        IEnumerable<INamedTypeSymbol> commands)
    {
        Event = new ObjectMeta(@event.Name, @event.ContainingNamespace.ToDisplayString());
        Handler = new ObjectMeta(handler.Name, handler.ContainingNamespace.ToDisplayString());

        Services = services
            .Select(q => new ObjectMeta(q.Name, q.ContainingNamespace.ToDisplayString()))
            .ToList();
        
        Commands = commands
            .Select(q => new ObjectMeta(q.Name, q.ContainingNamespace.ToDisplayString()))
            .ToList();
        
        // if (Handler.GetDocumentationCommentXml(CultureInfo.InvariantCulture) is { } xmlstr
        //     && !string.IsNullOrWhiteSpace(xmlstr))
        // {
        //     Documentation = new XmlDocument();
        //     Documentation.LoadXml(xmlstr);
        // }
    }
    
    public ObjectMeta Handler { get; }
    
    /// <summary>
    ///     Event triggering this saga
    /// </summary>
    public ObjectMeta Event { get; }

    // public XmlDocument? Documentation { get; }
    
    /// <summary>
    ///     Services this saga invokes
    /// </summary>
    public List<ObjectMeta> Services { get; set; } = [];

    /// <summary>
    ///     Commands this saga invokes
    /// </summary>
    public List<ObjectMeta> Commands { get; set; } = [];
}

public class ServiceMeta : IMetadataModel
{
    public ServiceMeta(
        INamedTypeSymbol service,
        INamedTypeSymbol handler,
        IEnumerable<INamedTypeSymbol> services,
        IEnumerable<INamedTypeSymbol> commands)
    {
        Service = new ObjectMeta(service.Name, service.ContainingNamespace.ToDisplayString());
        Handler = new ObjectMeta(handler.Name, handler.ContainingNamespace.ToDisplayString());
        
        InvokedServices = services
            .Select(q => new ObjectMeta(q.Name, q.ContainingNamespace.ToDisplayString()))
            .ToList();
        
        Commands = commands
            .Select(q => new ObjectMeta(q.Name, q.ContainingNamespace.ToDisplayString()))
            .ToList();
        
        //public string? XmlComment => Service.GetDocumentationCommentXml(CultureInfo.InvariantCulture);
    }
    
    public ObjectMeta Service { get; }
    public ObjectMeta Handler { get; }
    
    
    /// <summary>
    ///     Services invoking this services
    /// </summary>
    public List<ObjectMeta> CallingServices { get; set; } = [];

    /// <summary>
    ///     Sagas invoking this service
    /// </summary>
    public List<ObjectMeta> CallingSagas { get; set; } = [];
    
    
    /// <summary>
    ///     Services invoked by this service
    /// </summary>
    public List<ObjectMeta> InvokedServices { get; set; } = [];
    
    /// <summary>
    ///     Commands invoked through this service
    /// </summary>
    public List<ObjectMeta> Commands { get; set; } = [];
}
