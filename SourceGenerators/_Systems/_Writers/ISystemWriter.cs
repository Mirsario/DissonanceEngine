namespace SourceGenerators;

public interface ISystemWriter
{
	string AttributeName { get; }

	void WriteData(SystemData systemData, ref bool hasErrors);
}
