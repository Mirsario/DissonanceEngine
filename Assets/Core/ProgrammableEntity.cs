namespace GameEngine
{
	public abstract class ProgrammableEntity //TODO: This is one really stupid name
	{
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		public virtual void OnGUI() {}
	}
}
