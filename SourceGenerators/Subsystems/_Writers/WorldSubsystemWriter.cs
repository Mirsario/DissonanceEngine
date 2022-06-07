namespace SourceGenerators.Subsystems;

public sealed class WorldSubsystemWriter : ISubsystemWriter
{
	public string AttributeName { get; } = "Dissonance.Engine.WorldSubsystemAttribute";

	public void WriteData(SubsystemData data, ref bool hasErrors)
	{
		static IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;
		}

		SubsystemWriter.WriteParameters(data, ref hasErrors, GetParameterHandlers());

		WriteWorldEnumerationWrap(data, () => {
			SubsystemWriter.WritePredicatesWrap(data, () => {
				SubsystemWriter.WriteInvocation(data);
			});
		});
	}

	public static void WriteWorldEnumerationWrap(SubsystemData data, Action innerCall)
	{
		data.InvocationCode.AppendLine("foreach (var world in WorldManager.ReadWorlds()) {");
		data.InvocationCode.Indent();

		innerCall();

		data.InvocationCode.Unindent();
		data.InvocationCode.AppendLine($"}}");
	}
}
