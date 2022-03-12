using System;

namespace Dissonance.Engine
{
	public class EngineModuleHooks
	{
		// Init
		public Action PreInit { get; private set; }
		public Action Init { get; private set; }
		// Fixed Update
		public Action PreFixedUpdate { get; private set; }
		public Action FixedUpdate { get; private set; }
		public Action PostFixedUpdate { get; private set; }
		// Render Update
		public Action PreRenderUpdate { get; private set; }
		public Action RenderUpdate { get; private set; }
		public Action PostRenderUpdate { get; private set; }
	}
}
