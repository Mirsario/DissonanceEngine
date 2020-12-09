using System.Text.RegularExpressions;

namespace Dissonance.Engine.Utilities
{
	public static class StringExtensions
	{
		public static string ReplaceCaseInsensitive(this string str, string oldValue, string newValue)
			=> Regex.Replace(str, Regex.Escape(oldValue), newValue.Replace("$", "$$"), RegexOptions.IgnoreCase);
		public static string ReplaceCaseInsensitive(this string str, params (string oldValue, string newValue)[] replacements)
		{
			for(int i = 0; i < replacements.Length; i++) {
				(string oldValue, string newValue) = replacements[i];

				str = str.ReplaceCaseInsensitive(oldValue, newValue);
			}

			return str;
		}
	}
}
