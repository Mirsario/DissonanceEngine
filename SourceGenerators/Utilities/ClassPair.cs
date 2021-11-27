using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators.Utilities
{
	internal readonly struct ClassPair
	{
		public readonly ClassDeclarationSyntax Syntax;
		public readonly INamedTypeSymbol Symbol;

		public ClassPair(ClassDeclarationSyntax syntax, INamedTypeSymbol symbol)
		{
			Syntax = syntax;
			Symbol = symbol;
		}
	}
}
