using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	[Generator]
	public sealed partial class SubsystemGenerator : ISourceGenerator
	{
		private static readonly Dictionary<string, ISubsystemWriter> SubsystemMethodWritersByAttributeFullName = new();
		private static readonly List<ISubsystemWriter> SubsystemWriters = new();

		static SubsystemGenerator()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
				if (type.IsAbstract || !typeof(ISubsystemWriter).IsAssignableFrom(type)) {
					continue;
				}

				var subsystemWriter = (ISubsystemWriter)Activator.CreateInstance(type);

				SubsystemWriters.Add(subsystemWriter);
				SubsystemMethodWritersByAttributeFullName.Add(subsystemWriter.AttributeName, subsystemWriter);
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
				var subsystemMethods = new List<(MethodPair methodPair, ISubsystemWriter writer)>();

				//Console.WriteLine($"Enumerating type system: {systemPair.Symbol.Name}");

				foreach (var systemMethodPair in systemMethods) {
					ISubsystemWriter? methodWriter = null;
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

				//Console.WriteLine($"Found subsystems: {foundSubsystems}");

				if (!foundSubsystems) {
					continue;
				}

				if (!systemPair.Syntax.Modifiers.Any(t => t.Text == "partial")) {
					var location = systemPair.Syntax.GetLocation();

					context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.MissingPartialOnSystemClass, location));
					errorsDetected = true;
				}

				/*if (errorsDetected) {
					continue;
				}*/

				var system = new SystemData(context);

				foreach (var (subsystemMethod, subsystemWriter) in subsystemMethods) {
					var subsystem = new SubsystemData(system, subsystemMethod);

					subsystemWriter.WriteData(subsystem, ref errorsDetected);

					system.Subsystems.Add(subsystem);
				}

				//TODO: Check if 'partial' is present.
				//if (!systemTypeSymbol.GetTypeArgumentCustomModifiers(or

				var code = new CodeWriter();

				code.AppendLine($"using System.Diagnostics.CodeAnalysis;");
				code.AppendLine($"using System.Runtime.CompilerServices;");
				code.AppendLine($"using Dissonance.Engine;");
				code.AppendLine();

				code.AppendLine($"namespace {systemNamespace}");
				code.AppendLine($"{{");
				code.Indent();

				code.AppendLine($"sealed partial {systemPair.Symbol.TypeKind.ToString().ToLower()} {systemPair.Symbol.Name}");
				code.AppendLine($"{{");
				code.Indent();

				foreach (var (memberCode, memberFlags) in system.Members.OrderBy(tuple => (int)tuple.flags)) {
					code.AppendLine(memberCode);
					code.AppendLine();
				}

				// Initialize()
				bool hasInitCode = system.InitCode.StringBuilder.Length > 0;

				code.AppendLine($"protected sealed override void Initialize() {(hasInitCode ? null : " { }")}");

				if (hasInitCode) {
					code.AppendLine($"{{");
					code.Indent();

					code.AppendCode(system.InitCode);

					code.Unindent();
					code.AppendLine($"}}");
				}

				code.AppendLine();

				// Execute()
				code.AppendLine($"protected sealed override void Execute()");
				code.AppendLine($"{{");
				code.Indent();

				bool insertNewLine = false;

				foreach (var subsystem in system.Subsystems) {
					if (insertNewLine) {
						code.AppendLine();
					} else {
						insertNewLine = true;
					}

					code.AppendLine($"// {subsystem.Method.Symbol.Name}");
					code.AppendCode(subsystem.InvocationCode);
				}

				code.Unindent();
				code.AppendLine($"}}");

				foreach (var (subsystemMethod, methodWriter) in subsystemMethods) {
					code.AppendLine();

					string modifiersCode = string.Join(" ", subsystemMethod.Syntax.Modifiers.Select(m => m.ToString()));
					string parameterCode = string.Join(", ", subsystemMethod.Symbol.Parameters.Select(p => $"{p.ToDisplayString()} {p.Name}"));

					code.AppendLine($@"[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]");
					code.AppendLine($@"[SuppressMessage(""Performance"", ""CA1822: Mark members as static"", Justification = ""Method will be inlined"")]");
					code.AppendLine($@"{modifiersCode} {subsystemMethod.Syntax.ReturnType} {subsystemMethod.Symbol.Name}({parameterCode});");
				}

				code.Unindent();
				code.AppendLine($"}}");

				code.Unindent();
				code.AppendLine($"}}");

				try {
					context.AddSource($"{systemPair.Symbol.Name}.Generated.cs", SourceText.From(code.ToString(), Encoding.UTF8));
				}
				catch (ArgumentException) { }
			}
		}
	}
}
