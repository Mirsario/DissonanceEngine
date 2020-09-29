using Dissonance.Engine.Core;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine.Input
{
	public sealed partial class InputEngine : EngineModule
	{
		public const int MaxMouseButtons = 12;
		public const int MaxGamepads = 4;

		//Mouse
		public static Vector2 MouseDelta => PrevInput.mousePosition - CurrentInput.mousePosition;
		public static Vector2 MousePosition => CurrentInput.mousePosition;
		public static int MouseWheel => CurrentInput.mouseWheel;
		//Keyboard
		public static string InputString => CurrentInput.inputString;

		internal static InputEngine Instance => Game.Instance.GetModule<InputEngine>();
		internal static InputVariables CurrentInput => Game.IsFixedUpdate ? Instance.fixedInput : Instance.renderInput;
		internal static InputVariables PrevInput => Game.IsFixedUpdate ? Instance.prevFixedInput : Instance.prevRenderInput;

		internal InputVariables fixedInput;
		internal InputVariables renderInput;
		internal InputVariables prevFixedInput;
		internal InputVariables prevRenderInput;

		private Windowing windowing;

		protected override void Init()
		{
			Game.TryGetModule(out windowing);

			fixedInput = new InputVariables();
			renderInput = new InputVariables();
			prevFixedInput = new InputVariables();
			prevRenderInput = new InputVariables();

			InitSignals();
			InitTriggers();

			if(!Game.NoWindow) {
				InitCallbacks();
			}

			SingletonInputTrigger.StaticInit();
		}
		protected override void PreFixedUpdate() => PreUpdate();
		protected override void PostFixedUpdate() => PostUpdate();
		protected override void PreRenderUpdate() => PreUpdate();
		protected override void PostRenderUpdate() => PostUpdate();
		protected override void OnDispose() => windowing = null;

		private void PreUpdate()
		{
			if(Game.NoWindow) {
				return;
			}

			CurrentInput.Update();

			UpdateTriggers();

			CheckSpecialCombinations();
		}
		private void PostUpdate()
		{
			if(Game.NoWindow) {
				return;
			}

			CurrentInput.inputString = string.Empty;
			CurrentInput.mouseWheel = 0;

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