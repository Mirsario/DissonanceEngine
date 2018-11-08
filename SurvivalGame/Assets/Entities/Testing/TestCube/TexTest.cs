using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class TexTest : Entity, IHasMaterial
	{
		public MeshRenderer renderer;
		public BoxCollider collider;
		public Rigidbody rigidbody;

		public RenderTexture texture;
		public Light light;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");
			collider = AddComponent<BoxCollider>();
			collider.size = new Vector3(0.2f,1f,0.2f);
			rigidbody = AddComponent<Rigidbody>();
			rigidbody.Mass = 1f;

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Cube;

			Vector3 RandomColor() => new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized;
			light = AddComponent<Light>();
			light.color = RandomColor();
			light.range *= 0.5f;

			var resolution = new Vector2Int(32,32);
			texture = new RenderTexture(resolution.x,resolution.y,FilterMode.Point,TextureWrapMode.Clamp,false);
			GLDraw.DrawDelayed(() => {
				GLDraw.SetRenderTarget(texture);
				GLDraw.SetShader(Resources.Find<Shader>("GLDrawTest"));
				GLDraw.SetTextures(new Dictionary<string,Texture>() {
					{ "mainTex",Resources.Get<Texture>("Dirt.png") },
					{ "secondTex",Resources.Get<Texture>("Grass1.png") },
				});


				//GLDraw.ClearColor(new Vector4(n).Normalized,1f));
				GLDraw.Clear(ClearMask.ColorBufferBit);
				GLDraw.Viewport(0,0,resolution.x,resolution.y);
				GLDraw.Begin(PrimitiveType.Quads);
				GLDraw.TexCoord2(0.0f,1.0f);
				GLDraw.Color3(RandomColor());
				GLDraw.Vertex2(	-1.0f,1.0f);
				GLDraw.TexCoord2(0.0f,0.0f);
				GLDraw.Color3(RandomColor());
				GLDraw.Vertex2(	-1.0f,-1.0f);
				GLDraw.TexCoord2(1.0f,0.0f);
				GLDraw.Color3(RandomColor());
				GLDraw.Vertex2(	1.0f,-1.0f);
				GLDraw.TexCoord2(1.0f,1.0f);
				GLDraw.Color3(RandomColor());
				GLDraw.Vertex2(	1.0f,1.0f);
				GLDraw.End();

				GLDraw.SetShader(null);
				GLDraw.SetRenderTarget(null);

				Debug.Log("Drawing finished!");
				Graphics.CheckGLErrors();
				Debug.Log("Checked errors!");
			});

			var mat = new Material("textest",Resources.Find<Shader>("Diffuse"));
			mat.SetVector3("color",new Vector3(1f,1f,1f));
			mat.SetTexture("mainTex",texture);
			renderer.Material = mat;
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}