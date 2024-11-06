using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace LD.EventFlow.SourceGenerator;
[Generator] 
public class ConstructMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    { 
        Console.WriteLine("Run"); 
        
        context.RegisterPostInitializationOutput(x =>
        {
            x.AddSource("Attributes.g.cs", $@"

using System;
namespace LD.EventFlow.Attributes{{
[AttributeUsage(AttributeTargets.Interface, Inherited = true)] 
    public class ConstructAttribute : System.Attribute{{

    }}
}}
");
        });
        var factoryProvider = IMessagesConstructorToFactoryMethod(context);
        context.RegisterSourceOutput(factoryProvider.Collect(), (spc, value) =>
        { 
            foreach(var item in value)
            {
                if (item.Item1 != null)
                {
                    spc.AddSource(item.Item1, item.Item2);
                }
                else
                {
                   
                }
            } 
        });
    }
    // 주어진 구조체(Struct)에 대해, 본인 구조체 또는 구현한 인터페이스에 특정 어트리뷰트가 있는지 확인
    public static bool HasAttributeOnBaseOrSelf(StructDeclarationSyntax structNode, SemanticModel semanticModel, string attributeName)
    {
        // 구조체의 심볼 가져오기
        var structSymbol = semanticModel.GetDeclaredSymbol(structNode) as INamedTypeSymbol;
 
        
        if (structSymbol == null)
        {
            return false; // 구조체 심볼을 찾을 수 없으면 false
        }

        // 본인 구조체에 어트리뷰트가 있는지 확인
        if (HasAttribute(structSymbol, attributeName))
        {
            return true;
        }

        // 구조체가 구현한 인터페이스들에 대해 어트리뷰트가 있는지 확인
        foreach (var interfaceSymbol in structSymbol.Interfaces)
        { 
            if (HasAttribute(interfaceSymbol, attributeName))
            {
                return true;
            }
        }

        return false;
    }

    // 주어진 타입 심볼에 어트리뷰트가 있는지 확인
    private static bool HasAttribute(INamedTypeSymbol symbol, string attributeName)
    {
        return symbol.GetAttributes().Any(attr => attr.AttributeClass.Name == attributeName);
    }

    IncrementalValuesProvider<(string, string)> IMessagesConstructorToFactoryMethod(IncrementalGeneratorInitializationContext context)
    {
        var p = context.SyntaxProvider.CreateSyntaxProvider<(string,string)>(predicate: static (node, _) =>
        { 
            // 노드가 타입 지정된 경우
            return node is StructDeclarationSyntax typeDeclared &&
                   typeDeclared.BaseList.Types.Count() != 0;
        }, transform: (syntax,
            _) =>
        { 
            if (HasAttributeOnBaseOrSelf(syntax.Node as StructDeclarationSyntax, syntax.SemanticModel,
                    "ConstructAttribute"))
            {
                StructDeclarationSyntax? typeDeclarationSyntax = syntax.Node as StructDeclarationSyntax;

                foreach (var members in typeDeclarationSyntax.Members)
                {
                    if (members is ConstructorDeclarationSyntax constructorSyntax)
                    {
                        if (constructorSyntax.ParameterList.Parameters != null &&
                            constructorSyntax.ParameterList.Parameters.Count != 0)
                        {
                            string src = $@"  
 $namespace_start$

public partial struct $classname${{

    $code$

}}

$namespace_end$
";
                            var fullConstructionParameter =
                                constructorSyntax.ParameterList.Parameters[0].Parent.ToString();
                            var parameters = constructorSyntax.ParameterList.Parameters;
                            string initialize = string.Join(",", parameters.Select(x => x.Identifier));
                            var @structNamespace = GetNamespace(typeDeclarationSyntax);

                            if (string.IsNullOrEmpty(@structNamespace))
                            {
                                src = src.Replace("$namespace_start$", "");
                                src = src.Replace("$namespace_end$", "");
                            }
                            else
                            {
                                src = src.Replace("$namespace_start$", $"namespace {@structNamespace}{{");
                                src = src.Replace("$namespace_end$", "}");
                            }

                            src = src.Replace("$classname$", typeDeclarationSyntax.Identifier.ToString());
                            src = src.Replace("$code$", $@"
    public static {typeDeclarationSyntax.Identifier} Construct{fullConstructionParameter}
    {{
        return new {typeDeclarationSyntax.Identifier}({initialize});
    }}

");
                            string prefix = string.IsNullOrWhiteSpace(@structNamespace)
                                ? "Global"
                                : $"{@structNamespace}";
                            return ($"{prefix}.{typeDeclarationSyntax.Identifier}.g.cs", src);
                        }
                    }
                } 
            }

            return default;
        });

        return p;
    }
    
    static bool HasAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
    {
        foreach (var attributeList in attributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (attribute.Name.ToString() == attributeName)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    static string GetNamespace(StructDeclarationSyntax syntax)
    { 
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode? potentialNamespaceParent = syntax.Parent;
    
        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();
        
            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.({nameSpace})";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }
}

