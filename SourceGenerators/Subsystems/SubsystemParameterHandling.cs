using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	internal static class SubsystemParameterHandling
	{
		private static readonly Dictionary<string, ComponentSource> ComponentSourceByAttributeName = new() {
			{ "Dissonance.Engine.FromEntityAttribute", ComponentSource.Entity },
			{ "Dissonance.Engine.FromWorldAttribute", ComponentSource.World },
			{ "Dissonance.Engine.FromGlobalAttribute", ComponentSource.Global },
		};

		public static string HandleParameters(ComponentSource? defaultComponentSource, GeneratorExecutionContext context, MethodPair methodPair, ref bool hasErrors, Action<IParameterSymbol, ComponentSource> callback = null)
		{
			var parametersCode = new StringBuilder();
			bool insertComma = false;

			foreach (var parameter in methodPair.Symbol.Parameters) {
				if (insertComma) {
					parametersCode.Append(", ");
				} else {
					insertComma = true;
				}

				switch (parameter.RefKind) {
					case RefKind.In:
						parametersCode.Append("in ");
						break;
					case RefKind.Ref:
						parametersCode.Append("ref ");
						break;
					case RefKind.Out:
						hasErrors = true;

						context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.InvalidSubsystemParameter, methodPair.Syntax.GetLocation()));
						break;
				}

				if (parameter.Type.GetFullName() == "Dissonance.Engine.World") {
					parametersCode.Append("World");
					continue;
				}

				if (parameter.Type.GetFullName() == "Dissonance.Engine.Entity") {
					parametersCode.Append("entity");
					continue;
				}

				ComponentSource? componentSource = null;
				bool multipleSources = false;

				foreach (var parameterAttribute in parameter.GetAttributes()) {
					var attributeClass = parameterAttribute.AttributeClass;

					if (attributeClass != null && ComponentSourceByAttributeName.TryGetValue(attributeClass.GetFullName(), out var attributeSource)) {
						if (componentSource.HasValue) {
							multipleSources = true;
						} else {
							componentSource = attributeSource;
						}
					}
				}

				componentSource ??= defaultComponentSource;

				if (multipleSources || !componentSource.HasValue) {
					hasErrors = true;

					context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.UnknownSubsystemParameterSource, methodPair.Syntax.GetLocation(), parameter.Name));
				}

				switch (componentSource) {
					case ComponentSource.Entity:
						parametersCode.Append($"entity.Get<{parameter.Type.ToDisplayString()}>()");
						break;
					case ComponentSource.World:
						parametersCode.Append($"WorldGet<{parameter.Type.ToDisplayString()}>()");
						break;
					case ComponentSource.Global:
						parametersCode.Append($"GlobalGet<{parameter.Type.ToDisplayString()}>()");
						break;
					case null:
						if (parameter.RefKind == RefKind.None) {
							parametersCode.Append("default");
						} else {
							parametersCode.Append($"Unsafe.NullRef<{parameter.Type.ToDisplayString()}>()");
						}

						break;
				}

				callback?.Invoke(parameter, componentSource.Value);
			}

			return parametersCode.ToString();
		}
	}
}
