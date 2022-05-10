namespace Dissonance.Engine;

public abstract class GameSystem
{
	private bool initialized;

	protected internal SystemTypeData TypeData { get; }

	protected GameSystem()
	{
		TypeData = SystemManager.GetSystemTypeData(GetType());
	}

	protected virtual void Initialize(World world) { }

	protected virtual void Execute(World world) { }

	/// <summary>
	/// Initializes (if needed) and executes this system.
	/// </summary>
	public void Update(World world)
	{
		if (!initialized) {
			Initialize(world);

			initialized = true;
		}

		Execute(world);
	}
}
