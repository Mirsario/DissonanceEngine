using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

#pragma warning disable IDE0060 // Remove unused parameter

namespace SourceGenerators.Subsystems;

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
				parameterData.SubsystemData.ReportDiagnostic(DiagnosticRules.InvalidSubsystemParameter, parameter.Name);
				break;
		}
	}

	public static void WorldParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.Type.GetFullName() == "Dissonance.Engine.World") {
			parameterData.ArgumentCode.Append("World");

			handled = true;
		}
	}

	public static void GlobalComponentParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.GetAttributes().Any(a => a.AttributeClass?.GetFullName() == "Dissonance.Engine.FromGlobalAttribute")) {
			parameterData.SubsystemData.ExecutionPredicates.Add($"Global.Has<{parameter.Type.ToDisplayString()}>()");
			parameterData.ArgumentCode.Append($"Global.Get<{parameter.Type.ToDisplayString()}>()");

			handled = true;
		}
	}

	public static void WorldComponentParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.GetAttributes().Any(a => a.AttributeClass?.GetFullName() == "Dissonance.Engine.FromWorldAttribute")) {
			parameterData.SubsystemData.ExecutionPredicates.Add($"World.Has<{parameter.Type.ToDisplayString()}>()");
			parameterData.ArgumentCode.Append($"World.Get<{parameter.Type.ToDisplayString()}>()");

			handled = true;
		}
	}
}
