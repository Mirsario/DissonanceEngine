using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace GameEngine
{
	public static class Input
	{
		internal class InputVariables
		{
			//Mouse
			public MouseState cursorState;
			public MouseState mouseState;
			public MouseState mouseStatePrev;
			public int mouseWheel;
			public Vector2 mousePosition;
			//public Vector2 mousePositionPrev;
			public Vector2 mouseDelta;
			public bool[] mouseButtons = new bool[5];
			public bool[] mouseButtonsPrev = new bool[5];

			//Keyboard
			public List<Keys> pressedKeys = new List<Keys>();
			public List<Keys> pressedKeysPrev = new List<Keys>();
			public string inputString = "";
		}
		internal static InputVariables fixedTimeVars;
		internal static InputVariables renderTimeVars;
		internal static InputVariables Vars => Game.fixedUpdate ? fixedTimeVars : renderTimeVars;

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
				for(int i=0;i<5;i++) {
					vars.mouseButtonsPrev[i] = vars.mouseButtons[i];
				}
				vars.mouseStatePrev = vars.mouseState;
				vars.mouseState = Mouse.GetState();
				vars.cursorState = Mouse.GetCursorState();
				
				vars.mouseWheel = vars.mouseState.Wheel-vars.mouseStatePrev.Wheel;
				vars.mouseButtons[0] = vars.mouseState.LeftButton==ButtonState.Pressed;
				vars.mouseButtons[1] = vars.mouseState.RightButton==ButtonState.Pressed;
				vars.mouseButtons[2] = vars.mouseState.MiddleButton==ButtonState.Pressed;
				vars.mouseButtons[3] = vars.mouseState.XButton1==ButtonState.Pressed;
				vars.mouseButtons[4] = vars.mouseState.XButton2==ButtonState.Pressed;

				Vector2Int point = Game.window.PointToClient(new Vector2Int(vars.cursorState.X,vars.cursorState.Y));
				vars.mousePosition = new Vector2(point.x,point.y);
				vars.mouseDelta = new Vector2(vars.mouseStatePrev.X-vars.mouseState.X,vars.mouseStatePrev.Y-vars.mouseState.Y);
			}else{
				for(int i=0;i<5;i++) {
					vars.mouseButtonsPrev[i] = false;
				}
				vars.mouseDelta = Vector2.zero;
			}
			CheckSpecialCombinations();
		}
		private static void LateUpdate()
		{
			var vars = Vars;
			vars.pressedKeysPrev.Clear();
			vars.pressedKeysPrev.AddRange(vars.pressedKeys);
			vars.inputString = "";
		}
		private static void CheckSpecialCombinations()
		{
			if(GetKeyDown(Keys.Enter) && (GetKey(Keys.AltLeft) || GetKey(Keys.AltRight))) {
				Graphics.Fullscreen = !Graphics.Fullscreen;
			}
		}
		#endregion
		#region Dispose
		public static void Dispose()
		{
			
		}
		#endregion

		#region Public Methods
		public static float GetDirection(Keys left,Keys right)
		{
			return (GetKey(left) ?-1f : 0f)+(GetKey(right) ? 1f : 0f);
		}
		public static Vector2 GetDirection(Keys up,Keys down,Keys left,Keys right)
		{
			return new Vector2(
				(GetKey(right) ? 1f : 0f)-(GetKey(left) ? 1f : 0f),
				(GetKey(up) ? 1f : 0f)-(GetKey(down) ? 1f : 0f)
			);
		}
		public static bool GetKey(Keys key) => Vars.pressedKeys.Contains(key);
		public static bool GetKeyDown(Keys key) => Vars.pressedKeys.Contains(key) && !Vars.pressedKeysPrev.Contains(key);
		public static bool GetKeyUp(Keys key) => !Vars.pressedKeys.Contains(key) && Vars.pressedKeysPrev.Contains(key);
		public static bool GetMouseButton(int button) => Vars.mouseButtons[button];
		public static bool GetMouseButtonDown(int button) => Vars.mouseButtons[button] && !Vars.mouseButtonsPrev[button];
		public static bool GetMouseButtonUp(int button) => !Vars.mouseButtons[button] && Vars.mouseButtonsPrev[button];
		#endregion

		#region Callbacks
		internal static void MouseMove(object sender,MouseMoveEventArgs e)
		{
			
		}
		internal static void KeyPress(object sender,KeyPressEventArgs e)
		{
			for(int i=0;i<2;i++) {
				var vars = i==0 ? fixedTimeVars : renderTimeVars;
				vars.inputString += e.KeyChar;
			}
		}
		internal static void KeyDown(object sender,KeyboardKeyEventArgs e)
		{
			for(int i=0;i<2;i++) {
				var vars = i==0 ? fixedTimeVars : renderTimeVars;
				var key = (Keys)(int)e.Key;
				if(!vars.pressedKeys.Contains(key)) {
					vars.pressedKeys.Add(key);
				}
			}
		}
		internal static void KeyUp(object sender,KeyboardKeyEventArgs e)
		{
			for(int i=0;i<2;i++) {
				var vars = i==0 ? fixedTimeVars : renderTimeVars;
				var key = (Keys)(int)e.Key;
				if(vars.pressedKeys.Contains(key)) {
					vars.pressedKeys.Remove(key);
				}
			}
		}
		#endregion
	}
	public enum Keys {
		Unknown,
		ShiftLeft,
		LShift = 1,
		ShiftRight,
		RShift = 2,
		ControlLeft,
		LControl = 3,
		ControlRight,
		RControl = 4,
		AltLeft,
		LAlt = 5,
		AltRight,
		RAlt = 6,
		WinLeft,
		LWin = 7,
		WinRight,
		RWin = 8,
		Menu,
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		F10,
		F11,
		F12,
		F13,
		F14,
		F15,
		F16,
		F17,
		F18,
		F19,
		F20,
		F21,
		F22,
		F23,
		F24,
		F25,
		F26,
		F27,
		F28,
		F29,
		F30,
		F31,
		F32,
		F33,
		F34,
		F35,
		Up,
		Down,
		Left,
		Right,
		Enter,
		Escape,
		Space,
		Tab,
		BackSpace,
		Back = 53,
		Insert,
		Delete,
		PageUp,
		PageDown,
		Home,
		End,
		CapsLock,
		ScrollLock,
		PrintScreen,
		Pause,
		NumLock,
		Clear,
		Sleep,
		Keypad0,
		Keypad1,
		Keypad2,
		Keypad3,
		Keypad4,
		Keypad5,
		Keypad6,
		Keypad7,
		Keypad8,
		Keypad9,
		KeypadDivide,
		KeypadMultiply,
		KeypadSubtract,
		KeypadMinus = 79,
		KeypadAdd,
		KeypadPlus = 80,
		KeypadDecimal,
		KeypadPeriod = 81,
		KeypadEnter,
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
		K,
		L,
		M,
		N,
		O,
		P,
		Q,
		R,
		S,
		T,
		U,
		V,
		W,
		X,
		Y,
		Z,
		Number0,
		Number1,
		Number2,
		Number3,
		Number4,
		Number5,
		Number6,
		Number7,
		Number8,
		Number9,
		Tilde,
		Grave = 119,
		Minus,
		Plus,
		BracketLeft,
		LBracket = 122,
		BracketRight,
		RBracket = 123,
		Semicolon,
		Quote,
		Comma,
		Period,
		Slash,
		BackSlash,
		NonUSBackSlash,
		LastKey
	}
}