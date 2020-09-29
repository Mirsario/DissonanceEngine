using System.Text.RegularExpressions;

namespace Dissonance.Engine.Core.Internal
{
	internal static class RegexCache
	{
		public static Regex commandArguments = new Regex(@"-(\w+)(?:[\s]+|\b)(\"".+\""|[^-\s][^\s]*)?", RegexOptions.Compiled);
	}
}
