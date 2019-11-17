using OpenTK.Graphics.OpenGL;
using GLRenderbufferStorage = OpenTK.Graphics.OpenGL.RenderbufferStorage;

namespace GameEngine.Graphics
{
	public class Renderbuffer
	{
		internal static Renderbuffer activeBuffer;
		public static Renderbuffer ActiveBuffer => activeBuffer;

		public readonly int Id;
		public readonly string Name;
		internal RenderbufferStorage storageType;
		
		public Renderbuffer(string name,RenderbufferStorage storageType)
		{
			Name = name;
			Id = GL.GenRenderbuffer();
			this.storageType = storageType;

			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer,Id);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,(GLRenderbufferStorage)this.storageType,Screen.Width,Screen.Height);
		}

		public static void Bind(Renderbuffer newBuffer)
		{
			if(newBuffer!=activeBuffer) {
				GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer,newBuffer?.Id ?? 0);
				activeBuffer = newBuffer;
			}
		}
	}
}