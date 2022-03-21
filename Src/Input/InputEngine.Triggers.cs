using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine.Input
{
	partial class InputEngine
	{
		internal static class SingletonTriggerIds<T> where T : SingletonInputTrigger
		{
#pragma warning disable CS0649
			public static int Id;
#pragma warning restore CS0649
		}

		private static InputTrigger[] triggers = Array.Empty<InputTrigger>();
		private static Dictionary<string, InputTrigger> triggersByName = new();

		public static InputTrigger[] Triggers => triggers;

		private static void InitTriggers()
		{
			
		}

		private static void InitTriggersForAssembly(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes()) {
				if (type.IsAbstract || !typeof(SingletonInputTrigger).IsAssignableFrom(type)) {
					continue;
				}

				var trigger = RegisterTrigger(type, type.Name, null);

				typeof(SingletonTriggerIds<>).MakeGenericType(type)
					.GetField(nameof(SingletonTriggerIds<SingletonInputTrigger>.Id), BindingFlags.Static | BindingFlags.Public)
					.SetValue(null, trigger.Id);
			}
		}

		private static void UpdateTriggers()
		{
			for (int i = 0; i < triggers.Length; i++) {
				var trigger = triggers[i];
				ref var input = ref trigger.CurrentInput;

				input.PrevAnalogInput = input.AnalogInput;
				input.WasPressed = input.IsPressed;

				trigger.Value = 0f;

				for (int j = 0; j < trigger.bindingCount; j++) {
					trigger.Value += trigger.bindings[j].Value;
				}
			}
		}

		public static InputTrigger RegisterTrigger(string name, InputBinding[] bindings, float? minValue = null, float? maxValue = null)
			=> RegisterTrigger(typeof(InputTrigger), name, bindings, minValue, maxValue);

		public static T GetTrigger<T>() where T : SingletonInputTrigger
			=> (T)triggers[SingletonTriggerIds<T>.Id];

		internal static InputTrigger RegisterTrigger(Type type, string name, InputBinding[] bindings, float? minValue = null, float? maxValue = null)
		{
			if (triggersByName.TryGetValue(name, out var trigger)) {
				trigger.Bindings = bindings;
				return trigger;
			}

			int id = triggers.Length;

			trigger = (InputTrigger)Activator.CreateInstance(type, true); // new InputTrigger();

			trigger.Init(id, name, bindings, minValue ?? InputTrigger.DefaultMinValue, maxValue ?? InputTrigger.DefaultMaxValue);

			Array.Resize(ref triggers, id + 1);

			triggers[id] = trigger;
			triggersByName[name] = trigger;

			InputTrigger.Count = triggers.Length;

			return trigger;
		}
	}
}
