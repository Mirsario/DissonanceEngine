using System.Text.RegularExpressions;

namespace SourceGenerators.Utilities
{
	public static class StringUtils
	{
		private static readonly Regex endLineBreakRegex = new Regex(@"(?:\r\n|\n\r|\r|\n)$", RegexOptions.Compiled);

		public static string OmitEndLineBreak(string input)
		{
			return endLineBreakRegex.Replace(input, string.Empty);
		}
	}
}
