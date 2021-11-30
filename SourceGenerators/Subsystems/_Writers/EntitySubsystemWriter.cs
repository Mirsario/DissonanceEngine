using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	public sealed class EntitySubsystemWriter : BaseSubsystemWriter
	{
		public override string AttributeName { get; } = "Dissonance.Engine.EntitySubsystemAttribute";

		public override void WriteData(SubsystemData data, ref bool hasErrors)
		{
			base.WriteData(data, ref hasErrors);

			string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

			data.InvocationCode.AppendLine("foreach (var entity in ReadEntities()) {");
			data.InvocationCode.Indent();

			data.InvocationCode.AppendCode(data.ArgumentCheckCode);

			data.InvocationCode.AppendLine($"{data.Method.Symbol.Name}({argumentsCode});");

			data.InvocationCode.Unindent();
			data.InvocationCode.AppendLine("}");
		}

		public override IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return EntityParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;
			yield return EntityComponentParameterHandler;
		}

		public static void EntityParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
		{
			var parameter = parameterData.Parameter;

			if (parameter.Type.GetFullName() == "Dissonance.Engine.Entity") {
				parameterData.ArgumentCode.Append("entity");

				handled = true;
			}
		}

		private static void EntityComponentParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
		{
			var parameter = parameterData.Parameter;

			if (parameter.Type.IsValueType || true) {
				parameterData.ArgumentCode.Append($"entity.Get<{parameter.Type.ToDisplayString()}>()");

				handled = true;

				// Temporary checks.

				var checkCode = parameterData.SubsystemData.ArgumentCheckCode;

				checkCode.AppendLine("// Temporary.");
				checkCode.AppendLine($"if (!entity.Has<{parameter.Type.ToDisplayString()}>()) {{");
				checkCode.Indent();

				checkCode.AppendLine("continue;");

				checkCode.Unindent();
				checkCode.AppendLine("}");
				checkCode.AppendLine();
			}
		}
	}
}
