using System.Text;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	internal sealed class SubsystemMethodWriter : MethodWriter
	{
		public override string AttributeName { get; } = "Dissonance.Engine.SubsystemAttribute";

		public override bool WriteCall(GeneratorExecutionContext context, CodeWriter code, MethodPair methodPair)
		{
			bool hasErrors = false;
			string parametersCode = SubsystemParameterHandling.HandleParameters(null, context, methodPair, ref hasErrors);

			code.AppendLine($"{methodPair.Symbol.Name}({parametersCode});");

			return !hasErrors;
		}
	}
}
