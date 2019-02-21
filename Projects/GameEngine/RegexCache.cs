using System.Text.RegularExpressions;

namespace GameEngine
{
	internal static class RegexCache
	{
		public static Regex shaderFSuffixA = new Regex(@"([^.]|^)([\d]+)f(?=[^\w])");
		public static Regex shaderFSuffixB = new Regex(@"(\.)([\d]+)f(?=[^\w])");

		public static Regex commandArguments = new Regex(@"-(\w+)(?:[\s]+|\b)(\"".+\""|[^-\s][^\s]*)?");
	}
}
