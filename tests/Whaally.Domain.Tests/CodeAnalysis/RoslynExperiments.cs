using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace Whaally.Domain.Tests.CodeAnalysis;

public class RoslynExperiments : IAsyncLifetime
{
    private MSBuildWorkspace? _workspace;
    private Solution? _solution;
    private Project? _consoleSample;
    private Document? _addItem;
    private SyntaxTree? _syntaxTree;
    private SemanticModel? _semanticModel;
    private SyntaxNode? _root;
    private GenericNameSyntax? _commandHandler;
    private IdentifierNameSyntax? _command;

    public async Task InitializeAsync()
    {
        _workspace = MSBuildWorkspace.Create();
        _solution = await _workspace.OpenSolutionAsync("../../../Whaally.Domain.sln");
        _consoleSample = _solution.Projects.Single(q => q.Name == "ConsoleSample");
        _addItem = _consoleSample.Documents.Single(q => q.Name == "AddItem.cs");
        _syntaxTree = (await _addItem.GetSyntaxTreeAsync())!;
        _semanticModel = (await _addItem.GetSemanticModelAsync())!;
        _root = await _syntaxTree.GetRootAsync();

        _commandHandler = _root.DescendantNodes()
            .OfType<GenericNameSyntax>()
            .Single(q => q.Identifier.Text == "ICommandHandler");

        _command = _root.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Single(q => q.Identifier.Text == "ICommand");
    }
    
    [Fact]
    public void CommandShouldNotBeNull() => _command.Should().NotBeNull();

    [Fact]
    public void CommandHandlerShouldNotBeNull() => _commandHandler.Should().NotBeNull();
    
    [Fact]
    public void SolutionIsNotNull() => _solution.Should().NotBeNull();

    [Fact]
    public void ConsoleSampleProjectIsNotNull() => _consoleSample.Should().NotBeNull();

    [Fact]
    public void AddItemIsNotNull() => _addItem.Should().NotBeNull();

    [Fact]
    public void CanGetCommandHandlerClass()
    {
        var @class = _root?
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Should()
            .ContainSingle()
            .Subject;
        
        // Method one to retrieve some command handler
        @class?.BaseList?.Types.SingleOrDefault(q => q.Type == _commandHandler).Should().NotBeNull();
        
        // Hardcoding an assumption, not practical in the real world
        @class?.Identifier.Text.Should().Be("AddItemHandler");
    }
    
    [Fact]
    public void CanGetCommandRecord()
    {
        var @record = _root?
            .DescendantNodes()
            .OfType<RecordDeclarationSyntax>()
            .Should()
            .ContainSingle()
            .Subject;

        @record?.BaseList?.Types.SingleOrDefault(q => q.Type == _command).Should().NotBeNull();
        @record?.Identifier.Text.Should().Be("AddItem");
    }
    
    [Fact]
    public void GetHandler() => _root?.DescendantNodes().OfType<MethodDeclarationSyntax>().Should().NotBeNull();

    public Task DisposeAsync() => Task.CompletedTask;
}