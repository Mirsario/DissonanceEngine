﻿using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Input
{
	partial class InputEngine
	{
		private InputTrigger[] triggers;
		private Dictionary<string, InputTrigger> triggersByName;

		public static InputTrigger[] Triggers => Instance.triggers;

		private void InitTriggers()
		{
			triggers = new InputTrigger[0];
			triggersByName = new Dictionary<string, InputTrigger>();
		}
		private void UpdateTriggers()
		{
			for(int i = 0; i < triggers.Length; i++) {
				var trigger = triggers[i];
				ref var input = ref trigger.CurrentInput;

				input.prevAnalogInput = input.analogInput;
				input.wasPressed = input.isPressed;

				trigger.Value = 0f;

				for(int j = 0; j < trigger.bindingCount; j++) {
					trigger.Value += trigger.bindings[j].Value;
				}
			}
		}

		public static InputTrigger RegisterTrigger(string name, InputBinding[] bindings, float? minValue = null, float? maxValue = null)
			=> RegisterTrigger(typeof(InputTrigger), name, bindings, minValue, maxValue);
		public static T GetTrigger<T>() where T : SingletonInputTrigger
			=> (T)Instance.triggers[SingletonInputTrigger.Info<T>.id];

		internal static InputTrigger RegisterTrigger(Type type, string name, InputBinding[] bindings, float? minValue = null, float? maxValue = null)
		{
			var instance = Instance;

			if(instance.triggersByName.TryGetValue(name, out var trigger)) {
				trigger.Bindings = bindings;
				return trigger;
			}

			int id = instance.triggers.Length;

			trigger = (InputTrigger)Activator.CreateInstance(type, true); //new InputTrigger();

			trigger.Init(id, name, bindings, minValue ?? InputTrigger.DefaultMinValue, maxValue ?? InputTrigger.DefaultMaxValue);

			Array.Resize(ref instance.triggers, id + 1);

			instance.triggers[id] = trigger;
			instance.triggersByName[name] = trigger;

			InputTrigger.Count = instance.triggers.Length;

			return trigger;
		}
	}
}
