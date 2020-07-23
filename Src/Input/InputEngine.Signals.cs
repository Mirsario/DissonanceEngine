using System;
using System.Collections.Generic;
using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine.Input
{
	partial class InputEngine
	{
		private Dictionary<string,int> signalIdByName;
		private List<(Func<object,float> getter,object arg)> signals;

		private void InitSignals()
		{
			signalIdByName = new Dictionary<string,int>(StringComparer.InvariantCultureIgnoreCase);
			signals = new List<(Func<object,float>, object)>();

			//Register key signals
			foreach(Keys key in Enum.GetValues(typeof(Keys))) {
				RegisterSignal(key.ToString(),arg => GetKey((Keys)arg) ? 1f : 0f,key);
			}

			//Register mouse signals
			foreach(MouseButton button in Enum.GetValues(typeof(MouseButton))) {
				RegisterSignal($"Mouse{button}",arg => GetMouseButton((MouseButton)arg) ? 1f : 0f,button);
			}

			RegisterSignal("MouseX",arg => MouseDelta.x);
			RegisterSignal("MouseY",arg => MouseDelta.y);
			RegisterSignal("MouseWheel",arg => MouseWheel);
		}

		public static void RegisterSignal(string nameId,Func<object,float> valueGetter,object arg = null)
		{
			var tuple = (valueGetter,arg);
			var instance = Instance;

			if(instance.signalIdByName.TryGetValue(nameId,out int id)) {
				instance.signals[id] = tuple;

				return;
			}

			instance.signalIdByName[nameId] = instance.signals.Count;

			instance.signals.Add(tuple);
		}
		public static float GetSignal(string nameId)
		{
			var instance = Instance;

			if(!instance.signalIdByName.TryGetValue(nameId,out int id)) {
				return 0f;
			}

			var (getter,arg) = instance.signals[id];

			return getter(arg);
		}
	}
}
