using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators.Utilities
{
	internal readonly struct MethodPair
	{
		public readonly MethodDeclarationSyntax Syntax;
		public readonly IMethodSymbol Symbol;

		public MethodPair(MethodDeclarationSyntax syntax, IMethodSymbol symbol)
		{
			Syntax = syntax;
			Symbol = symbol;
		}
	}
}
