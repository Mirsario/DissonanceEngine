using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	partial class Shader
	{
		//TODO: Finish this

		public void SetFloat(string uniformName, float value)
		{
			SetShader(this);

			if (uniforms.TryGetValue(uniformName, out var uniform)) {
				GL.Uniform1(uniform.location, value);
			}
		}

		public void SetVector2(string uniformName, Vector2 value)
		{
			SetShader(this);

			if (uniforms.TryGetValue(uniformName, out var uniform)) {
				GL.Uniform2(uniform.location, value.x, value.y);
			}
		}
	}
}
