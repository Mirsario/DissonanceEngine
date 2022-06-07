using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems;

public sealed class MessageSubsystemWriter : ISubsystemWriter
{
	private class WriterData
	{
		public bool MessageContainsEntity;
		public ParameterData? MessageParameter;
	}

	private delegate void SubsystemParameterHandlerWithWriteData(ParameterData parameterData, WriterData writerData, ref bool hasErrors, ref bool handled);

	public string AttributeName { get; } = "Dissonance.Engine.MessageSubsystemAttribute";

	public void WriteData(SubsystemData data, ref bool hasErrors)
	{
		if (data.Parameters.Length == 0) {
			//TODO: Report error
			hasErrors = true;

			return;
		}

		var writerData = new WriterData();

		SubsystemParameterHandler WrapParameterHandler(SubsystemParameterHandlerWithWriteData handler)
			=> (ParameterData parameterData, ref bool hasErrors, ref bool handled) => handler(parameterData, writerData, ref hasErrors, ref handled);

		IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;

			yield return WrapParameterHandler(MessageParameterHandler);
			yield return WrapParameterHandler(EntityParameterHandler);

			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;

			yield return WrapParameterHandler(EntityComponentParameterHandler);
		}

		SubsystemWriter.WriteParameters(data, ref hasErrors, GetParameterHandlers());

		WorldSubsystemWriter.WriteWorldEnumerationWrap(data, () => {
			WriteMessageEnumerationWrap(data, () => {
				SubsystemWriter.WritePredicatesWrap(data, () => {
					SubsystemWriter.WriteInvocation(data);
				});
			});
		});
	}

	private static void WriteMessageEnumerationWrap(SubsystemData data, Action innerAction)
	{
		var messageParameterData = data.Parameters[0];
		string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

		data.InvocationCode.AppendLine($"foreach (var message in world.ReadMessages<{messageParameterData.Parameter.Type.ToDisplayString()}>()) {{");
		data.InvocationCode.Indent();

		innerAction();

		data.InvocationCode.Unindent();
		data.InvocationCode.AppendLine("}");
	}

	private static void MessageParameterHandler(ParameterData parameterData, WriterData writerData, ref bool hasErrors, ref bool handled)
	{
		if (parameterData.ParameterIndex != 0) {
			return;
		}
		
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

	private static void EntityParameterHandler(ParameterData parameterData, WriterData writerData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.Type.GetFullName() != "Dissonance.Engine.Entity") {
			return;
		}
		
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

	private static void EntityComponentParameterHandler(ParameterData parameterData, WriterData writerData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (!parameter.GetAttributes().Any(a => a.AttributeClass?.GetFullName() == "Dissonance.Engine.FromEntityAttribute")) {
			return;
		}

		if (!writerData.MessageContainsEntity) {
			hasErrors = true;

			parameterData.SubsystemData.ReportDiagnostic(
				DiagnosticRules.InvalidMessageEntityParameter,
				parameterData.SubsystemData.Method.Symbol.Name,
				parameter.Name,
				writerData.MessageParameter?.Parameter.Type.Name ?? "null"
			);

			return;
		}

		parameterData.ArgumentCode.Append($"message.Entity.Get<{parameter.Type.ToDisplayString()}>()");
		parameterData.SubsystemData.ExecutionPredicates.Add($"message.Entity.Has<{parameter.Type.ToDisplayString()}>()");

		handled = true;
	}
}
