using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems;

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

		foreach (string predicate in data.ExecutionPredicates) {
			data.InvocationCode.AppendLine($"if (!({predicate})) {{");
			data.InvocationCode.Indent();
			data.InvocationCode.AppendLine("continue;");
			data.InvocationCode.Unindent();
			data.InvocationCode.AppendLine("}");
			data.InvocationCode.AppendLine();
		}

		data.InvocationCode.AppendLine($"{data.Method.Symbol.Name}({argumentsCode});");

		data.InvocationCode.Unindent();
		data.InvocationCode.AppendLine("}");
	}

	public override IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
	{
		yield return CommonParameterHandlers.RefKindHandler;
		
		yield return MessageParameterHandler;
		yield return EntityParameterHandler;

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

	private void EntityParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.Type.GetFullName() == "Dissonance.Engine.Entity") {
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

			parameterData.ArgumentCode.Append("message.Entity");

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
			parameterData.SubsystemData.ExecutionPredicates.Add($"message.Entity.Has<{parameter.Type.ToDisplayString()}>()");

			handled = true;
		}
	}
}
