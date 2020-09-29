using Dissonance.Engine.Core;
using Dissonance.Engine.Core.ProgrammableEntities;
using Dissonance.Engine.Graphics.Shaders;
using Dissonance.Engine.Graphics.UserInterface;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics.RenderPasses.Default
{
	public class GUIPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(null);

			Shader.SetShader(Rendering.GUIShader);

			GL.Enable(EnableCap.Blend);
			Rendering.SetBlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GUI.canDraw = true;

			Game.Instance?.OnGUI();
			ProgrammableEntityManager.InvokeHook(nameof(ProgrammableEntity.OnGUI));

			GUI.canDraw = false;

			GL.Disable(EnableCap.Blend);
		}
	}
}