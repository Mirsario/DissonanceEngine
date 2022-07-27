using Microsoft.CodeAnalysis;

namespace SourceGenerators.Subsystems;

public sealed class SubsystemWriter : BaseSubsystemWriter
{
	public override string AttributeName { get; } = "Dissonance.Engine.SubsystemAttribute";

	public override void WriteData(SubsystemData data, ref bool hasErrors)
	{
		base.WriteData(data, ref hasErrors);

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

		string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

		data.InvocationCode.AppendLine($"{data.Method.Symbol.Name}({argumentsCode});");

		if (data.ExecutionPredicates.Count != 0) {
			data.InvocationCode.Unindent();
			data.InvocationCode.AppendLine($"}}");
		}
	}

	public override IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
	{
		yield return CommonParameterHandlers.RefKindHandler;
		yield return CommonParameterHandlers.WorldParameterHandler;
		yield return CommonParameterHandlers.GlobalComponentParameterHandler;
		yield return CommonParameterHandlers.WorldComponentParameterHandler;
	}
}
