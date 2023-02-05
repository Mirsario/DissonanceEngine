using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGenerators.Utilities;

internal static class ISymbolExtensions
{
	public static string GetReference(this ISymbol symbol, ReferenceContext context = default)
	{
		string @namespace = symbol.GetNamespace();
		string name = symbol.GetNestedName(context);

		return !context.OmitNamespace(@namespace) && !string.IsNullOrEmpty(@namespace) ? $"{@namespace}.{name}" : name;
	}
	
	public static string GetFullName(this ISymbol symbol)
	{
		string @namespace = symbol.GetNamespace();
		string name = symbol.GetNestedName();

		return !string.IsNullOrEmpty(@namespace) ? $"{@namespace}.{name}" : name;
	}

	public static string GetName(this ISymbol symbol, ReferenceContext context = default)
	{
		if (symbol is INamedTypeSymbol { IsGenericType: true } typeSymbol) {
			return $"{symbol.Name}<{string.Join(", ", typeSymbol.TypeArguments.Select(a => a.GetReference(context)))}>";
		}

		return symbol.Name;
	}

	public static string GetNestedName(this ISymbol symbol, ReferenceContext context = default)
	{
		string nests = symbol.GetNestedPath();
		string name = symbol.GetName(context);

		return !string.IsNullOrEmpty(nests) ? $"{nests}.{name}" : name;
	}
	
	public static string GetNestedPath(this ISymbol symbol)
	{
		var sb = new StringBuilder();
		bool appendDot = false;

		while (true) {
			symbol = symbol.ContainingType;

			if (symbol == null || string.IsNullOrEmpty(symbol.Name)) {
				break;
			}

			if (appendDot) {
				sb.Insert(0, '.');
			} else {
				appendDot = true;
			}

			sb.Insert(0, symbol.Name);
		}

		return sb.ToString();
	}

	public static string GetNamespace(this ISymbol symbol)
	{
		var sb = new StringBuilder();
		bool appendDot = false;

		while (true) {
			symbol = symbol.ContainingNamespace;

			if (symbol == null || string.IsNullOrEmpty(symbol.Name)) {
				break;
			}

			if (appendDot) {
				sb.Insert(0, '.');
			} else {
				appendDot = true;
			}

			sb.Insert(0, symbol.Name);
		}

		return sb.ToString();
	}
}
