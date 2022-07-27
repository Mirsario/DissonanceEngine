using System.Text;
using System.Text.RegularExpressions;

namespace SourceGenerators.Utilities;

public sealed class CodeWriter
{
	private static readonly Regex lineBreakRegex = new(@"(\r\n|\n\r|\r|\n)", RegexOptions.Compiled);

	public readonly StringBuilder StringBuilder;

	private int numTabs;
	private string tabs;
	private string tabsReplacement;
	private bool skipTabs;

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
	{
		AppendTabs(false);
		StringBuilder.Append(lineBreakRegex.Replace(text, tabsReplacement));
	}

	public void AppendLine()
	{
		AppendTabs(true);
		StringBuilder.AppendLine();
	}

	public void AppendLine(string text)
	{
		AppendTabs(true);
		StringBuilder.AppendLine(lineBreakRegex.Replace(text, tabsReplacement));
	}

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

	private void AppendTabs(bool resetTabs)
	{
		if (!skipTabs) {
			StringBuilder.Append(tabs);
		}

		skipTabs = !resetTabs;
	}
}
