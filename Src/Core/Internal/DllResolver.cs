using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Dissonance.Framework;

namespace Dissonance.Engine.Core.Internal
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
			if(initCalled) {
				return;
			}

			initCalled = true;

			assemblyCache = new Dictionary<string, Assembly>();

			static bool TryGetAssembly(string assemblyName, string argsName, out Assembly assembly)
			{
				if(assemblyName == argsName || argsName.StartsWith(assemblyName + ",")) {
					try {
						var obj = Properties.Resources.ResourceManager.GetObject(assemblyName);
						var data = (byte[])obj;

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

				if(assemblyCache.TryGetValue(argsName, out var assembly)) {
					return assembly;
				}

				for(int i = 0; i < EmbeddedAssemblies.Length; i++) {
					if(TryGetAssembly(EmbeddedAssemblies[i], argsName, out assembly)) {
						//TODO: Move this somewhere, and find a way to unhardcode?
						if(assembly.FullName.StartsWith("BulletSharp,")) {
							NativeLibrary.SetDllImportResolver(assembly, (name, assembly, path) => {
								IntPtr pointer = IntPtr.Zero;

								var libraryNames = name switch
								{
									"libbulletc" => OSUtils.GetOS() switch
									{
										OSUtils.OS.Windows => new[] { "libbulletc.dll" },
										OSUtils.OS.Linux => new[] { "libbulletc.so" },
										OSUtils.OS.OSX => new[] { "libbulletc.dylib" },
										_ => null
									},
									_ => null
								};

								if(libraryNames == null) {
									return pointer;
								}

								var paths = new List<string>();

								for(int i = 0; i < DllManager.LibraryDirectories.Length; i++) {
									string libraryDirectory = DllManager.LibraryDirectories[i];

									for(int j = 0; j < libraryNames.Length; j++) {
										paths.Add(Path.Combine(libraryDirectory, libraryNames[j]));
									}
								}

								foreach(string currentPath in paths) {
									try {
										pointer = NativeLibrary.Load(currentPath, assembly, path);
									}
									catch { }

									if(pointer != IntPtr.Zero) {
										break;
									}
								}

								return pointer;
							});
						}

						return assembly;
					}
				}

				return null;
			};
		}
	}
}
