using System;

namespace AbyssCrusaders.UI.Menu
{
	public abstract class MenuController : IDisposable
	{
		public MenuState currentState;
		
		public virtual void OnGUI() => currentState?.OnGUI();
		public virtual void Dispose() {}

		public void SetState<T>() where T : MenuState, new()
		{
			currentState?.Dispose();
			currentState = new T() {
				controller = this
			};
		}
	}
}
