using System.Linq;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

#pragma warning disable IDE0060 // Remove unused parameter

namespace SourceGenerators;

public static class CommonParameterHandlers
{
	public static void RefKindHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;
		var argumentBuilder = parameterData.ArgumentCode;

		switch (parameter.RefKind) {
			case RefKind.In:
				argumentBuilder.Append("in ");
				break;
			case RefKind.Ref:
				argumentBuilder.Append("ref ");
				break;
			case RefKind.Out:
				hasErrors = true;
				parameterData.SystemData.ReportDiagnostic(DiagnosticRules.InvalidSystemParameter, parameter.Name);
				break;
		}
	}

	public static void WorldParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.Type.GetFullName() == "Dissonance.Engine.World") {
			parameterData.ArgumentCode.Append("world");

			handled = true;
		}
	}

	public static void GlobalComponentParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.GetAttributes().Any(a => a.AttributeClass?.GetFullName() == "Dissonance.Engine.FromGlobalAttribute")) {
			parameterData.SystemData.ExecutionPredicates.Add($"Global.Has<{parameter.Type.ToDisplayString()}>()");
			parameterData.ArgumentCode.Append($"Global.Get<{parameter.Type.ToDisplayString()}>()");

			handled = true;
		}
	}

	public static void WorldComponentParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.GetAttributes().Any(a => a.AttributeClass?.GetFullName() == "Dissonance.Engine.FromWorldAttribute")) {
			parameterData.SystemData.ExecutionPredicates.Add($"world.Has<{parameter.Type.ToDisplayString()}>()");
			parameterData.ArgumentCode.Append($"world.Get<{parameter.Type.ToDisplayString()}>()");

			handled = true;
		}
	}
}
