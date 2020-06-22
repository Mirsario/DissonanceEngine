using System;

namespace Dissonance.Engine.Graphics.RenderPasses
{
	public class RenderPassInfoAttribute : Attribute
	{
		public static readonly RenderPassInfoAttribute Default;

		public bool RequiresShader { get; set; }
		public string[] AcceptedShaderNames { get; set; }

		static RenderPassInfoAttribute()
		{
			Default = new RenderPassInfoAttribute {
				RequiresShader = false,
				AcceptedShaderNames = null
			};
		}
	}
}