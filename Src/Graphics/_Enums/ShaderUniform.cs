using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class ShaderUniform
	{
		public string Name { get; set; }
		public ActiveUniformType Type { get; set; }
		public int Location { get; set; }

		public ShaderUniform(string name, ActiveUniformType type, int location)
		{
			Name = name;
			Type = type;
			Location = location;
		}
	}
}
