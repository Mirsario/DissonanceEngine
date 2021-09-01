using System;
using System.Collections.Generic;
using System.Reflection;
using Dissonance.Framework;

namespace Dissonance.Engine
{
	internal static class DllResolver
	{
		private static readonly string[] EmbeddedAssemblies = {
			"System.Text.Encoding.CodePages",
			"System.Runtime.CompilerServices.Unsafe",
			"NVorbis",
			"Ionic.Zip",
			"Newtonsoft.Json",
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
						using var stream = AssemblyCache.EngineAssembly.GetManifestResourceStream($"{nameof(Dissonance)}.{nameof(Engine)}.References.{assemblyName}.dll");
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
						//TODO: Unhardcode somehow..?
						if (assembly.FullName.StartsWith("BulletSharp,")) {
							DllMapResolver.SetForAssembly(assembly, $"{Assembly.GetExecutingAssembly().GetName().Name}.dll.config");
						}

						return assembly;
					}
				}

				return null;
			};
		}
	}
}
