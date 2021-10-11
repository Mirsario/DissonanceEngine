using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine.Input
{
	partial class InputEngine
	{
		// Mouse
		
		public static bool GetMouseButton(MouseButton button)
			=> CurrentInput.MouseButtons[(int)button];
		
		public static bool GetMouseButtonDown(MouseButton button)
			=> CurrentInput.MouseButtons[(int)button] && !PrevInput.MouseButtons[(int)button];
		
		public static bool GetMouseButtonUp(MouseButton button)
			=> !CurrentInput.MouseButtons[(int)button] && PrevInput.MouseButtons[(int)button];
		
		// Keys
		
		public static bool GetKey(Keys key)
			=> CurrentInput.PressedKeys.ContainsKey(key);
		
		public static bool GetKeyDown(Keys key)
			=> CurrentInput.PressedKeys.ContainsKey(key) && !PrevInput.PressedKeys.ContainsKey(key);
		
		public static bool GetKeyUp(Keys key)
			=> !CurrentInput.PressedKeys.ContainsKey(key) && PrevInput.PressedKeys.ContainsKey(key);
		
		// Directions
		
		public static float GetDirection(Keys negative, Keys positive)
			=> (GetKey(negative) ? -1f : 0f) + (GetKey(positive) ? 1f : 0f);
		
		public static Vector2 GetDirection(Keys up, Keys down, Keys left, Keys right) => new(
			(GetKey(right) ? 1f : 0f) - (GetKey(left) ? 1f : 0f),
			(GetKey(up) ? 1f : 0f) - (GetKey(down) ? 1f : 0f)
		);

		public static Vector2 GetDirection(InputTrigger up, InputTrigger down, InputTrigger left, InputTrigger right) => new(
			(right.IsPressed ? 1f : 0f) - (left.IsPressed ? 1f : 0f),
			(up.IsPressed ? 1f : 0f) - (down.IsPressed ? 1f : 0f)
		);
	}
}
