using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	internal static class DllResolver
	{
		//TODO: Unhardcode
		private static readonly string[] EmbeddedAssemblies = {
			"BulletSharp"
		};

		private static Dictionary<string, Assembly> assemblyCache;
		private static bool initCalled;

		public static void Init()
		{
			if (initCalled) {
				return;
			}

			initCalled = true;

			assemblyCache = new Dictionary<string, Assembly>();

			static bool TryGetAssembly(string assemblyName, string argsName, out Assembly assembly)
			{
				if (assemblyName == argsName || argsName.StartsWith(assemblyName + ",")) {
					try {
						using var stream = AssemblyCache.EngineAssembly.GetManifestResourceStream($"{nameof(Dissonance)}.{nameof(Engine)}.{assemblyName}.dll");
						byte[] data = new byte[stream.Length];

						stream.Read(data, 0, data.Length);

						assembly = assemblyCache[argsName] = Assembly.Load(data);

						return true;
					}
					catch(BadImageFormatException e) {
						Console.Write($"{e.GetType().Name}: {e.FusionLog}");
					}
					catch(Exception e) {
						Console.Write($"{e.GetType().Name}: {e.Message}");
					}
				}

				assembly = null;

				return false;
			}

			AppDomain.CurrentDomain.AssemblyResolve += (obj, args) => {
				string argsName = args.Name;

				if (assemblyCache.TryGetValue(argsName, out var assembly)) {
					return assembly;
				}

				for (int i = 0; i < EmbeddedAssemblies.Length; i++) {
					if (TryGetAssembly(EmbeddedAssemblies[i], argsName, out assembly)) {
						//TODO: Unhardcode
						if (assembly.FullName.StartsWith("BulletSharp,")) {
							throw new NotImplementedException();

							//DllMapResolver.SetForAssembly(assembly, $"{Assembly.GetExecutingAssembly().Location}.config");
						}

						return assembly;
					}
				}

				return null;
			};
		}
	}
}
