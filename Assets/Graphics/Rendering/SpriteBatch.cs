using System;

namespace GameEngine.Graphics
{
	public class SpriteBatch : IDisposable
	{
		protected static readonly Vector4 DefaultUvPoints = new Vector4(0f,1f,1f,0f);

		protected Mesh bufferMesh;
		protected int vertexIndex;
		protected int triangleIndex;
		protected bool began;

		public SpriteBatch()
		{
			bufferMesh = new Mesh();
		}

		public void Begin(int bufferSize)
		{
			if(began) {
				throw new InvalidOperationException($"Cannot call {nameof(Begin)} before {nameof(End)} has been called after the previous {nameof(Begin)} call.");
			}

			int vertexCount = bufferSize*4;
			int triangleCount = bufferSize*6;

			bufferMesh.Vertices = new Vector3[vertexCount];
			bufferMesh.Uv0 = new Vector2[vertexCount];
			bufferMesh.triangles = new int[triangleCount];

			vertexIndex = triangleIndex = 0;

			began = true;
		}
		public void End()
		{
			if(!began) {
				throw new InvalidOperationException($"Cannot call {nameof(End)} before {nameof(Begin)} has been called.");
			}

			bufferMesh.Apply();
			bufferMesh.Render();

			bufferMesh.Vertices = null;
			bufferMesh.Uv0 = null;
			bufferMesh.triangles = null;

			began = false;
		}
		public void Draw(RectFloat dstRect,float depth = 0f) => Draw(dstRect.Points,DefaultUvPoints,depth);
		public void Draw(RectFloat dstRect,RectFloat srcRect,float depth = 0f) => Draw(dstRect.Points,new Vector4(srcRect.x,srcRect.y+srcRect.height,srcRect.x+srcRect.width,srcRect.y),depth);
		public void Draw(Vector4 vertexPoints,Vector4 uvPoints,float depth = 0f)
		{
			if(!began) {
				throw new InvalidOperationException($"Cannot call draw functions before {nameof(Begin)} has been called.");
			}

			var vertices = bufferMesh.Vertices;
			var uv0 = bufferMesh.Uv0;
			var triangles = bufferMesh.triangles;

			vertices[vertexIndex  ] = new Vector3(vertexPoints.x,vertexPoints.y,depth);
			vertices[vertexIndex+1] = new Vector3(vertexPoints.x,vertexPoints.w,depth);
			vertices[vertexIndex+2] = new Vector3(vertexPoints.z,vertexPoints.w,depth);
			vertices[vertexIndex+3] = new Vector3(vertexPoints.z,vertexPoints.y,depth);

			uv0[vertexIndex  ] = new Vector2(uvPoints.x,uvPoints.w);
			uv0[vertexIndex+1] = new Vector2(uvPoints.x,uvPoints.y);
			uv0[vertexIndex+2] = new Vector2(uvPoints.z,uvPoints.y);
			uv0[vertexIndex+3] = new Vector2(uvPoints.z,uvPoints.w);

			triangles[triangleIndex++] = vertexIndex;
			triangles[triangleIndex++] = vertexIndex+1;
			triangles[triangleIndex++] = vertexIndex+2;
			triangles[triangleIndex++] = vertexIndex;
			triangles[triangleIndex++] = vertexIndex+2;
			triangles[triangleIndex++] = vertexIndex+3;

			vertexIndex += 4;
		}
		public void Dispose()
		{
			bufferMesh.Dispose();
		}
	}
}
