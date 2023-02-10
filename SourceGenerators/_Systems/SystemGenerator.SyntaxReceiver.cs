using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerators.Utilities;

namespace SourceGenerators;

partial class SystemGenerator
{
	/// <summary> Created on demand before each generation pass </summary>
	private sealed class SyntaxReceiver : ISyntaxContextReceiver
	{
		public List<(TypePair classPair, List<(MethodPair methodPair, AttributeData systemAttribute)> systemMethods)> SystemMethods { get; } = new();

		/// <summary> Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation </summary>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax) {
				return;
			}

			var namedTypeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

			if (namedTypeSymbol == null) {
				return;
			}

			List<(MethodPair, AttributeData)>? systemMethods = null;

			foreach (var member in classDeclarationSyntax.Members) {
				if (member is not MethodDeclarationSyntax methodDeclarationSyntax) {
					continue;
				}

				IMethodSymbol? methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
				
				if (methodSymbol == null) {
					continue;
				}

				static bool IsSystemAttribute(AttributeData attributeData)
				{
					var attributeClass = attributeData.AttributeClass;

					while (attributeClass != null) {
						if (attributeClass.GetFullName() == "Dissonance.Engine.SystemAttribute") {
							return true;
						}

						attributeClass = attributeClass.BaseType;
					}

					return false;
				}

				var systemAttribute = methodSymbol.GetAttributes().FirstOrDefault(IsSystemAttribute);

				if (systemAttribute == null) {
					continue;
				}

				var methodPair = new MethodPair(methodDeclarationSyntax, methodSymbol);

				(systemMethods ??= new()).Add((methodPair, systemAttribute));
			}

			if (systemMethods != null) {
				var classPair = new TypePair(classDeclarationSyntax, namedTypeSymbol);

				SystemMethods.Add((classPair, systemMethods));
			}
		}
	}
}
