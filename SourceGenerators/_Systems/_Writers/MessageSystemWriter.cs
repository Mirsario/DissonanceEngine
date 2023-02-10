using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators;

public sealed class MessageSystemWriter : ISystemWriter
{
	private class WriterData
	{
		public bool MessageContainsEntity;
		public ParameterData? MessageParameter;
	}

	private delegate void SubsystemParameterHandlerWithWriteData(ParameterData parameterData, WriterData writerData, ref bool hasErrors, ref bool handled);

	public string AttributeName { get; } = "Dissonance.Engine.MessageSystemAttribute";

	public void WriteData(SystemData data, ref bool hasErrors)
	{
		if (data.Parameters.Length == 0) {
			//TODO: Report error
			hasErrors = true;

			return;
		}

		var writerData = new WriterData();

		SystemParameterHandler WrapParameterHandler(SubsystemParameterHandlerWithWriteData handler)
			=> (ParameterData parameterData, ref bool hasErrors, ref bool handled) => handler(parameterData, writerData, ref hasErrors, ref handled);

		IEnumerable<SystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;

			yield return WrapParameterHandler(MessageParameterHandler);
			yield return WrapParameterHandler(EntityParameterHandler);

			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;

			yield return WrapParameterHandler(EntityComponentParameterHandler);
		}

		SystemWriter.WriteParameters(data, ref hasErrors, GetParameterHandlers());

		SystemWriter.WriteReturnWrap(data, () => {
			WorldSystemWriter.WriteWorldEnumerationWrap(data, () => {
				WriteMessageEnumerationWrap(data, () => {
					SystemWriter.WritePredicatesWrap(data, () => {
						SystemWriter.WriteInvocation(data);
					});
				});
			});
		});
	}

	private static void WriteMessageEnumerationWrap(SystemData data, Action innerAction)
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

			parameterData.SystemData.ReportDiagnostic(
				DiagnosticRules.InvalidMessageEntityParameter,
				parameterData.SystemData.Method.Symbol.Name,
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

			parameterData.SystemData.ReportDiagnostic(
				DiagnosticRules.InvalidMessageEntityParameter,
				parameterData.SystemData.Method.Symbol.Name,
				parameter.Name,
				writerData.MessageParameter?.Parameter.Type.Name ?? "null"
			);

			return;
		}

		parameterData.ArgumentCode.Append($"message.Entity.Get<{parameter.Type.ToDisplayString()}>()");
		parameterData.SystemData.ExecutionPredicates.Add($"message.Entity.Has<{parameter.Type.ToDisplayString()}>()");

		handled = true;
	}
}
