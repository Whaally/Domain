using Microsoft.CodeAnalysis;

namespace Whaally.Domain.Generators;

public static class INamedTypeExtensions
{
    public static IEnumerable<PropertyMeta> GetPropertyMeta(this INamedTypeSymbol symbol)
    {
        var properties = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(q => !q.IsReadOnly);

        // todo: get properties on primary constructors, then merge with object properties.
        //       primarily to be able to decorate the constructor arguments with attributes as well. 
        
        return properties
            .Select(prop =>
                new PropertyMeta(
                    prop.MetadataName,
                    prop.GetAttributes()
                        .Select(att =>
                        {
                            var constructorParams = att.AttributeConstructor?.Parameters.Select(q => q.Name) ?? [];
                            var constructorArguments = att.ConstructorArguments.Select(q => q.Value ?? q.Values);

                            KeyValuePair<string, object?>[] args =
                            [
                                ..constructorParams.Zip(constructorArguments,
                                    (a, b) => new KeyValuePair<string, object?>(a, b)),
                                ..att.NamedArguments.Select(q =>
                                    new KeyValuePair<string, object?>(q.Key, q.Value.Value))
                            ];
                            
                            return new ValidationRule(
                                att.AttributeClass?.Name.Replace("Attribute", "") ?? "",
                                args.ToDictionary(q => q.Key, q => q.Value));
                        })))
            .ToList();
    }

    public static string ToGeneratorString(this PropertyMeta propertyMeta) =>
        $$"""
        new PropertyMetadata(
            "{{propertyMeta.Name}}",
            new ValidationRule[] {
                {{ string.Join(",\r\n        ", propertyMeta.Rules.Select(ToGeneratorString)).Indent().Indent() }}
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
        [ "{{ System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(kvp.Key) }}" ] = "{{kvp.Value}}" 
        """;
}
