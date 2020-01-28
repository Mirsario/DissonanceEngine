using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine.Core
{
	public static class DllResolver
	{
		private static readonly string[] EmbeddedAssemblies = {
			"System.Text.Encoding.CodePages",
			"System.Runtime.CompilerServices.Unsafe",
			"NVorbis",
			"Ionic.Zip",
			"Newtonsoft.Json",
			"BulletSharp",
		};

		private static Dictionary<string,Assembly> assemblyCache;
		private static bool initCalled;

		public static void Init()
		{
			if(initCalled) {
				return;
			}

			initCalled = true;

			assemblyCache = new Dictionary<string,Assembly>();

			static bool TryGetAssembly(string assemblyName,string argsName,out Assembly assembly)
			{
				if(assemblyName==argsName || argsName.StartsWith(assemblyName+",")) {
					try {
						assembly = assemblyCache[argsName] = Assembly.Load((byte[])Properties.Resources.ResourceManager.GetObject(assemblyName));

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

			AppDomain.CurrentDomain.AssemblyResolve += (obj,args) => {
				string argsName = args.Name;

				Console.Write(argsName+"... ");

				if(assemblyCache.TryGetValue(argsName,out var assembly)) {
					Console.WriteLine("Success!");

					return assembly;
				}

				for(int i = 0;i<EmbeddedAssemblies.Length;i++) {
					if(TryGetAssembly(EmbeddedAssemblies[i],argsName,out assembly)) {
						Console.WriteLine("Success!");

						return assembly;
					}
				}

				Console.WriteLine("Fail.");

				return null;
			};
		}
	}
}
