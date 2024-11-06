using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DummyAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule1 = new ("D001", "Require Construct Attribute", "Require Construct Attribute", "Test", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor Rule2 = new ("D002", "CompilationAction", "CompilationAction", "Test", DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule1, Rule2);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(analysisContext =>
        {
            var interfaceDeclarationSyntax = analysisContext.Node as InterfaceDeclarationSyntax;
           
            if (interfaceDeclarationSyntax!.BaseList != null)
            {
                foreach (var item in interfaceDeclarationSyntax.BaseList.Types)
                {  
                    if (item.ToString().Contains("IEventMessage"))
                    { 
                        if(!(interfaceDeclarationSyntax.AttributeLists.Any(x=>x.ToString() == "[Construct]")))
                        {
                            analysisContext.ReportDiagnostic(Diagnostic.Create(Rule1, interfaceDeclarationSyntax.GetLocation()));
                        }
                    }
                }
            }
        }, SyntaxKind.InterfaceDeclaration); 
    } 
}