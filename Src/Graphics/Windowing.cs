using Silk.NET.Core.Contexts;
using Silk.NET.GLFW;

namespace Dissonance.Engine.Graphics;

public abstract class Windowing : EngineModule, IGLContextSource
{
	public delegate void KeyCallback(Keys key, int scanCode, InputAction action, KeyModifiers mods);
	public delegate void CharCallback(uint codePoint);
	public delegate void ScrollCallback(double xOffset, double yOffset);
	public delegate void MouseButtonCallback(MouseButton button, InputAction action, KeyModifiers mods);
	public delegate void CursorPositionCallback(double x, double y);

	public Vector2 WindowCenter => WindowLocation + WindowSize * 0.5f;
	public RectInt WindowRectangle => new(WindowLocation, WindowSize);

	public abstract event CursorPositionCallback OnCursorPositionCallback;
	public abstract event MouseButtonCallback OnMouseButtonCallback;
	public abstract event ScrollCallback OnScrollCallback;
	public abstract event KeyCallback OnKeyCallback;
	public abstract event CharCallback OnCharCallback;

	public abstract IGLContext GLContext { get; }
	public abstract Vector2Int WindowSize { get; }
	public abstract Vector2Int WindowLocation { get; }
	public abstract Vector2Int FramebufferSize { get; }
	public abstract CursorState CursorState { get; set; }
	public abstract bool ShouldClose { get; set; }

	public abstract void SwapBuffers();
	public abstract bool SetVideoMode(int width, int height);
	public abstract Vector2 GetCursorPosition();
}
