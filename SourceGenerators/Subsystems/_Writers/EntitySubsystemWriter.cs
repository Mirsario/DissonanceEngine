using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	public sealed class EntitySubsystemWriter : BaseSubsystemWriter
	{
		private struct WriterData
		{
			public List<string> RequiredComponentTypes = new();
		}

		private WriterData writerData;

		public override string AttributeName { get; } = "Dissonance.Engine.EntitySubsystemAttribute";

		public override void WriteData(SubsystemData data, ref bool hasErrors)
		{
			writerData = new();

			base.WriteData(data, ref hasErrors);

			string readEntitiesCall;

			if (writerData.RequiredComponentTypes.Count != 0) {
				// Create an entity set
				string entitySetName = $"entities{data.Method.Symbol.Name}";

				data.SystemData.Members.Add(($"private EntitySet {entitySetName};", MemberFlag.Field | MemberFlag.Private));

				data.SystemData.InitCode.Append($"{entitySetName} = World.GetEntitySet(e => {string.Join(" && ", writerData.RequiredComponentTypes.Select(t => $"e.Has<{t}>()"))});");

				readEntitiesCall = $"{entitySetName}.ReadEntities()";
			} else {
				readEntitiesCall = "World.ReadEntities()";
			}

			// Write update code

			string argumentsCode = string.Join(", ", data.Parameters.Select(p => p.ArgumentCode.ToString()));

			data.InvocationCode.AppendLine($"foreach (var entity in {readEntitiesCall}) {{");
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

		private void EntityComponentParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
		{
			var parameter = parameterData.Parameter;

			if (parameter.Type.IsValueType || true) {
				string parameterTypeName = parameter.Type.ToDisplayString();

				parameterData.ArgumentCode.Append($"entity.Get<{parameterTypeName}>()");

				writerData.RequiredComponentTypes.Add(parameterTypeName);

				handled = true;
			}
		}

		private static void EntityParameterHandler(ParameterData parameterData, ref bool hasErrors, ref bool handled)
		{
			var parameter = parameterData.Parameter;

			if (parameter.Type.GetFullName() == "Dissonance.Engine.Entity") {
				parameterData.ArgumentCode.Append("entity");

				handled = true;
			}
		}
	}
}
