using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public static class DrawUtils
	{
		private static Mesh bufferMesh;

		public static void DrawQuadUv0()
		{
			PrimitiveMeshes.ScreenQuad.Value.Render();
		}

		public static void DrawQuadUv0(Vector4 vertices, Vector4 uv0)
		{
			bufferMesh.Vertices[0] = new Vector3(vertices.X, vertices.Y, 0f);
			bufferMesh.Vertices[1] = new Vector3(vertices.X, vertices.W, 0f);
			bufferMesh.Vertices[2] = new Vector3(vertices.Z, vertices.W, 0f);
			bufferMesh.Vertices[3] = new Vector3(vertices.Z, vertices.Y, 0f);

			bufferMesh.Uv0[0] = new Vector2(uv0.X, uv0.W);
			bufferMesh.Uv0[1] = new Vector2(uv0.X, uv0.Y);
			bufferMesh.Uv0[2] = new Vector2(uv0.Z, uv0.Y);
			bufferMesh.Uv0[3] = new Vector2(uv0.Z, uv0.W);

			bufferMesh.Apply();
			bufferMesh.Render();
		}

		internal static void Init()
		{
			bufferMesh?.Dispose();

			bufferMesh = new Mesh {
				Vertices = new Vector3[4],
				Uv0 = new Vector2[4],
				Indices = new uint[] {
					0,1,2,
					0,2,3
				},
				BufferUsage = BufferUsageHint.StreamDraw
			};
		}
	}
}
