using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems;

public sealed class EntitySubsystemWriter : ISubsystemWriter
{
	public class WriterData
	{
		public List<string> RequiredComponentTypes = new();
	}

	public string AttributeName { get; } = "Dissonance.Engine.EntitySubsystemAttribute";

	public void WriteData(SubsystemData data, ref bool hasErrors)
	{
		var writerData = new WriterData();

		IEnumerable<SubsystemParameterHandler> GetParameterHandlers()
		{
			yield return CommonParameterHandlers.RefKindHandler;
			yield return CommonParameterHandlers.WorldParameterHandler;
			yield return EntityParameterHandler;
			yield return CommonParameterHandlers.GlobalComponentParameterHandler;
			yield return CommonParameterHandlers.WorldComponentParameterHandler;
			yield return (ParameterData parameterData, ref bool hasErrors, ref bool handled) => EntityComponentParameterHandler(parameterData, writerData, ref hasErrors, ref handled);
		}

		SubsystemWriter.WriteParameters(data, ref hasErrors, GetParameterHandlers());

		WorldSubsystemWriter.WriteWorldEnumerationWrap(data, () => {
			WriteEntityEnumerationWrap(data, writerData, () => {
				SubsystemWriter.WritePredicatesWrap(data, () => {
					SubsystemWriter.WriteInvocation(data);
				});
			});
		});
	}

	public static void WriteEntityEnumerationWrap(SubsystemData data, WriterData writerData, Action innerCall)
	{
		string readEntitiesCall;

		if (writerData.RequiredComponentTypes.Count != 0) {
			// Create an entity set
			string componentSetName = $"componentSet{data.Method.Symbol.Name}";
			string inclusions = $"{string.Join(string.Empty, writerData.RequiredComponentTypes.Select(componentType => $"\r\n\t.Include<{componentType}>()"))}";

			data.SystemData.Members.Add(($"private static readonly ComponentSet {componentSetName} = new ComponentSet(){inclusions};", MemberFlag.Field | MemberFlag.Private | MemberFlag.ReadOnly));

			//data.SystemData.InitCode.Append($"{componentSetName} = world.GetEntitySet(e => {string.Join(" && ", writerData.RequiredComponentTypes.Select(t => $"e.Has<{t}>()"))});");

			readEntitiesCall = $"world.GetEntitySet({componentSetName}).ReadEntities()";
		} else {
			readEntitiesCall = "world.ReadEntities()";
		}

		// Write update code

		string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

		data.InvocationCode.AppendLine($"foreach (var entity in {readEntitiesCall}) {{");
		data.InvocationCode.Indent();
		
		innerCall();

		data.InvocationCode.Unindent();
		data.InvocationCode.AppendLine("}");
	}

	public static void EntityComponentParameterHandler(ParameterData parameterData, WriterData writerData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.Type.IsValueType || true) {
			string parameterTypeName = parameter.Type.ToDisplayString();

			parameterData.ArgumentCode.Append($"entity.Get<{parameterTypeName}>()");

			writerData.RequiredComponentTypes.Add(parameterTypeName);

			handled = true;
		}
	}

	public static void EntityParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
	{
		var parameter = parameterData.Parameter;

		if (parameter.Type.GetFullName() == "Dissonance.Engine.Entity") {
			parameterData.ArgumentCode.Append("entity");

			handled = true;
		}
	}
}
