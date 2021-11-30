using System.Text;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	public sealed class MessageSubsystemWriter : BaseSubsystemWriter
	{
		private struct WriterData
		{
			public bool MessageContainsEntity;
			public ParameterData MessageParameter;
		}

		private WriterData writerData;

		public override string AttributeName { get; } = "Dissonance.Engine.MessageSubsystemAttribute";

		public override void WriteData(SubsystemData data, ref bool hasErrors)
		{
			writerData = new();

			base.WriteData(data, ref hasErrors);

			if (data.Parameters.Length == 0) {
				//TODO: Report error
				hasErrors = true;

				return;
			}

			var messageParameterData = data.Parameters[0];
			string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

			data.InvocationCode.AppendLine($"foreach (var message in ReadMessages<{messageParameterData.Parameter.Type.ToDisplayString()}>()) {{");
			data.InvocationCode.Indent();

			data.InvocationCode.AppendCode(data.ArgumentCheckCode);

			data.InvocationCode.AppendLine($"{data.Method.Symbol.Name}({argumentsCode});");

			data.InvocationCode.Unindent();
			data.InvocationCode.AppendLine("}");
		}

		public override IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			
			yield return MessageParameterHandler;

			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;

			yield return EntityComponentParameterHandler;
		}

		private void MessageParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
		{
			if (parameterData.ParameterIndex == 0) {
				parameterData.ArgumentCode.Append("message");

				writerData.MessageParameter = parameterData;

				var messageType = parameterData.Parameter.Type;
				var messageMembers = messageType.GetMembers();

				writerData.MessageContainsEntity = messageMembers.Any(symbol => {
					if (symbol.Name != "Entity") {
						return false;
					}

					ITypeSymbol? memberType = symbol switch {
						IFieldSymbol field => field.Type,
						IPropertySymbol property => property.Type,
						_ => null
					};

					return memberType != null && memberType.GetFullName() == "Dissonance.Engine.Entity";
				});

				handled = true;
			}
		}

		private void EntityComponentParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
		{
			var parameter = parameterData.Parameter;

			if (parameter.GetAttributes().Any(a => a.AttributeClass?.GetFullName() == "Dissonance.Engine.FromEntityAttribute")) {
				if (!writerData.MessageContainsEntity) {
					hasErrors = true;

					parameterData.SubsystemData.ReportDiagnostic(
						DiagnosticRules.InvalidMessageEntityParameter,
						parameterData.SubsystemData.Method.Symbol.Name,
						parameter.Name,
						writerData.MessageParameter.Parameter.Type.Name
					);

					return;
				}

				parameterData.ArgumentCode.Append($"message.Entity.Get<{parameter.Type.ToDisplayString()}>()");

				handled = true;

				var checkCode = parameterData.SubsystemData.ArgumentCheckCode;

				checkCode.AppendLine($"if (!message.Entity.Has<{parameter.Type.ToDisplayString()}>()) {{");
				checkCode.Indent();

				checkCode.AppendLine("continue;");

				checkCode.Unindent();
				checkCode.AppendLine("}");
				checkCode.AppendLine();
			}
		}

		/*public override bool WriteCall(GeneratorExecutionContext context, CodeWriter code, MethodPair methodPair)
		{
			bool MessageParameterHandler(ParameterHandling.ReportDiagnosticDelegate reportDiagnostic, IParameterSymbol parameter, StringBuilder callBuilder, ref bool hasErrors)
			{
				if (parameter.Type.IsValueType) {
					callBuilder.Append($"entity.Get<{parameter.Type.ToDisplayString()}>()");

					// Temporary checks.
					code.AppendLine($"if (!entity.Has<{parameter.Type.ToDisplayString()}>()) {{");
					code.Indent();

					code.AppendLine("continue;");

					code.Unindent();
					code.AppendLine("}");
					code.AppendLine();

					return true;
				}

				return false;
			}

			bool hasErrors = false;
			string parametersCode = ParameterHandling.HandleParameters(context, methodPair, ref hasErrors, ParameterHandlers);
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
		}*/
	}
}
