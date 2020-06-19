namespace Dissonance.Engine.Core.Modules
{
	public abstract class EngineModule
	{
		public virtual bool AutoLoad => true;

		public Game Game { get; internal set; }

		//Init
		
		[VirtualMethodHook(typeof(EngineModuleHooks),nameof(EngineModuleHooks.PreInit),false,true)]
		protected virtual void PreInit() { }
		
		[VirtualMethodHook(typeof(EngineModuleHooks),nameof(EngineModuleHooks.Init),false,true)]
		protected virtual void Init() { }
		
		//Fixed Update
		
		[VirtualMethodHook(typeof(EngineModuleHooks),nameof(EngineModuleHooks.PreFixedUpdate),false,true)]
		protected virtual void PreFixedUpdate() { }
		
		[VirtualMethodHook(typeof(EngineModuleHooks),nameof(EngineModuleHooks.PostFixedUpdate),false,true)]
		protected virtual void PostFixedUpdate() { }
		
		//Render Update
		
		[VirtualMethodHook(typeof(EngineModuleHooks),nameof(EngineModuleHooks.PreRenderUpdate),false,true)]
		protected virtual void PreRenderUpdate() { }

		[VirtualMethodHook(typeof(EngineModuleHooks),nameof(EngineModuleHooks.PostRenderUpdate),false,true)]
		protected virtual void PostRenderUpdate() { }
	}
}
