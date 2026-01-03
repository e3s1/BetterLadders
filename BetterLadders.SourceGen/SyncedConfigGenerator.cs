using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BetterLadders.SourceGen;

[Generator]
public class SyncedConfigGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
        {
            Console.Error.WriteLine("Syntax receiver not supported.");
            return;
        }
        
        var compilation = context.Compilation;
        
        var configSymbol = receiver.ClassDeclarations
            .Select(decl => compilation.GetSemanticModel(decl.SyntaxTree).GetDeclaredSymbol(decl))
            .OfType<INamedTypeSymbol>()
            .First(s => s.GetAttributes().Any(a =>
                a.AttributeClass?.Name == "SourceGenAttribute"
            ));

        var attrData = configSymbol.GetAttributes()
            .First(a => a.AttributeClass?.Name == "SourceGenAttribute");

        var targetFieldType = (INamedTypeSymbol)attrData.ConstructorArguments[0].Value!;
        var targetClassType = (INamedTypeSymbol)attrData.ConstructorArguments[1].Value!;

        var syncedFields = configSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f =>
            {
                if (f.Type is INamedTypeSymbol { IsGenericType: true } nts)
                {
                    return nts.Name == targetFieldType.Name
                           && nts.Arity == targetFieldType.Arity;
                }

                return false;
            })
            .ToList();

        var targetClassNamespace = targetClassType.ContainingNamespace;
        var targetClassName = targetClassType.Name;
        var networkVariableDefinitions = string.Join("\n\t", syncedFields
            .Select(f => (fieldName: f.Name, type: ((INamedTypeSymbol)f.Type).TypeArguments.First()))
            .Select(x => $"internal readonly NetworkVariable<{x.type}> {x.fieldName} = new();"));
        var networkVariablesList = string.Join(",\n\t\t\t", syncedFields.Select(f => f.Name));
        
        var contents = $$"""
                         using Unity.Netcode;

                         namespace {{targetClassNamespace}};

                         public partial class {{targetClassName}}
                         {
                             {{networkVariableDefinitions}}
                             
                             internal NetworkVariableBase[] NetworkVariables;
                             
                             private void Awake()
                             {
                                 NetworkVariables =
                                 [
                                     {{networkVariablesList}}
                                 ];
                             }
                         }
                         """;
        
        context.AddSource($"{targetClassName}.generated.cs", SourceText.From(contents, Encoding.UTF8));
    }
}

class SyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> ClassDeclarations { get; } = [];

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        // any field with at least one attribute is a candidate for property generation
        if (syntaxNode is ClassDeclarationSyntax decl)
        {
            ClassDeclarations.Add(decl);
        }
    }
}