using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	public class SystemData
	{
		public readonly GeneratorExecutionContext GeneratorContext;
		public readonly CodeWriter InitCode = new();
		public readonly CodeWriter UpdateCode = new();
		public readonly List<SubsystemData> Subsystems = new();
		public readonly List<(string code, MemberFlag flags)> Members = new();

		public SystemData(GeneratorExecutionContext generatorContext)
		{
			GeneratorContext = generatorContext;
		}
	}
}
