using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators;

public class SystemData
{
	public readonly GeneratorExecutionContext GeneratorContext;
	public readonly MethodPair Method;
	public readonly ParameterData[] Parameters;
	public readonly ISystemWriter SystemWriter;
	public readonly CodeWriter InvocationCode = new();
	public readonly List<string> ExecutionPredicates = new();
	public readonly List<(string code, MemberFlag flags)> Members = new();

	public SystemData(GeneratorExecutionContext generatorContext, MethodPair method, ISystemWriter systemWriter)
	{
		GeneratorContext = generatorContext;
		Method = method;
		SystemWriter = systemWriter;

		Parameters = new ParameterData[method.Symbol.Parameters.Length];

		for (int i = 0; i < Parameters.Length; i++) {
			Parameters[i] = new(this, method.Symbol.Parameters[i], i);
		}
	}

	public void ReportDiagnostic(DiagnosticDescriptor descriptor, params object[] messageArgs)
	{
		GeneratorContext.ReportDiagnostic(Diagnostic.Create(descriptor, Method.Syntax.GetLocation(), messageArgs));
	}
}
