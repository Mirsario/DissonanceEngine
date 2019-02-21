using System;

namespace AbyssCrusaders.UI.Menu
{
	public abstract class MenuState : IDisposable
	{
		public MenuController controller;

		public MenuState()
		{
			OnActivated();
		}

		public virtual void OnActivated() {}
		public virtual void OnGUI() {}
		public virtual void Dispose() {}
	}
}
