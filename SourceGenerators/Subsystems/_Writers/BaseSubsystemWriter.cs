using Microsoft.CodeAnalysis;

namespace SourceGenerators.Subsystems;

public abstract class BaseSubsystemWriter : ISubsystemWriter
{
	public abstract string AttributeName { get; }

	public virtual void WriteData(SubsystemData subsystemData, ref bool hasErrors)
	{
		for (int i = 0; i < subsystemData.Parameters.Length; i++) {
			var parameterData = subsystemData.Parameters[i];
			var parameter = parameterData.Parameter;
			bool isHandled = false;

			foreach (var parameterHandler in GetParameterHandlers()) {
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

	public virtual IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
	{
		yield break;
	}
}
