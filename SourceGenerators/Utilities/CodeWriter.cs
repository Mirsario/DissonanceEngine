using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators.Utilities
{
	public sealed class CodeWriter
	{
		private static readonly Regex lineStartRegex = new(@"(^|\r\n|\n\r|\r|\n)", RegexOptions.Compiled);

		public readonly StringBuilder StringBuilder;

		private int numTabs;
		private string tabs;
		private string tabsReplacement;

		public CodeWriter() : this(new StringBuilder()) { }

		public CodeWriter(StringBuilder stringBuilder)
		{
			StringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
			tabsReplacement = tabs = string.Empty;
		}

		public override string ToString()
			=> StringBuilder.ToString();

		public void Indent()
		{
			tabs = new string('\t', ++numTabs);
			tabsReplacement = "$1" + tabs;
		}

		public void Unindent()
		{
			if (numTabs <= 0) {
				throw new InvalidOperationException("Already at 0 tabs.");
			}

			tabs = new string('\t', --numTabs);
			tabsReplacement = "$1" + tabs;
		}

		public void Append(string text)
			=> StringBuilder.Append(lineStartRegex.Replace(text, tabsReplacement));

		public void AppendLine()
			=> StringBuilder.AppendLine(tabs);

		public void AppendLine(string text)
			=> StringBuilder.AppendLine(lineStartRegex.Replace(text, tabsReplacement));

		public void AppendCode(CodeWriter code, bool omitEndLineBreak = true)
		{
			if (code.StringBuilder.Length == 0) {
				return;
			}

			string text = code.ToString();

			if (omitEndLineBreak) {
				text = StringUtils.OmitEndLineBreak(text);
			}

			AppendLine(text);
		}
	}
}
