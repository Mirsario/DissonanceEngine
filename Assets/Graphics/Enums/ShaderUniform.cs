using Dissonance.Framework.OpenGL;

namespace GameEngine.Graphics
{
	internal class ShaderUniform
	{
		public string name;
		public ActiveUniformType type;
		public int location;

		public ShaderUniform(string name,ActiveUniformType type,int location)
		{
			this.name = name;
			this.type = type;
			this.location = location;
		}
	}
}