using Dissonance.Framework.OpenGL;

namespace GameEngine.Graphics
{
	public class GUIPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(null);

			Shader.SetShader(Rendering.GUIShader);

			GL.Enable(EnableCap.Blend);
			Rendering.SetBlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
			GUI.canDraw = true;

			Game.instance.OnGUI();
			ProgrammableEntityHooks.InvokeHook(nameof(ProgrammableEntity.OnGUI));

			GUI.canDraw = false;

			GL.Disable(EnableCap.Blend);
		}
	}
}