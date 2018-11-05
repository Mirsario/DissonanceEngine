using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
{
	public class Sprite : Renderer
	{
		protected override void OnDispose()
		{
			Graphics.rendererList.Remove(this);
			Materials = null;
		}
		protected override void OnInit()
		{
			Materials = new Material[1];
			Material = Material.defaultMat;
		}
		public override void FixedUpdate()
		{
			
		}
		//TODO: is dis slew
		protected override Mesh GetRenderMesh(Vector3 rendererPosition,Vector3 cameraPosition) => PrimitiveMeshes.quad;
	}
}