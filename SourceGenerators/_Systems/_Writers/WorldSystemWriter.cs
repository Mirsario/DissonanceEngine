using System;
using System.Collections.Generic;

namespace SourceGenerators;

public sealed class WorldSystemWriter : ISystemWriter
{
	public string AttributeName { get; } = "Dissonance.Engine.WorldSystemAttribute";

	public void WriteData(SystemData data, ref bool hasErrors)
	{
		static IEnumerable<SystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;
		}

		SystemWriter.WriteParameters(data, ref hasErrors, GetParameterHandlers());

		SystemWriter.WriteReturnWrap(data, () => {
			WriteWorldEnumerationWrap(data, () => {
				SystemWriter.WritePredicatesWrap(data, () => {
					SystemWriter.WriteInvocation(data);
				});
			});
		});
	}

	public static void WriteWorldEnumerationWrap(SystemData data, Action innerCall)
	{
		// Create an entity set
		string worldComponentSetName = $"worldComponentSet{data.Method.Symbol.Name}";

		data.Members.Add(($"private static readonly ComponentSet {worldComponentSetName} = new ComponentSet();", MemberFlag.Field | MemberFlag.Private | MemberFlag.ReadOnly));

		string readWorldsCall = $"WorldManager.GetWorldSet({worldComponentSetName}).ReadWorlds()";

		data.InvocationCode.AppendLine($"foreach (var world in {readWorldsCall}) {{");
		data.InvocationCode.Indent();

		innerCall();

		data.InvocationCode.Unindent();
		data.InvocationCode.AppendLine($"}}");
	}
}
