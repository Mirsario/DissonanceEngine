using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	[Generator]
	public sealed partial class SubsystemGenerator : ISourceGenerator
	{
		private static readonly Dictionary<string, MethodWriter> SubsystemMethodWritersByAttributeFullName = new();
		private static readonly MethodWriter[] SubsystemMethodWriters = new MethodWriter[] {
			new SubsystemMethodWriter(),
			new EntitySubsystemMethodWriter(),
			new MessageSubsystemMethodWriter(),
		};

		static SubsystemGenerator()
		{
			foreach (var writer in SubsystemMethodWriters) {
				SubsystemMethodWritersByAttributeFullName.Add(writer.AttributeName, writer);
			}
		}

		public void Initialize(GeneratorInitializationContext context)
		{
			// Register a syntax receiver that will be created for each generation pass
			context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			// Retrieve the populated receiver 
			if (context.SyntaxContextReceiver is not SyntaxReceiver receiver) {
				return;
			}

			foreach (var (systemPair, systemMethods) in receiver.SystemTypes) {
				string systemNamespace = systemPair.Symbol.GetNamespace();
				bool foundSubsystems = false;
				bool errorsDetected = false;
				var subsystemMethods = new List<(MethodPair methodPair, MethodWriter writer)>();

				foreach (var systemMethodPair in systemMethods) {
					MethodWriter? methodWriter = null;
					bool skipMethod = false;

					foreach (var attributeSymbol in systemMethodPair.Symbol.GetAttributes()) {
						string? fullName = attributeSymbol.AttributeClass?.GetFullName();

						if (fullName == null) {
							continue;
						}

						if (!SubsystemMethodWritersByAttributeFullName.TryGetValue(fullName, out var newWriter)) {
							continue;
						}

						if (methodWriter == null) {
							methodWriter = newWriter;
						} else {
							var location = systemMethodPair.Syntax.GetLocation();

							context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.MultipleSubsystemAttributes, location));

							skipMethod = true;
						}
					}

					if (skipMethod || methodWriter == null) {
						continue;
					}

					foundSubsystems = true;

					if (!systemMethodPair.Syntax.Modifiers.Any(t => t.Text == "partial")) {
						var location = systemMethodPair.Syntax.GetLocation();

						context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.MissingPartialOnSubsystemMethod, location));
						errorsDetected = true;
					}

					subsystemMethods.Add((systemMethodPair, methodWriter));
				}

				if (!foundSubsystems) {
					continue;
				}

				if (!systemPair.Syntax.Modifiers.Any(t => t.Text == "partial")) {
					var location = systemPair.Syntax.GetLocation();

					context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.MissingPartialOnSystemClass, location));
					errorsDetected = true;
				}

				if (errorsDetected) {
					continue;
				}

				//TODO: Check if 'partial' is present.
				//if (!systemTypeSymbol.GetTypeArgumentCustomModifiers(or

				var code = new CodeWriter();

				code.AppendLine($"using System.Runtime.CompilerServices;");
				code.AppendLine();

				code.AppendLine($"namespace {systemNamespace}");
				code.AppendLine($"{{");
				code.Indent();

				code.AppendLine($"partial {systemPair.Symbol.TypeKind.ToString().ToLower()} {systemPair.Symbol.Name}");
				code.AppendLine($"{{");
				code.Indent();

				code.AppendLine($"protected internal override void FixedUpdate()");
				code.AppendLine($"{{");
				code.Indent();

				bool insertNewLine = false;

				foreach (var (subsystemMethod, methodWriter) in subsystemMethods) {
					if (insertNewLine) {
						code.AppendLine();
					} else {
						insertNewLine = true;
					}

					code.AppendLine($"// {subsystemMethod.Symbol.Name}");

					if (!methodWriter.WriteCall(context, code, subsystemMethod)) {
						errorsDetected = true;
					}
				}

				code.Unindent();
				code.AppendLine($"}}");

				foreach (var (subsystemMethod, methodWriter) in subsystemMethods) {
					code.AppendLine();

					string modifiersCode = string.Join(" ", subsystemMethod.Syntax.Modifiers.Select(m => m.ToString()));
					string parameterCode = string.Join(", ", subsystemMethod.Symbol.Parameters.Select(p => $"{p.ToDisplayString()} {p.Name}"));

					code.AppendLine($"[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]");
					code.AppendLine($"{modifiersCode} {subsystemMethod.Syntax.ReturnType} {subsystemMethod.Symbol.Name}({parameterCode});");
				}

				code.Unindent();
				code.AppendLine($"}}");

				code.Unindent();
				code.AppendLine($"}}");

				context.AddSource($"{systemPair.Symbol.Name}.Generated.cs", SourceText.From(code.ToString(), Encoding.UTF8));
			}
		}
	}
}
