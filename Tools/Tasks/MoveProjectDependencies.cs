using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dissonance.BuildTools.Tasks
{
	/// <summary>
	/// The goal of this task is to organize output folders by moving both managed and native references into a subdirectory, modifying deps.json and runtimeconfig.json files in the process.
	/// </summary>
	public class MoveProjectDependencies : TaskBase
	{
		[Required]
		public string OutputPath { get; set; } = string.Empty;

		[Required]
		public string LibrariesDir { get; set; } = string.Empty;

		[Required]
		public string AssemblyName { get; set; } = string.Empty;

		// Is there a way to get these properties without making a parameter? Please PR an improvement if so.
		// -- Mirsario.
		[Required]
		public string ProjectDepsFilePath { get; set; } = string.Empty;

		[Required]
		public string ProjectRuntimeConfigFilePath { get; set; } = string.Empty;

		protected override void Run()
		{
			Log.LogMessage(MessageImportance.Low, $"Executing {nameof(MoveProjectDependencies)}...");

			if (!AddProbingPaths()) {
				Log.LogMessage(MessageImportance.High, $"'{Path.GetFileName(ProjectRuntimeConfigFilePath)}' is missing, skipping dependency reorganization.");
				return;
			}

			MoveFiles();
		}

		private bool AddProbingPaths()
		{
			if (!File.Exists(ProjectRuntimeConfigFilePath)) {
				return false;
			}

			var runtimeConfigJson = JObject.Parse(File.ReadAllText(ProjectRuntimeConfigFilePath));

			const string RuntimeOptionsKey = "runtimeOptions";
			const string AdditionalProbingPathsKey = "additionalProbingPaths";

			if (runtimeConfigJson[RuntimeOptionsKey] is not JObject runtimeOptionsObject) {
				runtimeConfigJson[RuntimeOptionsKey] = runtimeOptionsObject = new JObject();
			}

			if (runtimeOptionsObject[AdditionalProbingPathsKey] is not JArray additionalProbingPathsArray) {
				runtimeOptionsObject[AdditionalProbingPathsKey] = additionalProbingPathsArray = new JArray();
			}

			if (!additionalProbingPathsArray.Any(j => j is JValue jValue && jValue.Value is string jString && jString == LibrariesDir)) {
				additionalProbingPathsArray.Add(LibrariesDir);
			}

			File.WriteAllText(ProjectRuntimeConfigFilePath, runtimeConfigJson.ToString());

			return true;
		}

		private void MoveFiles()
		{
			if (!File.Exists(ProjectDepsFilePath)) {
				return;
			}

			var depsJson = JObject.Parse(File.ReadAllText(ProjectDepsFilePath));

			if (depsJson["targets"] is not JObject targetsObject) {
				return;
			}

			foreach (var targetPair in targetsObject) {
				if (targetPair.Value is not JObject targetObject) {
					continue;
				}

				foreach (var libraryPair in targetObject) {
					if (libraryPair.Value is not JObject libraryObject) {
						continue;
					}

					string[] libraryKeySplit = libraryPair.Key.Split('/');

					if (libraryKeySplit.Length != 2) {
						return;
					}

					string libraryName = libraryKeySplit[0];
					string libraryVersion = libraryKeySplit[1];

					if (libraryName == AssemblyName) {
						continue;
					}

					MoveManagedLibraries(libraryObject, libraryName, libraryVersion);
					MoveNativeLibraries(libraryObject, libraryName, libraryVersion);
				}
			}

			File.WriteAllText(ProjectDepsFilePath, depsJson.ToString());
		}

		private void MoveManagedLibraries(JObject libraryObject, string libraryName, string libraryVersion)
		{
			if (libraryObject["runtime"] is not JObject runtimeObject) {
				return;
			}

			List<(string keyOld, string keyNew)>? dllRenames = null;

			foreach (var dllPair in runtimeObject) {
				if (dllPair.Value is not JObject assemblyObject) {
					continue;
				}

				string dllKey = dllPair.Key;
				string dllFileName = Path.GetFileName(dllKey);
				string dllPath = Path.Combine(OutputPath, dllFileName);

				if (!File.Exists(dllPath)) {
					continue;
				}

				string dllDestinationPath = Path.Combine(OutputPath, LibrariesDir, libraryName, libraryVersion, dllFileName);
				string dllDestinationDir = Path.GetDirectoryName(dllDestinationPath);

				Log.LogMessage(MessageImportance.Low, $"Moving library: '{dllFileName}' to '{dllDestinationPath}'...");

				if (File.Exists(dllDestinationPath)) {
					File.Delete(dllDestinationPath);
				}

				Directory.CreateDirectory(dllDestinationDir);
				File.Move(dllPath, dllDestinationPath);

				// Check for pdb too...
				string pdbPath = Path.ChangeExtension(dllPath, "pdb");

				if (File.Exists(pdbPath)) {
					string pdbDestinationPath = Path.ChangeExtension(dllDestinationPath, "pdb");

					if (File.Exists(pdbDestinationPath)) {
						File.Delete(pdbDestinationPath);
					}

					File.Move(pdbPath, pdbDestinationPath);
				}

				// Enqueue a rename of this key.
				if (dllKey != dllFileName) {
					(dllRenames ??= new()).Add((dllKey, dllFileName));
				}
			}

			if (dllRenames != null) {
				foreach (var (keyOld, keyNew) in dllRenames) {
					runtimeObject[keyNew] = runtimeObject[keyOld];

					runtimeObject.Remove(keyOld);
				}
			}
		}

		private void MoveNativeLibraries(JObject libraryObject, string libraryName, string libraryVersion)
		{
			if (libraryObject["runtimeTargets"] is not JObject runtimeTargetsObject) {
				return;
			}

			List<(string keyOld, string keyNew)>? nativeDllRenames = null;

			foreach (var dllPair in runtimeTargetsObject) {
				if (dllPair.Value is not JObject dllObject) {
					continue;
				}

				if (dllObject["assetType"] is not JValue { Value: "native" }) {
					continue;
				}

				if ((dllObject["rid"] as JValue)?.Value is not string runtimeId) {
					continue;
				}

				string dllKey = dllPair.Key;
				string dllFileName = Path.GetFileName(dllKey);
				string dllPath = Path.Combine(OutputPath, dllKey);

				if (!File.Exists(dllPath)) {
					continue;
				}

				string newDllKey = $"{LibrariesDir}/Native/{runtimeId}/{dllFileName}";

				string dllDestinationPath = Path.Combine(OutputPath, newDllKey.Replace('/', Path.DirectorySeparatorChar));
				string dllDestinationDir = Path.GetDirectoryName(dllDestinationPath);

				Log.LogMessage(MessageImportance.Low, $"Moving native library: '{dllFileName}' to '{dllDestinationPath}'...");

				if (File.Exists(dllDestinationPath)) {
					File.Delete(dllDestinationPath);
				}

				Directory.CreateDirectory(dllDestinationDir);
				File.Move(dllPath, dllDestinationPath);

				// Enqueue a rename of this key.
				if (dllKey != dllFileName) {
					(nativeDllRenames ??= new()).Add((dllKey, newDllKey));
				}
			}

			if (nativeDllRenames != null) {
				foreach (var (keyOld, keyNew) in nativeDllRenames) {
					runtimeTargetsObject[keyNew] = runtimeTargetsObject[keyOld];

					runtimeTargetsObject.Remove(keyOld);
				}
			}
		}
	}
}
