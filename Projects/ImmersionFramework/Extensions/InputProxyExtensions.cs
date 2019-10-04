using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public static class InputProvaiderExtensions
	{
		public static bool Active<TTrigger>(this IInputProvaider provaider) where TTrigger : SingletonInputTrigger
			=> provaider!=null && provaider.Inputs[SingletonInputTrigger.Info<TTrigger>.Id].Active;
		public static bool WasActive<TTrigger>(this IInputProvaider provaider) where TTrigger : SingletonInputTrigger
			=> provaider!=null && provaider.Inputs[SingletonInputTrigger.Info<TTrigger>.Id].WasActive;
		public static bool JustActivated<TTrigger>(this IInputProvaider provaider) where TTrigger : SingletonInputTrigger
			=> provaider!=null && provaider.Inputs[SingletonInputTrigger.Info<TTrigger>.Id].JustActivated;
		public static bool JustDeactivated<TTrigger>(this IInputProvaider provaider) where TTrigger : SingletonInputTrigger
			=> provaider!=null && provaider.Inputs[SingletonInputTrigger.Info<TTrigger>.Id].JustDeactivated;

		public static float Value<TTrigger>(this IInputProvaider provaider) where TTrigger : SingletonInputTrigger
			=> provaider==null ? 0f : provaider.Inputs[SingletonInputTrigger.Info<TTrigger>.Id].value;
		public static float PrevValue<TTrigger>(this IInputProvaider provaider) where TTrigger : SingletonInputTrigger
			=> provaider==null ? 0f : provaider.Inputs[SingletonInputTrigger.Info<TTrigger>.Id].prevValue;

		public static Vector2 Value<TTriggerX,TTriggerY>(this IInputProvaider provaider) where TTriggerX : SingletonInputTrigger where TTriggerY : SingletonInputTrigger
		{
			if(provaider==null) {
				return default;
			}

			var inputs = provaider.Inputs;
			return new Vector2(
				inputs[SingletonInputTrigger.Info<TTriggerX>.Id].value,
				inputs[SingletonInputTrigger.Info<TTriggerY>.Id].value
			);
		}
		public static Vector2 PrevValue<TTriggerX,TTriggerY>(this IInputProvaider provaider) where TTriggerX : SingletonInputTrigger where TTriggerY : SingletonInputTrigger
		{
			if(provaider==null) {
				return default;
			}

			var inputs = provaider.Inputs;
			return new Vector2(
				inputs[SingletonInputTrigger.Info<TTriggerX>.Id].prevValue,
				inputs[SingletonInputTrigger.Info<TTriggerY>.Id].prevValue
			);
		}

		public static Vector3 Value<TTriggerX,TTriggerY,TTriggerZ>(this IInputProvaider provaider) where TTriggerX : SingletonInputTrigger where TTriggerY : SingletonInputTrigger where TTriggerZ : SingletonInputTrigger
		{
			if(provaider==null) {
				return default;
			}

			var inputs = provaider.Inputs;
			return new Vector3(
				inputs[SingletonInputTrigger.Info<TTriggerX>.Id].value,
				inputs[SingletonInputTrigger.Info<TTriggerY>.Id].value,
				inputs[SingletonInputTrigger.Info<TTriggerZ>.Id].value
			);
		}
		public static Vector3 PrevValue<TTriggerX,TTriggerY,TTriggerZ>(this IInputProvaider provaider) where TTriggerX : SingletonInputTrigger where TTriggerY : SingletonInputTrigger where TTriggerZ : SingletonInputTrigger
		{
			if(provaider==null) {
				return default;
			}

			var inputs = provaider.Inputs;
			return new Vector3(
				inputs[SingletonInputTrigger.Info<TTriggerX>.Id].prevValue,
				inputs[SingletonInputTrigger.Info<TTriggerY>.Id].prevValue,
				inputs[SingletonInputTrigger.Info<TTriggerZ>.Id].prevValue
			);
		}
	}
}
