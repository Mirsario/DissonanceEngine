using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGenerators.Utilities;

internal static class ISymbolExtensions
{
	public static string GetFullName(this ISymbol symbol)
	{
		string ns = GetNamespace(symbol);

		return !string.IsNullOrWhiteSpace(ns) ? $"{ns}.{symbol.Name}" : symbol.Name;
	}

	public static string GetNamespace(this ISymbol symbol)
	{
		var sb = new StringBuilder();
		bool appendDot = false;

		while (true) {
			symbol = symbol.ContainingNamespace;

			if (symbol == null || string.IsNullOrWhiteSpace(symbol.Name)) {
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
