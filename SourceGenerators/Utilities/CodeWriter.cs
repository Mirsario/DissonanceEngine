using System.Text;

namespace SourceGenerators.Utilities
{
	internal sealed class CodeWriter
	{
		public readonly StringBuilder StringBuilder;

		private int numTabs;
		private string tabs;

		public CodeWriter() : this(new StringBuilder()) { }

		public CodeWriter(StringBuilder stringBuilder)
		{
			StringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
			tabs = string.Empty;
		}

		public override string ToString()
			=> StringBuilder.ToString();

		public void Indent()
		{
			tabs = new string('\t', ++numTabs);
		}

		public void Unindent()
		{
			if (numTabs <= 0) {
				throw new InvalidOperationException("Already at 0 tabs.");
			}

			tabs = new string('\t', --numTabs);
		}

		public void AppendLine()
			=> StringBuilder.AppendLine(tabs);

		public void AppendLine(string line)
			=> StringBuilder.AppendLine($"{tabs}{line}");
	}
}
