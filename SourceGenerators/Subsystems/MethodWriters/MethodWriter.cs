using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	internal abstract class MethodWriter
	{
		public abstract string AttributeName { get; }

		public abstract bool WriteCall(GeneratorExecutionContext context, CodeWriter code, MethodPair methodPair);
	}
}
