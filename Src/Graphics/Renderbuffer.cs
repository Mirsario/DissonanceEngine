using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class Renderbuffer
	{
		public static Renderbuffer ActiveBuffer { get; private set; }

		public readonly uint Id;
		public readonly string Name;

		internal RenderbufferStorage storageType;

		public Renderbuffer(string name, RenderbufferStorage storageType)
		{
			Name = name;
			Id = GL.GenRenderbuffer();
			this.storageType = storageType;

			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Id);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, this.storageType, Screen.Width, Screen.Height);
		}

		public static void Bind(Renderbuffer newBuffer)
		{
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, newBuffer?.Id ?? 0);

			ActiveBuffer = newBuffer;
		}
	}
}
