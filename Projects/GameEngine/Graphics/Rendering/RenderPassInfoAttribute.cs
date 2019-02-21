using System;

namespace GameEngine.Graphics
{
	public class RenderPassInfoAttribute : Attribute
	{
		public static readonly RenderPassInfoAttribute Default;
		
		public bool requiresShader;
		public string[] acceptedShaderNames;

		static RenderPassInfoAttribute()
		{
			Default = new RenderPassInfoAttribute {
				requiresShader = false,
				acceptedShaderNames = null
			};
		}
	}
}