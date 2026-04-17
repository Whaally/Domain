namespace Whaally.Domain.Analyzers;

public class MetadataGenerator
{
    public static string ForAggregate(AggregateMeta aggregateMeta)
    {
        return 
            $$"""
            using Whaally.Domain.Abstractions.Generated;
                 
            namespace {{aggregateMeta.Aggregate.Namespace}};
                 
            public sealed class {{aggregateMeta.Aggregate.Name}}Metadata : IAggregateMetadata {
                private {{aggregateMeta.Aggregate.Name}}Metadata() { }
                public static readonly {{aggregateMeta.Aggregate.Name}}Metadata Instance = new();
                
                public static readonly string Namespace = "{{aggregateMeta.Aggregate.Namespace}}";
                public static readonly string Name = "{{aggregateMeta.Aggregate.Name}}";
                
                public static readonly string Description = "";
                
                public IEnumerable<ICommandMetadata> Commands { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        aggregateMeta.Commands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                ];
                    
                public IEnumerable<IEventMetadata> Events { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        aggregateMeta.Events
                            .Select(@event => $"{@event.Namespace}.{@event.Name}Metadata.Instance"))}}
                ];
            }
            """;
    }
    
    public static string ForService(ServiceMeta serviceMeta)
    {
        return 
            $$"""
              using Whaally.Domain.Abstractions.Generated;

              namespace {{serviceMeta.Service.Namespace}};

              public sealed class {{serviceMeta.Service.Name}}Metadata : IServiceMetadata {
                  private {{serviceMeta.Service.Name}}Metadata() { }
                  public static readonly {{serviceMeta.Service.Name}}Metadata Instance = new();
                  
                  public IEnumerable<ISagaMetadata> CallingSagas { get; } = [
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.CallingSagas
                              .Select(saga => $"{saga.Namespace}.{saga.Name}Metadata.Instance"))}}
                  ];
                  
                  public IEnumerable<IServiceMetadata> CallingServices { get; } = [
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.CallingServices
                              .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                  ];
                  
                  public IEnumerable<IServiceMetadata> Invoked { get; } = [
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.InvokedServices
                              .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                  ];
                  
                  public IEnumerable<ICommandMetadata> Staged { get; } = [
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.Commands
                              .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}                
                  ];
              }
              """;
    }
    
    public static string ForCommand(CommandMeta commandMeta)
    {
        return 
            $$"""
            using Whaally.Domain.Abstractions.Generated;
                 
            namespace {{commandMeta.Command.Namespace}};
                 
            public sealed class {{commandMeta.Command.Name}}Metadata : ICommandMetadata {
                private {{commandMeta.Command.Name}}Metadata() { }
                public static readonly {{commandMeta.Command.Name}}Metadata Instance = new();
                
                public static readonly string Namespace = "{{commandMeta.Command.Namespace}}";
                public static readonly string Name = "{{commandMeta.Command.Name}}";
                
                public static readonly string Description = "";

                public IEnumerable<ISagaMetadata> CallingSagas { get; } = [
                    {{string.Join(
                            ",\r\n        ", 
                            commandMeta.CallingSagas
                                .Select(saga => $"{saga.Namespace}.{saga.Name}Metadata.Instance"))}}
                ];
                
                public IEnumerable<IServiceMetadata> CallingServices { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.CallingServices
                            .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                ];
                
                public IEnumerable<ICommandMetadata> CallingCommands { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.CallingCommands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                ];
                
                public IEnumerable<ICommandMetadata> Invoked { get; } = [ 
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.InvokedCommands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                ];
                    
                public IEnumerable<IEventMetadata> Staged { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.Events
                            .Select(@event => $"{@event.Namespace}.{@event.Name}Metadata.Instance"))}}
                ];
            }
            """;
    }

    public static string ForEvent(EventMeta eventMeta)
    {
        return 
            $$"""
            using Whaally.Domain.Abstractions.Generated;
             
            namespace {{eventMeta.Event.Namespace}};
             
            public sealed class {{eventMeta.Event.Name}}Metadata : IEventMetadata {
                private {{eventMeta.Event.Name}}Metadata() { }
                public static readonly {{eventMeta.Event.Name}}Metadata Instance = new();
                
                public IEnumerable<ICommandMetadata> CallingCommands { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        eventMeta.Commands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                ];
                
                public IEnumerable<IEventMetadata> Invoked { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        eventMeta.InvokedEvents
                            .Select(@event => $"{@event.Namespace}.{@event.Name}Metadata.Instance"))}}
                ];
                
                public IEnumerable<ISagaMetadata> Triggers { get; } = [ ];
            }
            """;
    }
    
    public static string ForSaga(SagaMeta sagaMeta)
    {
        return 
            $$"""
            using Whaally.Domain.Abstractions.Generated;

            namespace {{sagaMeta.Handler.Namespace}};

            public sealed class {{sagaMeta.Handler.Name}}Metadata : ISagaMetadata {
                private {{sagaMeta.Handler.Name}}Metadata() { }
                public static readonly {{sagaMeta.Handler.Name}}Metadata Instance = new();
                
                public IEventMetadata Trigger { get; } = {{sagaMeta.Event.Namespace}}.{{sagaMeta.Event.Name}}Metadata.Instance;
                
                public IEnumerable<IServiceMetadata> Invoked { get; } = [
                    {{string.Join(
                        ",\r\n        ", 
                        sagaMeta.Services
                            .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                ];
                
                public IEnumerable<ICommandMetadata> Staged { get; } = [
                    {{string.Join(
                            ",\r\n        ", 
                            sagaMeta.Commands
                                .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                ];
            }
            """;
    }
}
