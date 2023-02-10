namespace Dissonance.Engine;

public readonly struct CallbackHandle
{
	public readonly uint Id;

	public bool IsValid => Id != 0;
	public ref readonly CallbackDescription Description => ref Data.Description;

	internal ref CallbackData Data => ref CallbackStorage.GetData(this);

	internal CallbackHandle(uint id)
	{
		Id = id;
	}

	public override string ToString()
		=> $"Callback - {Description.Type.Name} (#{Id})";
}
