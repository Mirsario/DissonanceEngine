using System.Collections.Generic;

namespace Dissonance.Engine
{
	public abstract class CallbackSystem : GameSystem
	{
		internal readonly List<GameSystem> InvocationList = new();

		protected sealed override void Execute()
		{
			foreach (var subscriber in InvocationList) {
				subscriber.Update();
			}
		}
	}
}
