using GameEngine;

namespace Game
{
	public class GameInput
	{
		//Movement
		public static float mouseSensitivity = 1f/10f;
		public static InputTrigger moveX;
		public static InputTrigger moveY;
		public static InputTrigger lookX;
		public static InputTrigger lookY;
		public static InputTrigger jump;
		public static InputTrigger sprint;
		//
		public static InputTrigger primaryUse;
		public static InputTrigger secondaryUse;

		public static void Initialize()
		{
			moveX = Input.RegisterTrigger("MoveX",new InputBinding[] { "-A","+D","-GamePad0 Button13","+GamePad0 Button14","GamePad0 Axis0" },-1f,1f);
			moveY = Input.RegisterTrigger("MoveY",new InputBinding[] { "-S","+W","-GamePad0 Button12","+GamePad0 Button11","GamePad0 Axis1" },-1f,1f);
			lookX = Input.RegisterTrigger("LookX",new[] { new InputBinding("MouseX",mouseSensitivity,0f),"GamePad0 Axis3","-Left","+Right" });
			lookY = Input.RegisterTrigger("LookY",new[] { new InputBinding("MouseY",mouseSensitivity,0f),"GamePad0 Axis4","-Up","+Down" });
			jump = Input.RegisterTrigger("Jump",new InputBinding[] { "Space","GamePad0 Button0","GamePad0 Button7" });
			sprint = Input.RegisterTrigger("Sprint",new InputBinding[] { "LShift","GamePad0 Button6" });

			primaryUse = Input.RegisterTrigger("Primary Use",new[] { MouseButton.Left,new InputBinding("GamePad0 Axis5",deadZone:0.5f) });
			secondaryUse = Input.RegisterTrigger("Secondary Use",new[] { MouseButton.Right,new InputBinding("GamePad0 Axis2",deadZone:0.5f) });
		}
	}
}