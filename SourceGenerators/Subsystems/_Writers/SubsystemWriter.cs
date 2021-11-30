using Microsoft.CodeAnalysis;

namespace SourceGenerators.Subsystems
{
	public sealed class SubsystemWriter : BaseSubsystemWriter
	{
		public override string AttributeName { get; } = "Dissonance.Engine.SubsystemAttribute";

		public override void WriteData(SubsystemData data, ref bool hasErrors)
		{
			base.WriteData(data, ref hasErrors);

			string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

			data.InvocationCode.AppendLine($"{data.Method.Symbol.Name}({argumentsCode});");
		}

		public override IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;
		}
	}
}
