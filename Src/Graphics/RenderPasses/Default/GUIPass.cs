using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class GUIPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(null);

			Shader.SetShader(Rendering.GUIShader.GetValueImmediately());

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);

			GUI.canDraw = true;

			Game.Instance?.OnGUI();
			// ComponentManager.OnGUI();

			GUI.canDraw = false;

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.CullFace);
		}
	}
}
