using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceGenerators.Utilities;

namespace SourceGenerators;

[Generator]
public sealed partial class SystemGenerator : ISourceGenerator
{
	private static readonly List<ISystemWriter> systemWriters = new();
	private static readonly Dictionary<string, ISystemWriter> systemMethodWritersByAttributeFullName = new();
	private static readonly HashSet<string> importedNamespaces = new() {
		"System.Diagnostics.CodeAnalysis",
		"System.Runtime.CompilerServices",
		"Dissonance.Engine",
	};

	static SystemGenerator()
	{
		ErrorLogging.Activate();

		foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
			if (type.IsAbstract || !typeof(ISystemWriter).IsAssignableFrom(type)) {
				continue;
			}

			var systemWriter = (ISystemWriter)Activator.CreateInstance(type);

			systemWriters.Add(systemWriter);
			systemMethodWritersByAttributeFullName.Add(systemWriter.AttributeName, systemWriter);
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

		foreach (var (typePair, systemAttributePairs) in receiver.SystemMethods) {
			string systemNamespace = typePair.Symbol.GetNamespace();
			var systems = new List<SystemData>();

			foreach (var (methodPair, systemAttribute) in systemAttributePairs) {
				// Check if the system method contains a 'partial' modifier.
				if (!methodPair.Syntax.Modifiers.Any(t => t.Text == "partial")) {
					var location = methodPair.Syntax.GetLocation();

					context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.MissingPartialOnSystemMethod, location));
					
					continue;
				}

				string? fullName = systemAttribute.AttributeClass?.GetFullName();

				if (fullName == null || !systemMethodWritersByAttributeFullName.TryGetValue(fullName, out var methodWriter)) {
					continue;
				}

				systems.Add(new SystemData(context, methodPair, methodWriter));
			}

			if (systems.Count == 0) {
				continue;
			}

			// Check if the system declaring type contains a 'partial' modifier.
			if (!typePair.Syntax.Modifiers.Any(t => t.Text == "partial")) {
				var location = typePair.Syntax.GetLocation();

				context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.MissingPartialOnSystemType, location));
				continue;
			}

			foreach (var system in systems) {
				var attributes = system.Method.Symbol.GetAttributes();

				if (!attributes.Any(a => a.AttributeClass is INamedTypeSymbol { IsGenericType: true } && a.AttributeClass?.ConstructUnboundGenericType().GetFullName() == "Dissonance.Engine.CalledInAttribute<T>")) {
					var location = system.Method.Syntax.GetLocation();
					
					context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.SystemContainsNoCallbacks, location, system.Method.Symbol.Name));
				}
			}
			
			bool hasErrors = false;

			foreach (var system in systems) {
				system.SystemWriter.WriteData(system, ref hasErrors);
			}

			var code = new CodeWriter();
			var typeSymbol = typePair.Symbol;

			foreach (string importedNamespace in importedNamespaces) {
				code.AppendLine($"using {importedNamespace};");
			}

			code.AppendLine();
			code.AppendLine($"namespace {systemNamespace};");
			code.AppendLine();

			code.AppendLine($"partial {typeSymbol.TypeKind.ToString().ToLower()} {typeSymbol.Name}");
			code.AppendLine($"{{");
			code.Indent();

			foreach (var system in systems) {
				foreach (var (memberCode, memberFlags) in system.Members.OrderBy(tuple => (int)tuple.flags)) {
					code.AppendLine(memberCode);
					code.AppendLine();
				}
			}

			code.AppendLine($"private static partial class Generated");
			code.AppendLine($"{{");
			code.Indent();

			var referenceContext = new ReferenceContext {
				ImportedNamespaces = importedNamespaces,
			};

			for (int i = 0; i < systems.Count; i++) {
				var system = systems[i];
				string generatedMethodName = system.Method.Symbol.Name;

				if (i != 0) {
					code.AppendLine();
				}

				var attributes = system.Method.Symbol.GetAttributes();

				foreach (var attribute in attributes) {
					if (attribute.AttributeClass == null) {
						continue;
					}

					string attributeReference = attribute.AttributeClass.GetReference(referenceContext);
					string? parentheses = null;

					if (attributeReference.EndsWith("Attribute")) {
						attributeReference = attributeReference.Substring(0, attributeReference.Length - 9);
					}

					if (!attribute.ConstructorArguments.IsEmpty || !attribute.NamedArguments.IsEmpty) {
						static string ValueToString(TypedConstant constant)
						{
							if (constant.IsNull) {
								return "null";
							}

							if (constant.Kind == TypedConstantKind.Array) {
								return $"new[] {{ {string.Join(", ", constant.Values.Select(ValueToString))} }}";
							}

							if (constant.Kind == TypedConstantKind.Enum) {
								return $"({constant.Type!.GetReference()}){constant.Value}";
							}

							if (constant.Value is string text) {
								return $@"""{text}""";
							}

							if (constant.Value is bool boolean) {
								return boolean ? "true" : "false";
							}

							return constant.Value!.ToString();
						}

						parentheses = string.Join(", ", Enumerable.Concat(
							attribute.ConstructorArguments.Select(ValueToString),
							attribute.NamedArguments.Select(a => $"{a.Key} = {ValueToString(a.Value)}")
						));

						parentheses = $"({parentheses})";
					}
					
					code.AppendLine($"[{attributeReference}{parentheses}]");
				}

				code.AppendLine($"private static SystemResult {generatedMethodName}()");
				code.AppendLine($"{{");
				code.Indent();
				
				code.AppendCode(system.InvocationCode);
				
				code.Unindent();
				code.AppendLine($"}}");
			}

			code.Unindent();
			code.AppendLine($"}}");

			foreach (var system in systems) {
				code.AppendLine();

				string modifiersCode = string.Join(" ", system.Method.Syntax.Modifiers.Select(m => m.ToString()));
				string parameterCode = string.Join(", ", system.Method.Symbol.Parameters.Select(p => $"{p.ToDisplayString()} {p.Name}"));

				code.AppendLine($@"[IgnoredSystem]");
				code.AppendLine($@"[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]");
				code.AppendLine($@"[SuppressMessage(""Performance"", ""CA1822: Mark members as static"", Justification = ""Method will be inlined"")]");
				code.AppendLine($@"{modifiersCode} {system.Method.Syntax.ReturnType} {system.Method.Symbol.Name}({parameterCode});");
			}

			code.Unindent();
			code.AppendLine($"}}");

			try {
				string baseFileNameWithoutExtension = $"{typeSymbol.Name}";
				string syntaxTreeFilePath = typePair.Syntax.SyntaxTree.FilePath;

				if (!string.IsNullOrEmpty(syntaxTreeFilePath)) {
					baseFileNameWithoutExtension = Path.GetFileNameWithoutExtension(syntaxTreeFilePath);
				}

				string fileName = $"{baseFileNameWithoutExtension}.Generated.cs";
				var sourceText = SourceText.From(code.ToString(), Encoding.UTF8);

				context.AddSource(fileName, sourceText);
			}
			catch (ArgumentException) { }
		}
	}
}
