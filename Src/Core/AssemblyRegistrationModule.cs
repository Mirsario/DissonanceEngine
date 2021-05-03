using System;
using System.Reflection;

namespace Dissonance.Engine
{
	public sealed class AssemblyRegistrationModule : EngineModule
	{
		public delegate void AssemblyRegistrationCallback(Assembly assembly, Type[] types);

		public static event AssemblyRegistrationCallback OnAssemblyRegistered;

		protected override void Init()
		{
			var appDomain = AppDomain.CurrentDomain;

			appDomain.AssemblyLoad += (sender, args) => {
				var assembly = args.LoadedAssembly;

				OnAssemblyRegistered?.Invoke(assembly, assembly.GetTypes());
			};

			foreach(var assembly in appDomain.GetAssemblies()) {
				OnAssemblyRegistered?.Invoke(assembly, assembly.GetTypes());
			}
		}
	}
}
