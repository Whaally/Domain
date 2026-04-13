using Microsoft.CodeAnalysis;

namespace Whaally.Domain.Analyzers;

public class ContainingClassResolver
{
    public static INamedTypeSymbol? GetContainingClassSymbol(IOperation? operation) =>
        GetContainingClassSymbol(operation?.SemanticModel?.GetEnclosingSymbol(operation.Syntax.SpanStart));

    public static INamedTypeSymbol? GetContainingClassSymbol(ISymbol? symbol) =>
        symbol?.ContainingType is null 
        || symbol.ContainingType.TypeKind is TypeKind.Class
            ? symbol?.ContainingType
            : GetContainingClassSymbol(symbol.ContainingType);
}
