using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics;

public class GUIPass : RenderPass
{
	public override void Render()
	{
		Framebuffer.BindWithDrawBuffers(null);

		Shader.SetShader(Rendering.GUIShader.GetValueImmediately());

		OpenGL.Enable(EnableCap.Blend);
		OpenGL.Enable(EnableCap.CullFace);
		OpenGL.CullFace(CullFaceMode.Back);

		GUI.canDraw = true;

		Game.Instance?.OnGUI();
		// ComponentManager.OnGUI();

		GUI.canDraw = false;

		OpenGL.Disable(EnableCap.Blend);
		OpenGL.Disable(EnableCap.CullFace);
	}
}
