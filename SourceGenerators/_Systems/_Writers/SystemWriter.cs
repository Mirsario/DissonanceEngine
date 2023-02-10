using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators;

public sealed class SystemWriter : ISystemWriter
{
	public string AttributeName { get; } = "Dissonance.Engine.SystemAttribute";
	
	public void WriteData(SystemData data, ref bool hasErrors)
	{
		static IEnumerable<SystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
		}

		WriteParameters(data, ref hasErrors, GetParameterHandlers());

		WriteReturnWrap(data, () => {
			WritePredicatesWrap(data, () => {
				WriteInvocation(data);
			});
		});
	}

	public static void WriteInvocation(SystemData data)
	{
		string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));
		
		data.InvocationCode.AppendLine($"{data.Method.Symbol.GetReference()}({argumentsCode});");
	}

	public static void WriteParameters(SystemData data, ref bool hasErrors, IEnumerable<SystemParameterHandler> parameterHandlers)
	{
		for (int i = 0; i < data.Parameters.Length; i++) {
			var parameterData = data.Parameters[i];
			var parameter = parameterData.Parameter;
			bool isHandled = false;

			foreach (var parameterHandler in parameterHandlers) {
				parameterHandler(parameterData, ref hasErrors, ref isHandled);

				if (isHandled) {
					break;
				}
			}

			if (!isHandled) {
				if (parameter.RefKind == RefKind.None) {
					parameterData.ArgumentCode.Append("default");
				} else {
					parameterData.ArgumentCode.Append($"Unsafe.NullRef<{parameter.Type.ToDisplayString()}>()");
				}

				parameterData.SystemData.ReportDiagnostic(DiagnosticRules.UnknownSystemParameterSource, parameter.Name);

				hasErrors = true;
			}
		}
	}

	public static void WritePredicatesWrap(SystemData data, Action innerCall)
	{
		if (data.ExecutionPredicates.Count != 0) {
			data.InvocationCode.Append($"if (");

			for (int i = 0; i < data.ExecutionPredicates.Count; i++) {
				data.InvocationCode.Append(data.ExecutionPredicates[i]);

				if (i < data.ExecutionPredicates.Count - 1) {
					data.InvocationCode.Append("\r\n");

					data.InvocationCode.Append("&& ");
				}
			}

			data.InvocationCode.AppendLine($") {{");
			data.InvocationCode.Indent();
		}

		innerCall();

		if (data.ExecutionPredicates.Count != 0) {
			data.InvocationCode.Unindent();
			data.InvocationCode.AppendLine($"}}");
		}
	}

	public static void WriteReturnWrap(SystemData data, Action innerCall)
	{
		string returnCode = "SystemResult.Completed";

		if (data.Method.Symbol.ReturnType.GetFullName() == "Dissonance.Engine.SystemResult") {
			returnCode = "result";

			data.InvocationCode.Append($"var {returnCode} = ");
		}

		innerCall();
		
		data.InvocationCode.AppendLine();
		data.InvocationCode.Append($"return {returnCode};");
	}
}
