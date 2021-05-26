namespace Dissonance.Engine
{
	public abstract class GameSystem
	{
		public World World { get; internal set; }

		public virtual void Initialize() { }
		public virtual void FixedUpdate() { }
		public virtual void RenderUpdate() { }
	}
}
