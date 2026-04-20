using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace Whaally.Domain.Generators;

[Generator]
public sealed class GeneralizedMetadataGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context
            .RegisterPostInitializationOutput(i =>
            {
                i.AddEmbeddedAttributeDefinition();
                i.AddSource(
                    "GenerateMetadataAttribute.g.cs",
                    """
                    namespace Whaally.Domain.Generators
                    {
                        [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
                        internal class GenerateMetadataAttribute: global::System.Attribute {} 
                    }
                    """);
            });

        IncrementalValuesProvider<IMetadataModel> domainComponents = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Whaally.Domain.Generators.GenerateMetadataAttribute",
            predicate: (node, _) => node is TypeDeclarationSyntax { BaseList: not null },
            transform: (syntaxContext, _) =>
            {
                if (syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.TargetNode) is not INamedTypeSymbol handlerClass) return null;

                if (handlerClass
                        .AllInterfaces
                        .SingleOrDefault(q =>
                            q.MetadataName is "IAggregate" 
                                or "IServiceHandler`1"
                                or "ICommandHandler`2"
                                or "IEventHandler`2"
                                or "ISaga`1") is not { } handlerDefinition) return null;

                var invocationExpressions = handlerClass
                    .GetMembers()
                    .SingleOrDefault(m => m is
                    {
                        Name: "Invoke" or "Evaluate" or "Apply" // retrieve the handler method
                    })
                    ?.DeclaringSyntaxReferences[0]
                    .GetSyntax()
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>();
                    
                var operations = invocationExpressions
                    ?.Select(q => syntaxContext.SemanticModel.GetOperation(q) as IInvocationOperation)
                    .Where(q => q is
                    {
                        TargetMethod.Name: "Invoke" or "Stage",
                        Instance.Type: not null
                    })
                    .Where(q => q != null)!
                    .ToList() ?? [];

                return (IMetadataModel?)(handlerDefinition.MetadataName switch
                {
                    "IAggregate" => new AggregateMeta(handlerClass),
                    "IServiceHandler`1" => new ServiceMeta(
                        service: (INamedTypeSymbol)handlerDefinition.TypeArguments[0], handler: handlerClass,
                        services: operations.Where(q => q!.TargetMethod.Name == "Invoke")
                            .SelectMany(q => q!.Arguments.SelectMany(ArgumentTypeResolver.GetConcreteArgumentType))
                            .OfType<INamedTypeSymbol>(),
                        commands: operations.Where(q => q!.TargetMethod.Name == "Stage")
                            .SelectMany(q => q!.Arguments
                                .Skip(1) // skip the argument holding the id of the target aggregate
                                .SelectMany(ArgumentTypeResolver.GetConcreteArgumentType))
                            .OfType<INamedTypeSymbol>()),
                    "ICommandHandler`2" => new CommandMeta(
                        aggregate: (INamedTypeSymbol)handlerDefinition.TypeArguments[0],
                        command: (INamedTypeSymbol)handlerDefinition.TypeArguments[1], handler: handlerClass,
                        commands: operations.Where(q => q!.TargetMethod.Name == "Invoke")
                            .SelectMany(q => q!.Arguments.SelectMany(ArgumentTypeResolver.GetConcreteArgumentType))
                            .OfType<INamedTypeSymbol>(),
                        events: operations.Where(q => q!.TargetMethod.Name == "Stage")
                            .SelectMany(q => q!.Arguments.SelectMany(ArgumentTypeResolver.GetConcreteArgumentType))
                            .OfType<INamedTypeSymbol>()),
                    "IEventHandler`2" => new EventMeta(aggregate: (INamedTypeSymbol)handlerDefinition.TypeArguments[0],
                        @event: (INamedTypeSymbol)handlerDefinition.TypeArguments[1], handler: handlerClass),
                    "ISaga`1" => new SagaMeta(@event: (INamedTypeSymbol)handlerDefinition.TypeArguments[0],
                        handler: handlerClass,
                        services: operations.Where(q => q!.TargetMethod.Name == "Invoke")
                            .SelectMany(q => q!.Arguments.SelectMany(ArgumentTypeResolver.GetConcreteArgumentType))
                            .OfType<INamedTypeSymbol>(),
                        commands: operations.Where(q => q!.TargetMethod.Name == "Stage")
                            .SelectMany(q => q!.Arguments
                                .Skip(1) // skip the argument holding the id of the target aggregate
                                .SelectMany(ArgumentTypeResolver.GetConcreteArgumentType))
                            .OfType<INamedTypeSymbol>()),
                    _ => null
                });
            })
            .Where(q => q != null)!;

        IncrementalValuesProvider<AggregateMeta> aggregate = domainComponents
            .Select((q, _) => q as AggregateMeta)
            .Where(q => q != null)!;

        // split up all domain components into subcollections for easier matching
        IncrementalValuesProvider<ServiceMeta> service = domainComponents
            .Select((q, _) => q as ServiceMeta)
            .Where(q => q != null)!;

        IncrementalValuesProvider<CommandMeta> command = domainComponents
            .Select((q, _) => q as CommandMeta)
            .Where(q => q != null)!;

        IncrementalValuesProvider<EventMeta> @event = domainComponents
            .Select((q, _) => q as EventMeta)
            .Where(q => q != null)!;

        IncrementalValuesProvider<SagaMeta> saga = domainComponents
            .Select((q, _) => q as SagaMeta)
            .Where(q => q != null)!;

        // create collections so we can search through these
        var services = service.Collect();
        var commands = command.Collect();
        var events = @event.Collect();
        var sagas = saga.Collect();

        // first try to figure out which commands and events belong to a given aggregate
        aggregate = aggregate
            .Combine(commands)
            .Select((input, _) =>
            {
                AggregateMeta single = input.Left;
                ImmutableArray<CommandMeta> collection = input.Right;
        
                single.Commands.AddRange(
                    collection
                        .Where(q => q.Aggregate == single.Aggregate)
                        .Select(q => q.Command));
        
                return single;
            });
        
        aggregate = aggregate
            .Combine(events)
            .Select((input, _) =>
            {
                AggregateMeta single = input.Left;
                ImmutableArray<EventMeta> collection = input.Right;
        
                single.Events.AddRange(
                    collection
                        .Where(q => q.Aggregate == single.Aggregate)
                        .Select(q => q.Event));
        
                return single;
            });
        
        
        // for services, determine all possible callers from within the domain
        service = service
            .Combine(services)
            .Select((input, _) =>
            {
                ServiceMeta single = input.Left;
                ImmutableArray<ServiceMeta> collection = input.Right;
                
                single.CallingServices.AddRange(
                    collection
                        .Where(q => q.InvokedServices.Contains(single.Service))
                        .Select(q => q.Service));

                return single;
            });
        
        service = service
            .Combine(sagas)
            .Select((input, _) =>
            {
                ServiceMeta single = input.Left;
                ImmutableArray<SagaMeta> collection = input.Right;
                
                single.CallingSagas.AddRange(
                    collection
                        .Where(q => q.Services.Contains(single.Service))
                        .Select(q => q.Handler));

                return single;
            });
        
        // for a command, figure out all possible callers from within the domain
        command = command
            .Combine(sagas)
            .Select((input, _) =>
            {
                CommandMeta single = input.Left;
                ImmutableArray<SagaMeta> collection = input.Right;
                
                single.CallingSagas.AddRange(
                    collection
                        .Where(q => q.Commands.Contains(single.Command))
                        .Select(q => q.Handler));

                return single;
            });
        
        command = command
            .Combine(services)
            .Select((input, _) =>
            {
                CommandMeta single = input.Left;
                ImmutableArray<ServiceMeta> collection = input.Right;
                
                single.CallingServices.AddRange(
                    collection
                        .Where(q => q.Commands.Contains(single.Command))
                        .Select(q => q.Service));

                return single;
            });
        
        command = command
            .Combine(commands)
            .Select((input, _) =>
            {
                CommandMeta single = input.Left;
                ImmutableArray<CommandMeta> collection = input.Right;
                
                single.CallingCommands.AddRange(
                    collection
                        .Where(q => q.InvokedCommands.Contains(single.Command))
                        .Select(q => q.Command));

                return single;
            });
        
        // for an event; determine all possilbe callers from within the domain
        @event = @event
            .Combine(commands)
            .Select((input, _) =>
            {
                EventMeta single = input.Left;
                ImmutableArray<CommandMeta> collection = input.Right;
                
                single.Commands.AddRange(
                    collection
                        .Where(q => q.Events.Contains(single.Event))
                        .Select(q => q.Command));

                return single;
            });
        
        // side case: signal which sagas are triggered by some event
        @event = @event
            .Combine(sagas)
            .Select((input, _) =>
            {
                EventMeta single = input.Left;
                ImmutableArray<SagaMeta> collection = input.Right;
                
                single.Sagas.AddRange(
                    collection
                        .Where(q => q.Event == single.Event)
                        .Select(q => q.Handler));

                return single;
            });
        
        // note that we cannot yet call events from events
        
        // for sagas it is already known which events trigger them due to type info
        
        context.RegisterSourceOutput(
            aggregate,
            (spc, meta) =>
            {
                spc.AddSource(
                    $"{meta.Aggregate.Namespace}.{meta.Aggregate.Name}.g.cs",
                    SourceText.From(
                        MetadataGenerator.ForAggregate(meta),
                        Encoding.UTF8));
            });
        
        context.RegisterSourceOutput(
            service,
            (spc, meta) =>
            {
                spc.AddSource(
                    $"{meta.Service.Namespace}.{meta.Service.Name}.g.cs",
                    SourceText.From(
                        MetadataGenerator.ForService(meta),
                        Encoding.UTF8));
            });

        context.RegisterSourceOutput(
            command,
            (spc, meta) =>
            {
                spc.AddSource(
                    $"{meta.Command.Namespace}.{meta.Command.Name}.g.cs",
                    SourceText.From(
                        MetadataGenerator.ForCommand(meta),
                        Encoding.UTF8));
            });
        
        context.RegisterSourceOutput(
            @event,
            (spc, meta) =>
            {
                spc.AddSource(
                    $"{meta.Event.Namespace}.{meta.Event.Name}.g.cs",
                    SourceText.From(
                        MetadataGenerator.ForEvent(meta),
                        Encoding.UTF8));
            });
        
        context.RegisterSourceOutput(
            saga,
            (spc, meta) =>
            {
                spc.AddSource(
                    $"{meta.Handler.Namespace}.{meta.Handler.Name}.g.cs",
                    SourceText.From(
                        MetadataGenerator.ForSaga(meta),
                        Encoding.UTF8));
            });
    }
}
