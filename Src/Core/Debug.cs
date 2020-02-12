using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Dissonance.Engine
{
	public static class Debug
	{
		private struct LineInfo
		{
			public string text;
			public string id;

			public LineInfo(string text,string id)
			{
				this.text = text;
				this.id = id;
			}
		}

		private static readonly List<LineInfo> Lines = new List<LineInfo>();
		private static readonly Dictionary<string,List<Stopwatch>> Stopwatches = new Dictionary<string,List<Stopwatch>>();

		public static void StartStopwatch(string name)
		{
			if(!Stopwatches.TryGetValue(name,out var stopwatchList)) {
				Stopwatches[name] = stopwatchList = new List<Stopwatch>();
			}

			var stopwatch = new Stopwatch();

			stopwatch.Start();

			stopwatchList.Add(stopwatch);
		}
		public static void EndStopwatch(string name)
		{
			if(Stopwatches.TryGetValue(name,out var stopwatchList) && stopwatchList.Count>0) {
				int i = stopwatchList.Count-1;

				var stopwatch = stopwatchList[i];

				stopwatch.Stop();

				Log(name+": "+stopwatch.Elapsed.TotalMilliseconds);

				stopwatchList.RemoveAt(i);
			}
		}
		public static void Log(object message,bool showTrace = false,string uniqueId = null,int stackframeOffset = 1)
		{
			string text = message.ToString();

			if(uniqueId!=null) {
				for(int i = 0,j = 0;i<Lines.Count;i++) {
					if(Lines[i].id==uniqueId) {
						RemoveLine(i);
						Lines.RemoveAt(i);

						i--;
						j++;
					}
				}

				text = uniqueId+": "+text;
			}

			var stackTrace = new StackTrace(true);
			var stackFrames = stackTrace.GetFrames();

			text = "["+Path.GetFileName(stackFrames[stackframeOffset].GetFileName())+", line "+stackFrames[stackframeOffset].GetFileLineNumber()+"] "+text;

			SplitWrite(text);

			Lines.Add(new LineInfo(text,uniqueId));

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

