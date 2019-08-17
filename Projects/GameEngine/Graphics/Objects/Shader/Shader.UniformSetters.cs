using OpenTK.Graphics.OpenGL;

namespace GameEngine.Graphics
{
	partial class Shader
	{
		//TODO: Finish this

		public void SetFloat(string uniform,float value)
		{
			SetShader(this);
			GL.Uniform1(uniforms[uniform].location,value);
		}
		public void SetVector2(string uniform,Vector2 value)
		{
			SetShader(this);
			GL.Uniform2(uniforms[uniform].location,1,new[] { value.x,value.y });
		}
	}
}