using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using OpenTK;
using OpenTK.Input;
using TKMouseButton = OpenTK.Input.MouseButton;

namespace GameEngine
{
	public static class Input
	{
		#region Consts
		public const int MaxMouseButtons = 12;
		public const int MaxGamepads = 4;
		#endregion
		#region Fields
		internal static InputVariables fixedTimeVars;
		internal static InputVariables renderTimeVars;
		internal static InputTrigger[] triggers;
		internal static Dictionary<string,InputTrigger> triggersByName;
		#endregion
		#region Properties
		internal static InputVariables Vars => Game.fixedUpdate ? fixedTimeVars : renderTimeVars;
		public static InputTrigger[] Triggers => triggers;
		public static int TriggerCount => triggers.Length;

		//Mouse
		public static Vector2 MouseDelta => Vars.mouseDelta;
		public static Vector2 MousePosition => Vars.mousePosition;
		public static int MouseWheel => Vars.mouseWheel;

		//Keyboard
		public static string InputString => Vars.inputString;
		#endregion

		#region Initialization
		internal static void Init()
		{
			fixedTimeVars = new InputVariables();
			renderTimeVars = new InputVariables();
			triggers = new InputTrigger[0];
			triggersByName = new Dictionary<string,InputTrigger>();
		}
		#endregion
		#region Update
		internal static void FixedUpdate() => Update();
		internal static void RenderUpdate() => Update();
		internal static void LateFixedUpdate() => LateUpdate();
		internal static void LateRenderUpdate() => LateUpdate();
		private static void Update()
		{
			var vars = Vars;
			if(Game.HasFocus) {
				vars.mouseStatePrev = vars.mouseState;
				vars.mouseState = Mouse.GetState();
				vars.cursorState = Mouse.GetCursorState();
				
				vars.mouseWheel = vars.mouseState.Wheel-vars.mouseStatePrev.Wheel;

				Vector2Int point = Game.window.PointToClient(new Vector2Int(vars.cursorState.X,vars.cursorState.Y));
				vars.mousePosition = new Vector2(point.x,point.y);
				vars.mouseDelta = new Vector2(vars.mouseStatePrev.X-vars.mouseState.X,vars.mouseStatePrev.Y-vars.mouseState.Y);
				
				for(int i = 0;i<MaxGamepads;i++) {
					var state = GamePad.GetState(i);
					string prefix = "GamePad"+i+" ";

					TriggerSet((float)state.Buttons.A,prefix+"Button0");
					TriggerSet((float)state.Buttons.B,prefix+"Button1");
					TriggerSet((float)state.Buttons.X,prefix+"Button2");
					TriggerSet((float)state.Buttons.Y,prefix+"Button3");
					TriggerSet((float)state.Buttons.LeftShoulder,prefix+"Button4");
					TriggerSet((float)state.Buttons.RightShoulder,prefix+"Button5");
					TriggerSet((float)state.Buttons.LeftStick,prefix+"Button6");
					TriggerSet((float)state.Buttons.RightStick,prefix+"Button7");
					TriggerSet((float)state.Buttons.Start,prefix+"Button8");
					TriggerSet((float)state.Buttons.Back,prefix+"Button9");
					TriggerSet((float)state.Buttons.BigButton,prefix+"Button10");
					TriggerSet((float)state.DPad.Up,prefix+"Button11");
					TriggerSet((float)state.DPad.Down,prefix+"Button12");
					TriggerSet((float)state.DPad.Left,prefix+"Button13");
					TriggerSet((float)state.DPad.Right,prefix+"Button14");

					var gamepadSticks = state.ThumbSticks;
					Vector2 leftStick = gamepadSticks.Left;
					TriggerSet(leftStick.x,prefix+"Axis0");
					TriggerSet(leftStick.y,prefix+"Axis1");
					Vector2 rightStick = gamepadSticks.Right;
					TriggerSet(-rightStick.x,prefix+"Axis3");
					TriggerSet(rightStick.y,prefix+"Axis4");

					var gamepadTriggers = state.Triggers;
					TriggerSet(gamepadTriggers.Left,prefix+"Axis2");
					TriggerSet(gamepadTriggers.Right,prefix+"Axis5");

					vars.gamepadStates[i] = state;
				}
			}else{
				vars.mouseDelta = Vector2.zero;
			}
			TriggerSet(vars.mouseDelta.x,"Mouse X",false);
			TriggerSet(vars.mouseDelta.y,"Mouse Y",false);
			TriggerSet(vars.mouseWheel,"Mouse ScrollWheel",false);

			CheckSpecialCombinations();
		}
		private static void LateUpdate()
		{
			var vars = Vars;

			vars.pressedKeysPrev.Clear();
			foreach(var key in vars.pressedKeys) {
				vars.pressedKeysPrev.Add(key);
			}

			vars.pressedButtonsPrev.Clear();
			foreach(string str in vars.pressedButtons) {
				vars.pressedButtonsPrev.Add(str);
			}

			for(int i=0;i<MaxMouseButtons;i++) {
				vars.mouseButtonsPrev[i] = vars.mouseButtons[i];
			}

			for(int i = 0;i<triggers.Length;i++) {
				var trigger = triggers[i];
				if(Game.fixedUpdate) {
					trigger.fixedInput.prevAnalogInput = trigger.fixedInput.analogInput;
					trigger.fixedInput.wasPressed = trigger.fixedInput.isPressed;
				}else{
					trigger.renderInput.prevAnalogInput = trigger.renderInput.analogInput;
					trigger.renderInput.wasPressed = trigger.renderInput.isPressed;
				}
			}

			vars.inputString = string.Empty;
		}
		private static void CheckSpecialCombinations()
		{
			if(GetKeyDown(Keys.F4) && (GetKey(Keys.LAlt) || GetKey(Keys.RAlt))) {
				Game.Quit();
			}else if(GetKeyDown(Keys.Enter) && (GetKey(Keys.LAlt) || GetKey(Keys.RAlt))) {
				Graphics.Fullscreen = !Graphics.Fullscreen;
			}
		}
		#endregion
		#region Dispose
		public static void Dispose()
		{
			
		}
		#endregion

		#region Triggers
		public static InputTrigger RegisterTrigger(string name,InputBinding[] bindings,float minValue = float.MinValue,float maxValue = float.MaxValue)
		{
			if(!triggersByName.TryGetValue(name,out var trigger)) {
				int id = triggers.Length;
				trigger = new InputTrigger(id,name,bindings,minValue,maxValue);
				Array.Resize(ref triggers,id+1);
				triggers[id] = trigger;
				triggersByName[name] = trigger;
			}else{
				trigger.Bindings = bindings;
			}
			return trigger;
		}
		
		private static void TriggerSet(float value,string triggerName,bool setBoth = true)
		{
			bool fixedTime = setBoth || Game.fixedUpdate;
			bool renderTime = setBoth || !Game.fixedUpdate;
			int hash = triggerName.ToLower(CultureInfo.InvariantCulture).GetHashCode();
			for(int i = 0;i<triggers.Length;i++) {
				var trigger = triggers[i];
				float fixedSumm = 0f;
				float renderSumm = 0f;
				for(int j = 0;j<trigger.bindingCount;j++) {
					ref var input = ref trigger.bindings[j];
					if(trigger.bindingsHashes[j]==hash) {
						float newValue = (input.inversed ? -value : value)*input.sensitivity;
						if(fixedTime) {
							input.fixedAnalogInput = newValue;
						}
						if(renderTime) {
							input.renderAnalogInput = newValue;
						}
					}
					if(fixedTime && (input.fixedAnalogInput>input.deadZone || input.fixedAnalogInput<-input.deadZone)) {
						fixedSumm += input.fixedAnalogInput;
					}
					if(renderTime && (input.renderAnalogInput>input.deadZone || input.renderAnalogInput<-input.deadZone)) {
						renderSumm += input.renderAnalogInput;
					}
				}
				trigger.SetAnalogValue(fixedSumm,renderSumm);
			}
		}
		private static void TriggerReset(string triggerName)
		{
			int hash = triggerName.ToLower().GetHashCode();
			for(int i = 0;i<triggers.Length;i++) {
				var trigger = triggers[i];
				float fixedSumm = 0f;
				float renderSumm = 0f;
				for(int j = 0;j<trigger.bindingCount;j++) {
					ref var input = ref trigger.bindings[j];
					if(trigger.bindingsHashes[j]==hash) {
						input.fixedAnalogInput = 0f;
						input.renderAnalogInput = 0f;
					}else{
						if(input.fixedAnalogInput>input.deadZone || input.fixedAnalogInput<-input.deadZone) {
							fixedSumm += input.fixedAnalogInput;
						}
						if(input.renderAnalogInput>input.deadZone || input.renderAnalogInput<-input.deadZone) {
							renderSumm += input.renderAnalogInput;
						}
					}
				}
				trigger.SetAnalogValue(fixedSumm,renderSumm);
			}
		}
		#endregion

		#region GetFunctions
		public static float GetDirection(Keys left,Keys right) => (GetKey(left) ?-1f : 0f)+(GetKey(right) ? 1f : 0f);
		public static Vector2 GetDirection(Keys up,Keys down,Keys left,Keys right) => new Vector2(
			(GetKey(right) ? 1f : 0f)-(GetKey(left) ? 1f : 0f),
			(GetKey(up) ? 1f : 0f)-(GetKey(down) ? 1f : 0f)
		);
		public static Vector2 GetDirection(InputTrigger up,InputTrigger down,InputTrigger left,InputTrigger right) => new Vector2(
			(right.IsPressed ? 1f : 0f)-(left.IsPressed ? 1f : 0f),
			(up.IsPressed ? 1f : 0f)-(down.IsPressed ? 1f : 0f)
		);

		public static bool GetKey(Keys key) => (Game.fixedUpdate ? fixedTimeVars : renderTimeVars).pressedKeys.Contains(key);
		public static bool GetKeyDown(Keys key)
		{
			var vars = Game.fixedUpdate ? fixedTimeVars : renderTimeVars;
			return vars.pressedKeys.Contains(key) && !vars.pressedKeysPrev.Contains(key);
		}
		public static bool GetKeyUp(Keys key)
		{
			var vars = Game.fixedUpdate ? fixedTimeVars : renderTimeVars;
			return !vars.pressedKeys.Contains(key) && vars.pressedKeysPrev.Contains(key);
		}

		public static bool GetMouseButton(MouseButton button) => (Game.fixedUpdate ? fixedTimeVars : renderTimeVars).mouseButtons[(int)button];
		public static bool GetMouseButtonDown(MouseButton button)
		{
			var vars = Game.fixedUpdate ? fixedTimeVars : renderTimeVars;
			return vars.mouseButtons[(int)button] && !vars.mouseButtonsPrev[(int)button];
		}
		public static bool GetMouseButtonUp(MouseButton button)
		{
			var vars = Game.fixedUpdate ? fixedTimeVars : renderTimeVars;
			return !vars.mouseButtons[(int)button] && vars.mouseButtonsPrev[(int)button];
		}
		#endregion

		#region Callbacks
		#region Mouse
		internal static void MouseMove(object sender,MouseMoveEventArgs e)
		{
			
		}
		internal static void MouseDown(object sender,MouseButtonEventArgs e)
		{
			#region Switch
			//Bleh constant switch to avoid string concateration
			int index;
			string name;
			switch(e.Button) {
				case TKMouseButton.Left:
					index = 0;
					name = "Mouse Left";
					break;
				case TKMouseButton.Right: //Index 2 -> 1
					index = 1;
					name = "Mouse Right";
					break;
				case TKMouseButton.Middle: //Index 1 -> 2
					index = 2;
					name = "Mouse Middle";
					break;
				case TKMouseButton.Button1:
					index = 3;
					name = "Mouse XButton1";
					break;
				case TKMouseButton.Button2:
					index = 4;
					name = "Mouse XButton2";
					break;
				case TKMouseButton.Button3:
					index = 5;
					name = "Mouse XButton3";
					break;
				case TKMouseButton.Button4:
					index = 6;
					name = "Mouse XButton4";
					break;
				case TKMouseButton.Button5:
					index = 7;
					name = "Mouse XButton5";
					break;
				case TKMouseButton.Button6:
					index = 8;
					name = "Mouse XButton6";
					break;
				case TKMouseButton.Button7:
					index = 9;
					name = "Mouse XButton7";
					break;
				case TKMouseButton.Button8:
					index = 10;
					name = "Mouse XButton8";
					break;
				case TKMouseButton.Button9:
					index = 11;
					name = "Mouse XButton9";
					break;
				default:
					return;
			}
			#endregion

			fixedTimeVars.mouseButtons[index] = true;
			fixedTimeVars.pressedButtons.Add(name);
			renderTimeVars.mouseButtons[index] = true;
			renderTimeVars.pressedButtons.Add(name);

			TriggerSet(1f,name);
		}
		internal static void MouseUp(object sender,MouseButtonEventArgs e)
		{
			#region Switch
			//Bleh constant switch to avoid string concateration
			int index;
			string name;
			switch(e.Button) {
				case TKMouseButton.Left:
					index = 0;
					name = "Mouse Left";
					break;
				case TKMouseButton.Right: //Index 2 -> 1
					index = 1;
					name = "Mouse Right";
					break;
				case TKMouseButton.Middle: //Index 1 -> 2
					index = 2;
					name = "Mouse Middle";
					break;
				case TKMouseButton.Button1:
					index = 3;
					name = "Mouse XButton1";
					break;
				case TKMouseButton.Button2:
					index = 4;
					name = "Mouse XButton2";
					break;
				case TKMouseButton.Button3:
					index = 5;
					name = "Mouse XButton3";
					break;
				case TKMouseButton.Button4:
					index = 6;
					name = "Mouse XButton4";
					break;
				case TKMouseButton.Button5:
					index = 7;
					name = "Mouse XButton5";
					break;
				case TKMouseButton.Button6:
					index = 8;
					name = "Mouse XButton6";
					break;
				case TKMouseButton.Button7:
					index = 9;
					name = "Mouse XButton7";
					break;
				case TKMouseButton.Button8:
					index = 10;
					name = "Mouse XButton8";
					break;
				case TKMouseButton.Button9:
					index = 11;
					name = "Mouse XButton9";
					break;
				default:
					return;
			}
			#endregion

			fixedTimeVars.mouseButtons[index] = false;
			fixedTimeVars.pressedButtons.Remove(name);
			renderTimeVars.mouseButtons[index] = false;
			renderTimeVars.pressedButtons.Remove(name);

			TriggerReset(name);
		}
		#endregion

		#region Keys
		internal static void KeyPress(object sender,KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;
			fixedTimeVars.inputString += keyChar;
			renderTimeVars.inputString += keyChar;
		}
		internal static void KeyDown(object sender,KeyboardKeyEventArgs e)
		{
			var key = (Keys)(int)e.Key;
			if(!fixedTimeVars.pressedKeys.Contains(key)) {
				fixedTimeVars.pressedKeys.Add(key);
				fixedTimeVars.pressedButtons.Add(key.ToString());
			}
			if(!renderTimeVars.pressedKeys.Contains(key)) {
				renderTimeVars.pressedKeys.Add(key);
				renderTimeVars.pressedButtons.Add(key.ToString());
			}

			TriggerSet(1f,key.ToString());
		}
		internal static void KeyUp(object sender,KeyboardKeyEventArgs e)
		{
			var key = (Keys)(int)e.Key;
			if(fixedTimeVars.pressedKeys.Contains(key)) {
				fixedTimeVars.pressedKeys.Remove(key);
				fixedTimeVars.pressedButtons.Remove(key.ToString());
			}
			if(renderTimeVars.pressedKeys.Contains(key)) {
				renderTimeVars.pressedKeys.Remove(key);
				renderTimeVars.pressedButtons.Remove(key.ToString());
			}

			TriggerReset(key.ToString());
		}
		#endregion
		#endregion
	}
}