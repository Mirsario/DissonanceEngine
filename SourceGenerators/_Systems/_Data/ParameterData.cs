using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGenerators;

public class ParameterData
{
	public readonly SystemData SystemData;
	public readonly IParameterSymbol Parameter;
	public readonly int ParameterIndex;
	public readonly StringBuilder ArgumentCode = new();

	public ParameterData(SystemData systemData, IParameterSymbol parameter, int parameterIndex)
	{
		SystemData = systemData;
		ParameterIndex = parameterIndex;
		Parameter = parameter;
	}
}
