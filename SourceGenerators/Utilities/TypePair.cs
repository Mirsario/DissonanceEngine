using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators.Utilities;

public readonly struct TypePair
{
	public readonly TypeDeclarationSyntax Syntax;
	public readonly INamedTypeSymbol Symbol;

	public TypePair(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
	{
		Syntax = syntax;
		Symbol = symbol;
	}
}
