using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGenerators.Subsystems
{
	public class ParameterData
	{
		public readonly SubsystemData SubsystemData;
		public readonly IParameterSymbol Parameter;
		public readonly int ParameterIndex;
		public readonly StringBuilder ArgumentCode = new();

		public ParameterData(SubsystemData subsystemData, IParameterSymbol parameter, int parameterIndex)
		{
			SubsystemData = subsystemData;
			ParameterIndex = parameterIndex;
			Parameter = parameter;
		}
	}
}
