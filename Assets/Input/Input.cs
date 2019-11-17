using System;
using System.Collections.Generic;
using System.Reflection;
using OpenTK;
using OpenTK.Input;
using TKMouseButton = OpenTK.Input.MouseButton;

namespace GameEngine
{
	public static class Input
	{
		public const int MaxMouseButtons = 12;
		public const int MaxGamepads = 4;

		internal static InputVariables fixedTimeVars;
		internal static InputVariables renderTimeVars;
		internal static InputTrigger[] triggers;
		internal static Dictionary<string,InputTrigger> triggersByName;

		internal static InputVariables Vars => Game.fixedUpdate ? fixedTimeVars : renderTimeVars;
		public static InputTrigger[] Triggers => triggers;

		//Mouse
		public static Vector2 MouseDelta => Vars.mouseDelta;
		public static Vector2 MousePosition => Vars.mousePosition;
		public static int MouseWheel => Vars.mouseWheel;

		//Keyboard
		public static string InputString => Vars.inputString;

		#region Initialization
		internal static void Init()
		{
			fixedTimeVars = new InputVariables();
			renderTimeVars = new InputVariables();
			triggers = new InputTrigger[0];
			triggersByName = new Dictionary<string,InputTrigger>();

			SingletonInputTrigger.StaticInit();
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
					string prefix = "gamepad"+i+" ";

					var buttons = state.Buttons;
					TriggerSet((float)buttons.A,prefix+"button0");
					TriggerSet((float)buttons.B,prefix+"button1");
					TriggerSet((float)buttons.X,prefix+"button2");
					TriggerSet((float)buttons.Y,prefix+"button3");
					TriggerSet((float)buttons.LeftShoulder,prefix+"button4");
					TriggerSet((float)buttons.RightShoulder,prefix+"button5");
					TriggerSet((float)buttons.LeftStick,prefix+"button6");
					TriggerSet((float)buttons.RightStick,prefix+"button7");
					TriggerSet((float)buttons.Start,prefix+"button8");
					TriggerSet((float)buttons.Back,prefix+"button9");
					TriggerSet((float)buttons.BigButton,prefix+"button10");

					var dpad = state.DPad;
					TriggerSet((float)dpad.Up,prefix+"button11");
					TriggerSet((float)dpad.Down,prefix+"button12");
					TriggerSet((float)dpad.Left,prefix+"button13");
					TriggerSet((float)dpad.Right,prefix+"button14");

					var gamepadSticks = state.ThumbSticks;

					Vector2 leftStick = gamepadSticks.Left;
					TriggerSet(leftStick.x,prefix+"axis0");
					TriggerSet(leftStick.y,prefix+"axis1");

					Vector2 rightStick = gamepadSticks.Right;
					TriggerSet(-rightStick.x,prefix+"axis3");
					TriggerSet(rightStick.y,prefix+"axis4");

					var gamepadTriggers = state.Triggers;
					TriggerSet(gamepadTriggers.Left,prefix+"axis2");
					TriggerSet(gamepadTriggers.Right,prefix+"axis5");

					vars.gamepadStates[i] = state;
				}
			}else{
				vars.mouseDelta = Vector2.Zero;
				vars.mouseWheel = 0;
			}

			TriggerSet(vars.mouseDelta.x,"mouse x",false);
			TriggerSet(vars.mouseDelta.y,"mouse y",false);
			TriggerSet(vars.mouseWheel,"mouse scrollwheel",false);

			if(Game.fixedUpdate) {
				CheckSpecialCombinations();
			}
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

			for(int i = 0;i<MaxMouseButtons;i++) {
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
				Screen.Fullscreen = !Screen.Fullscreen;
			}
		}
		#endregion
		#region Dispose
		public static void Dispose()
		{
			
		}
		#endregion

		#region Triggers
		//TODO: Improve design quality
		public static InputTrigger RegisterTrigger(string name,InputBinding[] bindings,float? minValue = null,float? maxValue = null)
			=> RegisterTrigger(typeof(InputTrigger),name,bindings,minValue,maxValue);
		//internal static T RegisterTrigger<T>(string name,InputBinding[] bindings,float? minValue = null,float? maxValue = null) where T : InputTrigger
		//	=> (T)RegisterTrigger(typeof(T),name,bindings,minValue,maxValue);
		internal static InputTrigger RegisterTrigger(Type type,string name,InputBinding[] bindings,float? minValue = null,float? maxValue = null)
		{
			if(!triggersByName.TryGetValue(name,out var trigger)) {
				int id = triggers.Length;

				trigger = (InputTrigger)Activator.CreateInstance(type,true); //new InputTrigger();
				trigger.Init(id,name,bindings,minValue ?? InputTrigger.DefaultMinValue,maxValue ?? InputTrigger.DefaultMaxValue);

				Array.Resize(ref triggers,id+1);
				triggers[id] = trigger;
				triggersByName[name] = trigger;

				InputTrigger.Count = triggers.Length;
			}else{
				trigger.Bindings = bindings;
			}

			return trigger;
		}
		
		private static void TriggerSet(float value,string triggerName,bool setBoth = true)
		{
			bool fixedTime = setBoth || Game.fixedUpdate;
			bool renderTime = setBoth || !Game.fixedUpdate;
			int hash = triggerName.GetHashCode();

			for(int i = 0;i<triggers.Length;i++) {
				var trigger = triggers[i];
				float fixedSumm = 0f;
				float renderSumm = 0f;

				for(int j = 0;j<trigger.bindingCount;j++) {
					ref var input = ref trigger.bindings[j];

					if(hash==input.InputHash && triggerName==input.InputLower) {
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
			int hash = triggerName.GetHashCode();
			for(int i = 0;i<triggers.Length;i++) {
				var trigger = triggers[i];
				float fixedSumm = 0f;
				float renderSumm = 0f;
				for(int j = 0;j<trigger.bindingCount;j++) {
					ref var input = ref trigger.bindings[j];

					if(hash==input.InputHash && triggerName==input.InputLower) {
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
		internal static void MouseDown(object sender,MouseButtonEventArgs e)
		{
			#region Switch
			//Bleh constant switch to avoid string concateration
			int index;
			string name;
			switch(e.Button) {
				case TKMouseButton.Left:
					index = 0;
					name = "mouse left";
					break;
				case TKMouseButton.Right: //Index 2 -> 1
					index = 1;
					name = "mouse right";
					break;
				case TKMouseButton.Middle: //Index 1 -> 2
					index = 2;
					name = "mouse middle";
					break;
				case TKMouseButton.Button1:
					index = 3;
					name = "mouse xbutton1";
					break;
				case TKMouseButton.Button2:
					index = 4;
					name = "mouse xbutton2";
					break;
				case TKMouseButton.Button3:
					index = 5;
					name = "mouse xbutton3";
					break;
				case TKMouseButton.Button4:
					index = 6;
					name = "mouse xbutton4";
					break;
				case TKMouseButton.Button5:
					index = 7;
					name = "mouse xbutton5";
					break;
				case TKMouseButton.Button6:
					index = 8;
					name = "mouse xbutton6";
					break;
				case TKMouseButton.Button7:
					index = 9;
					name = "mouse xbutton7";
					break;
				case TKMouseButton.Button8:
					index = 10;
					name = "mouse xbutton8";
					break;
				case TKMouseButton.Button9:
					index = 11;
					name = "mouse xbutton9";
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
					name = "mouse left";
					break;
				case TKMouseButton.Right: //Index 2 -> 1
					index = 1;
					name = "mouse right";
					break;
				case TKMouseButton.Middle: //Index 1 -> 2
					index = 2;
					name = "mouse middle";
					break;
				case TKMouseButton.Button1:
					index = 3;
					name = "mouse xbutton1";
					break;
				case TKMouseButton.Button2:
					index = 4;
					name = "mouse xbutton2";
					break;
				case TKMouseButton.Button3:
					index = 5;
					name = "mouse xbutton3";
					break;
				case TKMouseButton.Button4:
					index = 6;
					name = "mouse xbutton4";
					break;
				case TKMouseButton.Button5:
					index = 7;
					name = "mouse xbutton5";
					break;
				case TKMouseButton.Button6:
					index = 8;
					name = ",ouse xbutton6";
					break;
				case TKMouseButton.Button7:
					index = 9;
					name = ",ouse xbutton7";
					break;
				case TKMouseButton.Button8:
					index = 10;
					name = ",ouse xbutton8";
					break;
				case TKMouseButton.Button9:
					index = 11;
					name = ",ouse xbutton9";
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

			TriggerSet(1f,key.ToString().ToLower());
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

			TriggerReset(key.ToString().ToLower());
		}
		#endregion
		#endregion
	}
}