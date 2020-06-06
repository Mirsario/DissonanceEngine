using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Dissonance.Engine
{
	public static class Debug
	{
		public static void Log(object message,bool showTrace = false,int stackframeOffset = 1)
		{
			var stackTrace = new StackTrace(true);
			var stackFrames = stackTrace.GetFrames();
			var stackFrame = stackFrames[stackframeOffset];
			string fileName = Path.GetFileName(stackFrame.GetFileName());
			int lineNumber = stackFrames[stackframeOffset].GetFileLineNumber();

			Console.ForegroundColor = ConsoleColor.DarkGray;

			var stackFrameMethod = stackFrame.GetMethod();
			string sourceName = stackFrameMethod!=null ? stackFrameMethod.DeclaringType.Assembly.GetName().Name : "Unknown";

			Console.Write($"[{sourceName} - {fileName}, line {lineNumber}] ");

			Console.ForegroundColor = ConsoleColor.Gray;

			SplitWrite(message.ToString());

			if(showTrace) {
				for(int i = 1;i<stackFrames.Length;i++) {
					if(stackFrames[i].GetFileName()==null) {
						break;
					}

					var method = stackFrames[i].GetMethod();
					string frameText = "  "+Path.GetFileName(stackFrames[i].GetFileName())+":"+method.Name+"(";

					var parameters = method.GetParameters();

					for(int j = 0;j<parameters.Length;j++) {
						frameText += (j>0?",":"")+parameters[j].ParameterType.Name;
					}

					frameText += ") at line "+stackFrames[i].GetFileLineNumber();

					SplitWrite(frameText);
				}
			}
		}
		public static void SplitWrite(string str)
		{
			var lines = str.Split(new[]{"\r\n","\n"},StringSplitOptions.None);

			for(int i = 0;i<lines.Length;i++) {
				Console.WriteLine(lines[i]);
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

