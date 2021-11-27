using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	partial class SubsystemGenerator
	{
		/// <summary> Created on demand before each generation pass </summary>
		private sealed class SyntaxReceiver : ISyntaxContextReceiver
		{
			public List<(ClassPair classPair, List<MethodPair> methods)> SystemTypes { get; } = new();

			/// <summary> Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation </summary>
			public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
			{
				if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax) {
					return;
				}

				if (classDeclarationSyntax?.BaseList?.Types.Any(
					baseTypeSyntax => baseTypeSyntax is SimpleBaseTypeSyntax {
						Type: IdentifierNameSyntax {
							Identifier.Value: "GameSystem"
						}
					}
				) != true) {
					return;
				}

				INamedTypeSymbol? namedTypeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

				if (namedTypeSymbol != null) {
					var methods = new List<MethodPair>();

					foreach (var member in classDeclarationSyntax.Members) {
						if (member is not MethodDeclarationSyntax methodDeclarationSyntax) {
							continue;
						}

						IMethodSymbol? methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

						if (methodSymbol != null) {
							methods.Add(new MethodPair(methodDeclarationSyntax, methodSymbol));
						}
					}

					SystemTypes.Add((new ClassPair(classDeclarationSyntax, namedTypeSymbol), methods));
				}
			}
		}
	}
}
