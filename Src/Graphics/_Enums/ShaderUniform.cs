using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics;

public class ShaderUniform
{
	public string Name { get; set; }
	public UniformType Type { get; set; }
	public int Location { get; set; }

	public ShaderUniform(string name, UniformType type, int location)
	{
		Name = name;
		Type = type;
		Location = location;
	}
}
