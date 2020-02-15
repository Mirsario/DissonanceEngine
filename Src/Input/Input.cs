using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine
{
	public static partial class Input
	{
		public const int MaxMouseButtons = 12;
		public const int MaxGamepads = 4;

		internal static InputVariables fixedInput;
		internal static InputVariables renderInput;
		internal static InputVariables prevFixedInput;
		internal static InputVariables prevRenderInput;

		public static InputTrigger[] Triggers => triggers;
		//Mouse
		public static Vector2 MouseDelta => PrevInput.mousePosition-CurrentInput.mousePosition;
		public static Vector2 MousePosition => CurrentInput.mousePosition;
		public static int MouseWheel => CurrentInput.mouseWheel;
		//Keyboard
		public static string InputString => CurrentInput.inputString;

		internal static InputVariables CurrentInput => Game.fixedUpdate ? fixedInput : renderInput;
		internal static InputVariables PrevInput => Game.fixedUpdate ? prevFixedInput : prevRenderInput;

		internal static void Init()
		{
			fixedInput = new InputVariables();
			renderInput = new InputVariables();
			prevFixedInput = new InputVariables();
			prevRenderInput = new InputVariables();

			InitTriggers();
			InitCallbacks();

			SingletonInputTrigger.StaticInit();
		}
		internal static void Update()
		{
			CurrentInput.Update();

			UpdateTriggers();
		}
		internal static void LateUpdate()
		{
			CurrentInput.inputString = string.Empty;

			CurrentInput.CopyTo(PrevInput);
		}

		private static void CheckSpecialCombinations()
		{
			if(GetKeyDown(Keys.F4) && (GetKey(Keys.LeftAlt) || GetKey(Keys.RightAlt))) {
				Game.Quit();
			}

			/*if(GetKeyDown(Keys.Enter) && (GetKey(Keys.LAlt) || GetKey(Keys.RAlt))) {
				Screen.Fullscreen = !Screen.Fullscreen;
			}*/
		}
	}
}