using Dissonance.Engine.Core.ProgrammableEntities;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
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

			Game.Instance?.OnGUI();
			ProgrammableEntityManager.InvokeHook(nameof(ProgrammableEntity.OnGUI));

			GUI.canDraw = false;

			GL.Disable(EnableCap.Blend);
		}
	}
}