using GameEngine;

namespace AbyssCrusaders
{
	public class GameInput
	{
		//Movement
		public static InputTrigger moveX;
		public static InputTrigger moveY;
		public static InputTrigger jump;
		public static InputTrigger sprint;
		//Use
		public static InputTrigger primaryUse;
		public static InputTrigger secondaryUse;
		public static InputTrigger mmb;
		//Other
		public static InputTrigger zoom;

		//public static float mouseSensitivity = 1f/10f;

		public static void Initialize()
		{
			moveX = Input.RegisterTrigger("MoveX",new InputBinding[] { "-A","+D","GamePad0 Axis0" },-1f,1f);
			moveY = Input.RegisterTrigger("MoveY",new InputBinding[] { "+S","-W","-GamePad0 Axis1" },-1f,1f);
			jump = Input.RegisterTrigger("Jump",new InputBinding[] { "Space","GamePad0 Button0","GamePad0 Button7" });
			sprint = Input.RegisterTrigger("Sprint",new InputBinding[] { "LShift","GamePad0 Button6" });

			primaryUse = Input.RegisterTrigger("Primary Use",new[] { MouseButton.Left,new InputBinding("GamePad0 Axis5",deadZone:0.5f) });
			secondaryUse = Input.RegisterTrigger("Secondary Use",new[] { MouseButton.Right,new InputBinding("GamePad0 Axis2",deadZone:0.5f) });
			mmb = Input.RegisterTrigger("Middle Mouse Button",new InputBinding[] { MouseButton.Middle });
			
			zoom = Input.RegisterTrigger("Zoom",new InputBinding[] { "-GamePad0 Button12","+GamePad0 Button11","Mouse ScrollWheel" });
		}
	}
}