using Dissonance.Engine.Graphics.Meshes;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public static class DrawUtils
	{
		private static Mesh bufferMesh;

		public static void DrawQuadUv0()
		{
			PrimitiveMeshes.ScreenQuad.Render();
		}
		public static void DrawQuadUv0(Vector4 vertices, Vector4 uv0)
		{
			bufferMesh.Vertices[0] = new Vector3(vertices.x, vertices.y, 0f);
			bufferMesh.Vertices[1] = new Vector3(vertices.x, vertices.w, 0f);
			bufferMesh.Vertices[2] = new Vector3(vertices.z, vertices.w, 0f);
			bufferMesh.Vertices[3] = new Vector3(vertices.z, vertices.y, 0f);

			bufferMesh.Uv0[0] = new Vector2(uv0.x, uv0.w);
			bufferMesh.Uv0[1] = new Vector2(uv0.x, uv0.y);
			bufferMesh.Uv0[2] = new Vector2(uv0.z, uv0.y);
			bufferMesh.Uv0[3] = new Vector2(uv0.z, uv0.w);

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
				bufferUsage = BufferUsageHint.StreamDraw
			};
		}
	}
}