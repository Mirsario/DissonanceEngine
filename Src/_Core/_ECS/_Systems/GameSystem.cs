namespace Dissonance.Engine
{
	public abstract class GameSystem
	{
		private bool initialized;

		public World World { get; internal set; }

		protected internal SystemTypeData TypeData { get; }

		protected GameSystem()
		{
			TypeData = SystemManager.GetSystemTypeData(GetType());
		}

		protected virtual void Initialize() { }

		protected virtual void Execute() { }

		/// <summary>
		/// Initializes (if needed) and executes this system.
		/// </summary>
		public void Update()
		{
			if (!initialized) {
				Initialize();

				initialized = true;
			}

			Execute();
		}

		protected void SendMessage<T>(in T message) where T : struct
			=> World.SendMessage(message);

		protected MessageEnumerator<T> ReadMessages<T>() where T : struct
			=> World.ReadMessages<T>();
	}
}
