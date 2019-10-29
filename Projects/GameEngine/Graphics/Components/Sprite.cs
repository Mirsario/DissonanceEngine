using OpenTK.Graphics.OpenGL;
using GameEngine.Graphics;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine
{
	public class Sprite : Renderer
	{
		public enum SpriteEffects
		{
			FlipHorizontally = 1,
			FlipVertically = 2
		}

		protected static readonly Bounds DefaultBounds = new Bounds(Vector3.Zero,Vector3.One);

		public SpriteEffects spriteEffects;

		protected bool useMesh = true;
		protected RectFloat sourceRectangle = RectFloat.Default;
		protected Vector4 sourceUV = new Vector4(0f,0f,1f,1f);
		protected Material material;

		public RectFloat SourceRectangle {
			get => sourceRectangle;
			set {
				sourceRectangle = value;
				sourceUV = new Vector4(value.x,value.y,value.Right,value.Bottom);
				useMesh = value.x==0f && value.y==0f && value.width==1f && value.height==1f;
			}
		}

		public override Material Material {
			get => material;
			set => material = value;
		}

		public override bool GetRenderData(Vector3 rendererPosition,Vector3 cameraPosition,out Material material,out Bounds bounds,out object renderObject)
		{
			material = this.material;

			if(useMesh) {
				var mesh = spriteEffects switch {
					SpriteEffects.FlipHorizontally|SpriteEffects.FlipVertically => PrimitiveMeshes.quadXYFlipped,
					SpriteEffects.FlipHorizontally => PrimitiveMeshes.quadXFlipped,
					SpriteEffects.FlipVertically => PrimitiveMeshes.quadXFlipped,
					_ => PrimitiveMeshes.quad
				};

				bounds = mesh.bounds;
				renderObject = mesh;
			} else {
				bounds = DefaultBounds;
				renderObject = null;
			}

			return true;
		}
		public override void Render(object renderObject)
		{
			if(useMesh) {
				((Mesh)renderObject).DrawMesh();
			} else {
				int uvAttrib = (int)AttributeId.Uv0;

				Vector4 uvPoints = spriteEffects switch {
					SpriteEffects.FlipHorizontally|SpriteEffects.FlipVertically => new Vector4(sourceUV.z,sourceUV.w,sourceUV.x,sourceUV.y),
					SpriteEffects.FlipHorizontally => new Vector4(sourceUV.z,sourceUV.y,sourceUV.x,sourceUV.w),
					SpriteEffects.FlipVertically => new Vector4(sourceUV.x,sourceUV.w,sourceUV.z,sourceUV.y),
					_ => sourceUV
				};

				GL.Begin(PrimitiveType.Quads);

				GL.VertexAttrib2(uvAttrib,uvPoints.x,uvPoints.w); GL.Vertex2(-0.5f,-0.5f);
				GL.VertexAttrib2(uvAttrib,uvPoints.x,uvPoints.y); GL.Vertex2(-0.5f, 0.5f);
				GL.VertexAttrib2(uvAttrib,uvPoints.z,uvPoints.y); GL.Vertex2( 0.5f, 0.5f);
				GL.VertexAttrib2(uvAttrib,uvPoints.z,uvPoints.w); GL.Vertex2( 0.5f,-0.5f);

				GL.End();
			}
		}
	}
}