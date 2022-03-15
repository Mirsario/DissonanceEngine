using System.IO;
using Microsoft.Build.Framework;
using Newtonsoft.Json;

namespace Dissonance.BuildTools.Tasks
{
	/// <summary>
	/// Recursively deletes empty subdirectories in a provided directory.
	/// </summary>
	public class DeleteEmptyDirectories : TaskBase
	{
		[Required]
		public string Path { get; set; } = string.Empty;

		protected override void Run()
		{
			var directory = new DirectoryInfo(System.IO.Path.GetFullPath(Path));

			if (directory.Exists) {
				Recursion(directory);
			}
		}

		private void Recursion(DirectoryInfo directory)
		{
			foreach (var subDirectory in directory.EnumerateDirectories()) {
				Recursion(subDirectory);

				if (subDirectory.GetFiles().Length == 0 && subDirectory.GetDirectories().Length == 0) {
					Log.LogMessage(MessageImportance.High, $"Deleting folder '{subDirectory.Name}'...");

					subDirectory.Delete(true);
				}
			}
		}
	}
}
