using System.Reflection;
using Dissonance.Engine.Graphics;

namespace Dissonance.Engine.Input;

[ModuleDependency<Windowing>(isOptional: true)]
public sealed partial class InputEngine : EngineModule
{
	public const int MaxMouseButtons = 12;
	public const int MaxGamepads = 4;

	internal static InputVariables fixedInput;
	internal static InputVariables renderInput;
	internal static InputVariables prevFixedInput;
	internal static InputVariables prevRenderInput;

	private static Windowing windowing;

	// Mouse
	public static Vector2 MouseDelta => PrevInput.MousePosition - CurrentInput.MousePosition;
	public static Vector2 MousePosition => CurrentInput.MousePosition;
	public static int MouseWheel => CurrentInput.MouseWheel;
	// Keyboard
	public static string InputString => CurrentInput.InputString;

	internal static InputVariables CurrentInput => GameEngine.InFixedUpdate ? fixedInput : renderInput;
	internal static InputVariables PrevInput => GameEngine.InFixedUpdate ? prevFixedInput : prevRenderInput;

	protected override void Init()
	{
		ModuleManagement.TryGetModule(out windowing);

		fixedInput = new InputVariables();
		renderInput = new InputVariables();
		prevFixedInput = new InputVariables();
		prevRenderInput = new InputVariables();

		InitSignals();
		InitTriggers();
		InitCallbacks();
	}

	protected override void InitializeForAssembly(Assembly assembly)
		=> InitTriggersForAssembly(assembly);

	protected override void PreFixedUpdate()
		=> PreUpdate();

	protected override void PostFixedUpdate()
		=> PostUpdate();

	protected override void PreRenderUpdate()
		=> PreUpdate();

	protected override void PostRenderUpdate()
		=> PostUpdate();

	protected override void OnDispose()
	{
		windowing = null;
	}

	private void PreUpdate()
	{
		if (windowing == null) {
			return;
		}

		CurrentInput.Update(windowing.GetCursorPosition());

		UpdateTriggers();

		CheckSpecialCombinations();
	}

	private void PostUpdate()
	{
		if (windowing == null) {
			return;
		}

		CurrentInput.InputString = string.Empty;
		CurrentInput.MouseWheel = 0;

		CurrentInput.CopyTo(PrevInput);
	}

	private static void CheckSpecialCombinations()
	{
		if (GetKeyDown(Keys.F4) && (GetKey(Keys.LeftAlt) || GetKey(Keys.RightAlt))) {
			Game.Quit();
		}

		/*if (GetKeyDown(Keys.Enter) && (GetKey(Keys.LAlt) || GetKey(Keys.RAlt))) {
			Screen.Fullscreen = !Screen.Fullscreen;
		}*/
	}
}
