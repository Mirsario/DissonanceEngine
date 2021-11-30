namespace SourceGenerators.Subsystems
{
	public interface ISubsystemWriter
	{
		string AttributeName { get; }

		void WriteData(SubsystemData subsystemData, ref bool hasErrors);
	}
}
