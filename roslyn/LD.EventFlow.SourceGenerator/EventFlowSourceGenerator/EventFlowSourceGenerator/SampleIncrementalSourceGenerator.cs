using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace LD.EventFlow.SourceGenerator;
[Generator]
public class SampleIncrementalSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        Console.WriteLine("Run"); 
        var factoryProvider = IMessagesConstructorToFactoryMethod(context);
        context.RegisterSourceOutput(factoryProvider.Collect(), (spc, value) =>
        {
            int idx = 0;
            foreach(var item in value)
            {
                spc.AddSource(item.Item1, item.Item2);
            } 
        });
    } 
    
    IncrementalValuesProvider<(string, string)> IMessagesConstructorToFactoryMethod(IncrementalGeneratorInitializationContext context)
    {
        var p = context.SyntaxProvider.CreateSyntaxProvider<(string,string)>(predicate: static (node, _) =>
        { 
            // 노드가 타입 지정된 경우
            return node is StructDeclarationSyntax typeDeclared && typeDeclared.BaseList != null &&
                   typeDeclared.BaseList.Types.Count() != 0;
        }, transform: (syntax,
            _) =>
        { 
            StructDeclarationSyntax? typeDeclarationSyntax = syntax.Node as StructDeclarationSyntax;


            foreach (var members in typeDeclarationSyntax.Members)
            {
                if (members is ConstructorDeclarationSyntax constructorSyntax)
                {
                    if (constructorSyntax.ParameterList.Parameters != null &&
                        constructorSyntax.ParameterList.Parameters.Count != 0)
                    {
                        string src =  $@"  
 $namespace_start$

public partial struct $classname${{

    $code$

}}

$namespace_end$
";
                        var fullConstructionParameter = constructorSyntax.ParameterList.Parameters[0].Parent.ToString();
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
    public static {typeDeclarationSyntax.Identifier} Create{fullConstructionParameter}
    {{
        return new {typeDeclarationSyntax.Identifier}({initialize});
    }}

");

                        return ($"{typeDeclarationSyntax.Identifier}.g.cs",src);
                    } 
                }
            }

            return default;
        });

        return p;
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

