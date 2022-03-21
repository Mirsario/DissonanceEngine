using System;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public abstract class EngineModule : IDisposable
	{
		private bool preInitialized;
		private bool initialized;

		public DependencyInfo[] Dependencies { get; internal set; }
		public int DependencyIndex { get; internal set; }

		public Game Game => Game.Instance;

		public EngineModule()
		{
			Dependencies = GetType()
				.GetCustomAttributes<ModuleDependencyAttribute>(true)
				.Select(a => a.Info)
				.ToArray();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			OnDispose();
		}

		internal void InvokePreInitialize()
		{
			if (!preInitialized) {
				preInitialized = true;

				PreInit();
			}
		}

		internal void InvokeInitialize()
		{
			if (!initialized) {
				initialized = true;

				Init();
			}
		}

		internal void InvokeInitializeForAssembly(Assembly assembly)
			=> InitializeForAssembly(assembly);

		// Init

		protected virtual void PreInit() { }

		protected virtual void Init() { }

		protected virtual void InitializeForAssembly(Assembly assembly) { }

		// Fixed Update

		[VirtualMethodHook(typeof(EngineModuleHooks), nameof(EngineModuleHooks.PreFixedUpdate), false, true)]
		protected virtual void PreFixedUpdate() { }

		[VirtualMethodHook(typeof(EngineModuleHooks), nameof(EngineModuleHooks.FixedUpdate), false, true)]
		protected virtual void FixedUpdate() { }

		[VirtualMethodHook(typeof(EngineModuleHooks), nameof(EngineModuleHooks.PostFixedUpdate), false, true)]
		protected virtual void PostFixedUpdate() { }

		// Render Update

		[VirtualMethodHook(typeof(EngineModuleHooks), nameof(EngineModuleHooks.PreRenderUpdate), false, true)]
		protected virtual void PreRenderUpdate() { }

		[VirtualMethodHook(typeof(EngineModuleHooks), nameof(EngineModuleHooks.RenderUpdate), false, true)]
		protected virtual void RenderUpdate() { }

		[VirtualMethodHook(typeof(EngineModuleHooks), nameof(EngineModuleHooks.PostRenderUpdate), false, true)]
		protected virtual void PostRenderUpdate() { }

		// Etc

		protected virtual void OnDispose() { }
	}
}
