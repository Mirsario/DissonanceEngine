using System;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace SourceGenerators;

internal static class ErrorLogging
{
	private static bool isActive;
	private static string? logFilePath;

	public static void Activate()
	{
		if (isActive) {
			return;
		}

		logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{Assembly.GetExecutingAssembly().GetName().Name}.log");

		//Log($"Current directory: '{Path.GetFullPath(".")}'.");

		AppDomain.CurrentDomain.FirstChanceException += FirstChanceException;
		
		isActive = true;
	}

	private static void FirstChanceException(object sender, FirstChanceExceptionEventArgs args)
	{
		if (args.Exception is not Exception exception) {
			return;
		}

		string exceptionText = exception.ToString();

		if (exceptionText.Contains($" {nameof(SourceGenerators)}")) {
			Log($"Error: {exception.GetType().Name} - {exceptionText}");
		}
	}

	public static void Log(string message)
	{
		lock (Console.Out) {
			File.AppendAllText(logFilePath, message + "\r\n");
		}
	}
}
