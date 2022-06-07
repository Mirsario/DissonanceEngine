using Microsoft.CodeAnalysis;

namespace SourceGenerators.Subsystems;

public sealed class SubsystemWriter : ISubsystemWriter
{
	public string AttributeName { get; } = "Dissonance.Engine.SubsystemAttribute";
	
	public void WriteData(SubsystemData data, ref bool hasErrors)
	{
		static IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
		}

		WriteParameters(data, ref hasErrors, GetParameterHandlers());

		WritePredicatesWrap(data, () => {
			WriteInvocation(data);
		});
	}

	public static void WriteInvocation(SubsystemData data)
	{
		string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

		data.InvocationCode.AppendLine($"{data.Method.Symbol.Name}({argumentsCode});");
	}

	public static void WriteParameters(SubsystemData subsystemData, ref bool hasErrors, IEnumerable<SubsystemParameterHandler> parameterHandlers)
	{
		for (int i = 0; i < subsystemData.Parameters.Length; i++) {
			var parameterData = subsystemData.Parameters[i];
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

				parameterData.SubsystemData.ReportDiagnostic(DiagnosticRules.UnknownSubsystemParameterSource, parameter.Name);

				hasErrors = true;
			}
		}
	}

	public static void WritePredicatesWrap(SubsystemData data, Action innerCall)
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
}
