using System;
using System.Collections.Generic;
using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Graphics.Shaders;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.Renderers
{
	public abstract class Renderer : Component
	{
		public Func<bool?> PreCullingModifyResult { get; set; }
		public Func<bool,bool> PostCullingModifyResult { get; set; }

		public abstract Material Material { get; set; }

		protected override void OnDispose()
		{
			Material = null;
		}

		public virtual void ApplyUniforms(Shader shader) { }

		public abstract bool GetRenderData(Vector3 rendererPosition,Vector3 cameraPosition,out Material material,out Bounds bounds,out object renderObject);
		public abstract void Render(object renderObject);
	}
}