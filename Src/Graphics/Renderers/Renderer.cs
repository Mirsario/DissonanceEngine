using System;
using System.Collections.Generic;
using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Graphics.Shaders;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.Renderers
{
	public abstract class Renderer : Component
	{
		private static readonly List<Renderer> Renderers = new List<Renderer>();

		public static int RendererCount {
			get {
				lock(Renderers) {
					return Renderers.Count;
				}
			}
		}

		public Func<bool?> PreCullingModifyResult { get; set; }
		public Func<bool,bool> PostCullingModifyResult { get; set; }

		public abstract Material Material { get; set; }

		protected override void OnEnable()
		{
			lock(Renderers) {
				Renderers.Add(this);
			}
		}
		protected override void OnDisable()
		{
			lock(Renderers) {
				Renderers.Remove(this);
			}
		}

		protected override void OnDispose()
		{
			lock(Renderers) {
				Renderers.Remove(this);
			}

			Material = null;
		}

		public virtual void ApplyUniforms(Shader shader) { }

		public abstract bool GetRenderData(Vector3 rendererPosition,Vector3 cameraPosition,out Material material,out Bounds bounds,out object renderObject);
		public abstract void Render(object renderObject);

		public static IEnumerable<Renderer> EnumerateRenderers()
		{
			lock(Renderers) {
				foreach(var renderer in Renderers) {
					yield return renderer;
				}
			}
		}
	}
}