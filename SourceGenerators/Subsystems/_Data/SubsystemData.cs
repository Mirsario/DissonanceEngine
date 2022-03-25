using System.Text;
using Microsoft.CodeAnalysis;
using SourceGenerators.Utilities;

namespace SourceGenerators.Subsystems
{
	public class SubsystemData
	{
		public readonly SystemData SystemData;
		public readonly MethodPair Method;
		public readonly ParameterData[] Parameters;
		public readonly CodeWriter InvocationCode = new();
		public readonly List<string> ExecutionPredicates = new();

		public SubsystemData(SystemData systemData, MethodPair method)
		{
			SystemData = systemData;
			Method = method;

			Parameters = new ParameterData[method.Symbol.Parameters.Length];

			for (int i = 0; i < Parameters.Length; i++) {
				Parameters[i] = new(this, method.Symbol.Parameters[i], i);
			}
		}

		public void ReportDiagnostic(DiagnosticDescriptor descriptor, params object[] messageArgs)
		{
			SystemData.GeneratorContext.ReportDiagnostic(Diagnostic.Create(descriptor, Method.Syntax.GetLocation(), messageArgs));
		}
	}
}
