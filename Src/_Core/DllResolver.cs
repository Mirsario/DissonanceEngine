using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;

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

		static DllResolver()
		{
			Init();
		}

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
							SetForAssembly(assembly, $"{Assembly.GetExecutingAssembly().Location}.config");

							//DllMapResolver.SetForAssembly(assembly, $"{Assembly.GetExecutingAssembly().Location}.config");
						}

						return assembly;
					}
				}

				return null;
			};
		}

		// This method implements a *barebones* dllmap resolver for the engine's native libraries.
		// It expects 'DissonanceEngine.dll.config' to be present next to the engine's dll.
		public static void SetForAssembly(Assembly assembly, string configPath = null)
		{
			var stringComparer = StringComparer.InvariantCultureIgnoreCase;

			string osString;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				osString = "windows";
			} else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				osString = "osx";
			} else {
				osString = "linux";
			}

			string cpuString = RuntimeInformation.OSArchitecture switch {
				Architecture.Arm => "arm",
				Architecture.Arm64 => "armv8",
				Architecture.X86 => "x86",
				_ => "x86-64",
			};

			string wordSizeString = RuntimeInformation.OSArchitecture switch {
				Architecture.X86 => "32",
				Architecture.Arm => "32",
				_ => "64",
			};

			bool StringNullOrEqual(string a, string b)
				=> a == null || stringComparer.Equals(a, b);

			NativeLibrary.SetDllImportResolver(assembly, (name, assembly, path) => {
				string usedConfigPath = configPath;

				if (configPath == null) {
					if (string.IsNullOrWhiteSpace(assembly.Location)) {
						usedConfigPath = $"{assembly.ManifestModule.ScopeName}.config";
					} else {
						usedConfigPath = $"{assembly.Location}.config";
					}
				}

				if (!File.Exists(usedConfigPath)) {
					return IntPtr.Zero;
				}

				XElement root = XElement.Load(usedConfigPath);

				var maps = root
					.Elements("dllmap")
					.Where(element => stringComparer.Equals(element.Attribute("dll")?.Value, name))
					.Where(element => StringNullOrEqual(element.Attribute("os")?.Value, osString))
					.Where(element => StringNullOrEqual(element.Attribute("cpu")?.Value, cpuString))
					.Where(element => StringNullOrEqual(element.Attribute("wordsize")?.Value, wordSizeString));

				var map = maps.SingleOrDefault();

				if (map == null) {
					throw new ArgumentException($"'{Path.GetFileName(usedConfigPath)}' - Found {maps.Count()} possible mapping candidates for dll '{name}'.");
				}

				string targetPath = map.Attribute("target").Value;
				string usedConfigDirectory = Path.GetDirectoryName(usedConfigPath);
				string nativeLibraryPath = Path.GetFullPath(Path.Combine(usedConfigDirectory, targetPath));

				return NativeLibrary.Load(nativeLibraryPath);
			});
		}
	}
}
