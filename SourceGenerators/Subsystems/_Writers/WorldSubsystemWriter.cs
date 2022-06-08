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
		// Create an entity set
		string worldComponentSetName = $"worldComponentSet{data.Method.Symbol.Name}";

		data.SystemData.Members.Add(($"private static readonly ComponentSet {worldComponentSetName} = new ComponentSet();", MemberFlag.Field | MemberFlag.Private | MemberFlag.ReadOnly));

		//data.SystemData.InitCode.Append($"{componentSetName} = world.GetEntitySet(e => {string.Join(" && ", writerData.RequiredComponentTypes.Select(t => $"e.Has<{t}>()"))});");

		string readWorldsCall = $"WorldManager.GetWorldSet({worldComponentSetName}).ReadWorlds()";

		data.InvocationCode.AppendLine($"foreach (var world in {readWorldsCall}) {{");
		data.InvocationCode.Indent();

		innerCall();

		data.InvocationCode.Unindent();
		data.InvocationCode.AppendLine($"}}");
	}
}
