using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	public class SystemData
	{
		public readonly GeneratorExecutionContext GeneratorContext;
		public readonly List<string> Members = new();
		public readonly CodeWriter InitCode = new();
		public readonly CodeWriter UpdateCode = new();
		public readonly List<SubsystemData> Subsystems = new();

		public SystemData(GeneratorExecutionContext generatorContext)
		{
			GeneratorContext = generatorContext;
		}
	}
}
