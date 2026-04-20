using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Whaally.Domain.Generators;

/* todo: slop: clean up */
internal static class ArgumentTypeResolver
{
    public static ITypeSymbol?[] GetConcreteArgumentType(IArgumentOperation argument) =>
        GetConcreteArgumentType(argument.Value);
    
    public static ITypeSymbol?[] GetConcreteArgumentType(IOperation argument)
    {
        IOperation current = argument;

        while (true)
        {
            switch (current)
            {
                case IConversionOperation conv:
                    current = conv.Operand;
                    continue;

                case IArgumentOperation arg:
                    current = arg.Value;
                    continue;

                case IConditionalOperation cond:
                    current = cond.WhenTrue;
                    continue;
                
                case IArrayCreationOperation aco:
                    return aco.Initializer?.ElementValues.SelectMany(GetConcreteArgumentType).ToArray() ?? [];
                

                case IInvocationOperation inv:
                    var ret = inv.TargetMethod.ReturnType;
                    if (ret != null && !IsInterfaceWrapper(ret))
                        return [ ret ];

                    return [];

                case IObjectCreationOperation obj:
                    return [ obj.Type ];

                case IFieldReferenceOperation field:
                    return [ field.Type ];

                case IPropertyReferenceOperation prop:
                    return [ prop.Type ];

                case IParameterReferenceOperation param:
                    return [ param.Type ];

                case ILocalReferenceOperation local:
                    return [ local.Type ];

                case IAnonymousObjectCreationOperation anon:
                    return [ anon.Type ];

                case ILiteralOperation lit:
                    return [ lit.Type ];
                
                default:
                    return [ ];
            }
        }
    }

    private static bool IsInterfaceWrapper(ITypeSymbol type)
    {
        // Quick check – if it’s not an interface, it must be concrete.
        if (type.TypeKind != TypeKind.Interface
            && type.TypeKind != TypeKind.Array)
            return false;

        return true;
    }
}
