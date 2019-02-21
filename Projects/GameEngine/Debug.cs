using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace GameEngine
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
		static List<LineInfo> lines = new List<LineInfo>();
		public static Dictionary<string,List<Stopwatch>> stopwatch = new Dictionary<string,List<Stopwatch>>();
		
		static Debug()
		{
			Console.Clear();
		}
		public static void StartStopwatch(string name)
		{
			if(!stopwatch.ContainsKey(name)) {
				stopwatch.Add(name,new List<Stopwatch>());
			}
			int i = stopwatch[name].Count;
			stopwatch[name].Add(new Stopwatch());
			stopwatch[name][i].Start();
		}
		public static void EndStopwatch(string name)
		{
			if(stopwatch.ContainsKey(name) && stopwatch[name].Count>0) {
				int i = stopwatch[name].Count-1;
				stopwatch[name][i].Stop();
				Log(name+": "+stopwatch[name][i].Elapsed.TotalMilliseconds);
				stopwatch[name].RemoveAt(i);
			}
		}
		public static void Log(object message,bool showTrace = false,string uniqueId = null,int stackframeOffset = 1)
		{
			string text = message.ToString();
			if(uniqueId!=null) {
				int j = 0;
				for(int i=0;i<lines.Count;i++) {
					if(lines[i].id==uniqueId) {
						RemoveLine(i);
						lines.RemoveAt(i);
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
			lines.Add(new LineInfo(text,uniqueId));
			if(showTrace) {
				for(int i=1;i<stackFrames.Length;i++) {
					if(stackFrames[i].GetFileName()==null) {
						break;
					}
					var method = stackFrames[i].GetMethod();
					string frameText = "  "+Path.GetFileName(stackFrames[i].GetFileName())+":"+method.Name+"(";
					var parameters = method.GetParameters();
					for(int j=0;j<parameters.Length;j++) {
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
			for(int i=0;i<lines.Length;i++) {
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

