namespace Whaally.Domain.Generators;

public class MetadataGenerator
{
    public static string ForAggregate(AggregateMeta aggregateMeta)
    {
        return 
            $$"""
            #nullable enable
            
            using Whaally.Domain.Generators;
                 
            namespace {{aggregateMeta.Aggregate.Namespace}};
                 
            public sealed class {{aggregateMeta.Aggregate.Name}}Metadata : IAggregateMetadata {
                static {{aggregateMeta.Aggregate.Name}}Metadata() { }
                private {{aggregateMeta.Aggregate.Name}}Metadata() { }
                
                private static readonly {{aggregateMeta.Aggregate.Name}}Metadata instance = new();
                public static {{aggregateMeta.Aggregate.Name}}Metadata Instance => instance;
                
                public string Namespace => "{{aggregateMeta.Aggregate.Namespace}}";
                public string Name => "{{aggregateMeta.Aggregate.Name}}";
                
                public string Description => "";
                
                public IEnumerable<ICommandMetadata> Commands => new ICommandMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        aggregateMeta.Commands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<IEventMetadata> Events => new IEventMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        aggregateMeta.Events
                            .Select(@event => $"{@event.Namespace}.{@event.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<PropertyMetadata> Properties => new PropertyMetadata[] {
                    {{string.Join(
                        ",\r\n        ",
                        // aggregateMeta.Properties.Select(prop => $"new PropertyMetadata(\"{prop.Name}\", \"{prop.DataType}\")")
                        // aggregateMeta.Properties.Select(prop => $"new PropertyMetadata(\"{prop.Name}\", \"{prop.DataType}\")")
                        aggregateMeta.Properties.Select(q => q.ToGeneratorString())
                        )}}
                };
            }
            """;
    }
    
    public static string ForService(ServiceMeta serviceMeta)
    {
        return 
            $$"""
              #nullable enable
              
              using Whaally.Domain.Generators;

              namespace {{serviceMeta.Service.Namespace}};

              public sealed class {{serviceMeta.Service.Name}}Metadata : IServiceMetadata {
                  static {{serviceMeta.Service.Name}}Metadata() { }
                  private {{serviceMeta.Service.Name}}Metadata() { }
                  
                  private static readonly {{serviceMeta.Service.Name}}Metadata instance = new();
                  public static {{serviceMeta.Service.Name}}Metadata Instance => instance;
                  
                  public string Namespace => "{{serviceMeta.Service.Namespace}}";
                  public string Name => "{{serviceMeta.Service.Name}}";
                  
                  public string Description => "";
                  
                  public IEnumerable<ISagaMetadata> CallingSagas => new ISagaMetadata[] {
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.CallingSagas
                              .Select(saga => $"{saga.Namespace}.{saga.Name}Metadata.Instance"))}}
                  };
                  
                  public IEnumerable<IServiceMetadata> CallingServices => new IServiceMetadata[] {
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.CallingServices
                              .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                  };
                  
                  public IEnumerable<IServiceMetadata> Invoked => new IServiceMetadata[] {
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.InvokedServices
                              .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                  };
                  
                  public IEnumerable<ICommandMetadata> Staged => new ICommandMetadata[] {
                      {{string.Join(
                          ",\r\n        ", 
                          serviceMeta.Commands
                              .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}                
                  };
                  
                  public IEnumerable<PropertyMetadata> Properties => new PropertyMetadata[] {
                      {{string.Join(
                            ",\r\n        ", 
                            // serviceMeta.Properties.Select(prop => $"new PropertyMetadata(\"{prop.Name}\", \"{prop.DataType}\")")
                            []
                            )
                      
                      }}
                  };
              }
              """;
    }
    
    public static string ForCommand(CommandMeta commandMeta)
    {
        return 
            $$"""
            #nullable enable
            
            using Whaally.Domain.Generators;
                 
            namespace {{commandMeta.Command.Namespace}};
                 
            public sealed class {{commandMeta.Command.Name}}Metadata : ICommandMetadata {
                static {{commandMeta.Command.Name}}Metadata() { }
                private {{commandMeta.Command.Name}}Metadata() { }

                private static readonly {{commandMeta.Command.Name}}Metadata instance = new();
                public static {{commandMeta.Command.Name}}Metadata Instance => instance;
                
                public string Namespace => "{{commandMeta.Command.Namespace}}";
                public string Name => "{{commandMeta.Command.Name}}";
                
                public string Description => "";

                public IEnumerable<ISagaMetadata> CallingSagas => new ISagaMetadata[] {
                    {{string.Join(
                            ",\r\n        ", 
                            commandMeta.CallingSagas
                                .Select(saga => $"{saga.Namespace}.{saga.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<IServiceMetadata> CallingServices => new IServiceMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.CallingServices
                            .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<ICommandMetadata> CallingCommands => new ICommandMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.CallingCommands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<ICommandMetadata> Invoked => new ICommandMetadata[] { 
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.InvokedCommands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                };
                    
                public IEnumerable<IEventMetadata> Staged => new IEventMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        commandMeta.Events
                            .Select(@event => $"{@event.Namespace}.{@event.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<PropertyMetadata> Properties => new PropertyMetadata[] {
                    {{string.Join(
                            ",\r\n        ", 
                            // commandMeta.Properties.Select(prop => $"new PropertyMetadata(\"{prop.Name}\", \"{prop.DataType}\")")
                            []
                            )
                    
                    }}
                };
            }
            """;
    }

    public static string ForEvent(EventMeta eventMeta)
    {
        return 
            $$"""
            #nullable enable
            
            using Whaally.Domain.Generators;
             
            namespace {{eventMeta.Event.Namespace}};
             
            public sealed class {{eventMeta.Event.Name}}Metadata : IEventMetadata {
                static {{eventMeta.Event.Name}}Metadata() { }
                private {{eventMeta.Event.Name}}Metadata() { }

                private static readonly {{eventMeta.Event.Name}}Metadata instance = new();
                public static {{eventMeta.Event.Name}}Metadata Instance => instance;
                
                public string Namespace => "{{ eventMeta.Event.Namespace }}";
                public string Name => "{{ eventMeta.Event.Name }}";
                
                public string Description => "";
                
                public IEnumerable<ICommandMetadata> CallingCommands => new ICommandMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        eventMeta.Commands
                            .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<IEventMetadata> Invoked => new IEventMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        eventMeta.InvokedEvents
                            .Select(@event => $"{@event.Namespace}.{@event.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<ISagaMetadata> Triggers => new ISagaMetadata[] { };
                
                public IEnumerable<PropertyMetadata> Properties => new PropertyMetadata[] {
                    {{
                        string.Join(
                            ",\r\n        ", 
                            // eventMeta.Properties.Select(prop => $"new PropertyMetadata(\"{prop.Name}\", \"{prop.DataType}\")")
                            []
                        )
                    }}
                };
            }
            """;
    }
    
    public static string ForSaga(SagaMeta sagaMeta)
    {
        return 
            $$"""
            #nullable enable
            
            using Whaally.Domain.Generators;

            namespace {{sagaMeta.Handler.Namespace}};

            public sealed class {{sagaMeta.Handler.Name}}Metadata : ISagaMetadata {
                static {{sagaMeta.Handler.Name}}Metadata() { }
                private {{sagaMeta.Handler.Name}}Metadata() { }

                private static readonly {{sagaMeta.Handler.Name}}Metadata instance = new();
                public static {{sagaMeta.Handler.Name}}Metadata Instance => instance;
                
                public string Namespace => "{{ sagaMeta.Handler.Namespace }}";
                public string Name => "{{ sagaMeta.Handler.Name }}";
                
                public string Description => "";
                
                public IEventMetadata Trigger => {{sagaMeta.Event.Namespace}}.{{sagaMeta.Event.Name}}Metadata.Instance;
                
                public IEnumerable<IServiceMetadata> Invoked => new IServiceMetadata[] {
                    {{string.Join(
                        ",\r\n        ", 
                        sagaMeta.Services
                            .Select(service => $"{service.Namespace}.{service.Name}Metadata.Instance"))}}
                };
                
                public IEnumerable<ICommandMetadata> Staged => new ICommandMetadata[] {
                    {{string.Join(
                            ",\r\n        ", 
                            sagaMeta.Commands
                                .Select(command => $"{command.Namespace}.{command.Name}Metadata.Instance"))}}
                };
            }
            """;
    }
}
