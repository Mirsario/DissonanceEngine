using System.Text.RegularExpressions;

namespace Dissonance.Engine
{
	internal static class RegexCache
	{
		public static readonly Regex CommandArguments = new(@"-(\w+)(?:[\s]+|\b)(\"".+\""|[^-\s][^\s]*)?", RegexOptions.Compiled);
	}
}
