using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	internal sealed class MessageSubsystemMethodWriter : MethodWriter
	{
		public override string AttributeName { get; } = "Dissonance.Engine.MessageSubsystemAttribute";

		public override bool WriteCall(GeneratorExecutionContext context, CodeWriter code, MethodPair methodPair)
		{
			bool hasErrors = false;
			string parametersCode = SubsystemParameterHandling.HandleParameters(null, context, methodPair, ref hasErrors);
			var parameters = methodPair.Symbol.Parameters;

			if (parameters.Length == 0) {
				// TODO: Raise an error..

				return false;
			}

			code.AppendLine($"foreach (var message in ReadMessages<{parameters[0].Type.ToDisplayString()}>()) {{");
			code.Indent();

			code.AppendLine($"{methodPair.Symbol.Name}({parametersCode});");

			code.Unindent();
			code.AppendLine("}");

			return !hasErrors;
		}
	}
}
