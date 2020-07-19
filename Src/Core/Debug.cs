using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Dissonance.Engine.Core
{
	public static class Debug
	{
		public static readonly object LoggingLock = new object();
		public static readonly Dictionary<string,ConsoleColor> ThreadNameToColor = new Dictionary<string,ConsoleColor>();

		public static bool LogCurrentThread { get; set; }
		public static bool LogAssembly { get; set; } = true;

		public static void Log(object message,bool showTrace = false,int stackframeOffset = 1)
		{
			var stackTrace = new StackTrace(true);
			var stackFrames = stackTrace.GetFrames();
			var stackFrame = stackFrames[stackframeOffset];
			string fileName = Path.GetFileName(stackFrame.GetFileName());
			int lineNumber = stackFrames[stackframeOffset].GetFileLineNumber();
			var stackFrameMethod = stackFrame.GetMethod();

			lock(LoggingLock) {
				string threadName = Thread.CurrentThread.Name;
				bool logThread = LogCurrentThread && threadName!=null;

				lock(ThreadNameToColor) {
					Console.ForegroundColor = logThread && ThreadNameToColor.TryGetValue(threadName,out var consoleColor) ? consoleColor : ConsoleColor.DarkGray;
				}

				Console.Write("[");

				if(logThread) {
					Console.Write($"{Thread.CurrentThread.Name} - ");
				}

				if(LogAssembly) {
					Console.Write($"{(stackFrameMethod!=null ? stackFrameMethod.DeclaringType.Assembly.GetName().Name : "Unknown")} - ");
				}

				Console.Write($"{fileName}, line {lineNumber}] ");

				Console.ForegroundColor = ConsoleColor.Gray;

				Console.WriteLine(message);

				if(showTrace) {
					var stringBuilder = new StringBuilder();

					for(int i = 1;i<stackFrames.Length;i++) {
						if(stackFrames[i].GetFileName()==null) {
							break;
						}

						var method = stackFrames[i].GetMethod();
						var parameters = method.GetParameters();

						stringBuilder.Append($"  {Path.GetFileName(stackFrames[i].GetFileName())}:{method.Name}(");

						for(int j = 0;j<parameters.Length;j++) {
							stringBuilder.Append($"{(j>0 ? "," : "")}{parameters[j].ParameterType.Name}");
						}

						stringBuilder.Append($") at line {stackFrames[i].GetFileLineNumber()}");
					}

					Console.Write(stringBuilder);
				}
			}
		}
		public static void RemoveLine(int line)
		{
			int currentLine = Console.CursorTop-1;

			Console.SetCursorPosition(0,line);

			Console.Write(new string(' ',Console.WindowWidth));

			Console.SetCursorPosition(0,currentLine);
		}
	}
}

