using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class TexTest : Entity, IHasMaterial
	{
		public MeshRenderer renderer;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");

			static Vector3 RandomColor() => new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized;

			renderer = AddComponent<MeshRenderer>(c => c.Mesh = PrimitiveMeshes.Cube);
			AddComponent<BoxCollider>();
			AddComponent<Rigidbody>(c => c.Mass = 1f);
			AddComponent<Light>(c => {
				c.color = RandomColor();
				c.range *= 0.5f;
			});

			var resolution = new Vector2Int(32,32);
			
			GLDraw.DrawDelayed(() => {
				using var framebuffer = new Framebuffer("Test");

				var texture = new RenderTexture("test",resolution.x,resolution.y,FilterMode.Point,TextureWrapMode.Clamp,false);
				framebuffer.AttachRenderTexture(texture);
				GLDraw.SetRenderTarget(framebuffer);

				GLDraw.SetShader(Resources.Find<Shader>("GLDrawTest"));
				GLDraw.SetTextures(new Dictionary<string,Texture>() {
					{ "mainTex",Resources.Get<Texture>("Dirt.png") },
					{ "secondTex",Resources.Get<Texture>("Grass1.png") },
				});


				//GLDraw.ClearColor(new Vector4(n).Normalized,1f));
				GLDraw.Clear(ClearMask.ColorBufferBit);
				GLDraw.Viewport(0,0,resolution.x,resolution.y);

				GLDraw.Begin(PrimitiveType.Quads);

				GLDraw.TexCoord2(0f,1f); GLDraw.Color3(RandomColor()); GLDraw.Vertex2(-1f, 1f);
				GLDraw.TexCoord2(0f,0f); GLDraw.Color3(RandomColor()); GLDraw.Vertex2(-1f,-1f);
				GLDraw.TexCoord2(1f,0f); GLDraw.Color3(RandomColor()); GLDraw.Vertex2( 1f,-1f);
				GLDraw.TexCoord2(1f,1f); GLDraw.Color3(RandomColor()); GLDraw.Vertex2( 1f, 1f);

				GLDraw.End();

				GLDraw.SetShader(null);
				GLDraw.SetRenderTarget(null);

				Debug.Log("Drawing finished!");
				Rendering.CheckGLErrors();
				Debug.Log("Checked errors!");
				
				var mat = new Material("textest",Resources.Find<Shader>("DiffuseColor"));
				mat.SetVector3("color",new Vector3(1f,1f,1f));
				mat.SetTexture("mainTex",texture);
				renderer.Material = mat;
			});

		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}