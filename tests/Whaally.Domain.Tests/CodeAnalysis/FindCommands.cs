using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace Whaally.Domain.Tests.CodeAnalysis;

public class FindCommands : IAsyncLifetime
{
    private MSBuildWorkspace? _workspace;
    private Solution? _solution;
    private Project? _consoleSample;
    private SyntaxTree? _syntaxTree;
    private SyntaxNode? _root;

    private List<TypeDeclarationSyntax> _commands = [];

    public async Task InitializeAsync()
    {
        _workspace = MSBuildWorkspace.Create();
        _solution = await _workspace.OpenSolutionAsync("../../../Whaally.Domain.sln");
        _consoleSample = _solution.Projects.Single(q => q.Name == "ConsoleSample");
        
        foreach (var doc in _consoleSample.Documents)
        {
            _syntaxTree = await doc.GetSyntaxTreeAsync();
            _root = await _syntaxTree?.GetRootAsync()!;

            var descendantNodes = _root.DescendantNodes();
            
            var implementations = descendantNodes
                .OfType<IdentifierNameSyntax>()
                .Where(q => q.Identifier.Text == "ICommand");

            _commands.AddRange(
                descendantNodes
                    .OfType<RecordDeclarationSyntax>()
                    .Where(q => q.BaseList?.Types.Any(w => implementations.Contains(w.Type)) ?? false));
            
            _commands.AddRange(
                descendantNodes
                    .OfType<ClassDeclarationSyntax>()
                    .Where(q => q.BaseList?.Types.Any(w => implementations.Contains(w.Type)) ?? false));
        }
    }

    [Fact]
    public void HasSome() => _commands.Should().HaveCount(3);
    
    public Task DisposeAsync() => Task.CompletedTask;
}