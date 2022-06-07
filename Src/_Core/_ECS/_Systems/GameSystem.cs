namespace Dissonance.Engine;

public abstract class GameSystem
{
	private bool initialized;

	protected internal SystemTypeData TypeData { get; }

	protected GameSystem()
	{
		TypeData = SystemManager.GetSystemTypeData(GetType());
	}

	protected virtual void Initialize() { }

	protected virtual void Execute() { }

	public void Update()
	{
		if (!initialized) {
			Initialize();

			initialized = true;
		}

		Execute();
	}
}
