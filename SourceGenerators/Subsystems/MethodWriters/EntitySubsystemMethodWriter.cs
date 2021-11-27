using System.Text;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	internal sealed class EntitySubsystemMethodWriter : MethodWriter
	{
		public override string AttributeName { get; } = "Dissonance.Engine.EntitySubsystemAttribute";

		public override bool WriteCall(GeneratorExecutionContext context, CodeWriter code, MethodPair methodPair)
		{
			void WriteComponentParameterChecks(IParameterSymbol parameter, ComponentSource componentSource)
			{
				code.AppendLine($"if (!entity.Has<{parameter.Type.ToDisplayString()}>()) {{");
				code.Indent();

				code.AppendLine("continue;");

				code.Unindent();
				code.AppendLine("}");
				code.AppendLine();
			}

			bool hasErrors = false;

			code.AppendLine("foreach (var entity in ReadEntities()) {");
			code.Indent();

			string parametersCode = SubsystemParameterHandling.HandleParameters(ComponentSource.Entity, context, methodPair, ref hasErrors, WriteComponentParameterChecks);

			code.AppendLine($"{methodPair.Symbol.Name}({parametersCode});");

			code.Unindent();
			code.AppendLine("}");

			return !hasErrors;
		}
	}
}
