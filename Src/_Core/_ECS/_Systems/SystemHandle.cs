namespace Dissonance.Engine;

public readonly struct SystemHandle
{
	public readonly uint Id;

	public bool IsValid => Id != 0;
	public ref readonly SystemDescription Description => ref SystemStorage.GetDescription(this);

	internal SystemHandle(uint id)
	{
		Id = id;
	}

	public override string ToString()
		=> $"System - {Description.Method.Name} (#{Id})";
}
