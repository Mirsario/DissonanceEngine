using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics;

public class Renderbuffer
{
	public static Renderbuffer ActiveBuffer { get; private set; }

	public readonly uint Id;
	public readonly string Name;

	internal InternalFormat storageType;

	public Renderbuffer(string name, InternalFormat storageType)
	{
		Name = name;
		Id = OpenGL.GenRenderbuffer();
		this.storageType = storageType;

		OpenGL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Id);
		OpenGL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, this.storageType, (uint)Screen.Width, (uint)Screen.Height);
	}

	public static void Bind(Renderbuffer newBuffer)
	{
		OpenGL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, newBuffer?.Id ?? 0);

		ActiveBuffer = newBuffer;
	}
}
