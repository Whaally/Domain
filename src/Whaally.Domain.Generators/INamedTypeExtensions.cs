using Microsoft.CodeAnalysis;

namespace Whaally.Domain.Generators;

public static class INamedTypeExtensions
{
    public static IEnumerable<PropertyMeta> GetPropertyMeta(this INamedTypeSymbol symbol)
    {
        var properties = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(q => !q.IsReadOnly);
        
        // foreach (var prop in properties)
        // {
        //     var attributes = prop.GetAttributes()
        //         .Where(q => q.AttributeClass is
        //         {
        //             BaseType.MetadataName: "ValidationAttribute" or "DataTypeAttribute"
        //         });
        //
        //     foreach (var att in attributes)
        //     {
        //         var cArgs = att.ConstructorArguments;
        //         var nArgs = att.NamedArguments;
        //         
        //         
        //     }
        // }
        
        return properties
            .Select(prop =>
                new PropertyMeta(
                    prop.MetadataName,
                    prop.GetAttributes()
                        .Select(att =>
                            new ValidationRule(
                                att.AttributeClass?.ToDisplayString() ?? "",
                                att.NamedArguments.ToDictionary(q => q.Key, q => q.Value.Value)))))
            .ToList();
    }

    public static string ToGeneratorString(this PropertyMeta propertyMeta) =>
        $$"""
        new PropertyMetadata(
            "{{propertyMeta.Name}}",
            new ValidationRule[] {
                {{string.Join(",\r\n        ", propertyMeta.Rules.Select(ToGeneratorString))}}
            })
        """;

    private static string ToGeneratorString(this ValidationRule rule) =>
        $$"""
        new ValidationRule(
            "{{rule.Name}}",
            new Dictionary<string, object?>() {
                {{ string.Join(",\r\n        ", rule.Arguments.Select(ToGeneratorString)) }}
            })
        """;

    private static string ToGeneratorString(this KeyValuePair<string, object?> kvp) =>
        $$"""
        [ "{{ kvp.Key }}" ] = "{{kvp.Value}}" 
        """;
}
