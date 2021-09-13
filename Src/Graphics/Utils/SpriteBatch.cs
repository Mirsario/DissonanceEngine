using System;

namespace Dissonance.Engine.Graphics
{
	public class SpriteBatch : IDisposable
	{
		protected static readonly Vector4 DefaultUvPoints = new(0f, 1f, 1f, 0f);

		protected Mesh bufferMesh;
		protected int vertexCount;
		protected int indexCount;
		protected bool began;

		public SpriteBatch()
		{
			bufferMesh = new Mesh();
		}

		public void Begin(int initialBufferCapacity = 4)
		{
			if (began) {
				throw new InvalidOperationException($"Cannot call {nameof(Begin)} before {nameof(End)} has been called after the previous {nameof(Begin)} call.");
			}

			initialBufferCapacity = Math.Max(initialBufferCapacity, 1);

			int vertexCount = initialBufferCapacity * 4;
			int triangleCount = initialBufferCapacity * 6;

			bufferMesh.Vertices = new Vector3[vertexCount];
			bufferMesh.Uv0 = new Vector2[vertexCount];
			bufferMesh.Indices = new uint[triangleCount];

			this.vertexCount = indexCount = 0;

			began = true;
		}

		public void End()
		{
			if (!began) {
				throw new InvalidOperationException($"Cannot call {nameof(End)} before {nameof(Begin)} has been called.");
			}

			bufferMesh.Apply();
			bufferMesh.Render();

			bufferMesh.Vertices = null;
			bufferMesh.Uv0 = null;
			bufferMesh.Indices = null;

			began = false;
		}

		public void Draw(RectFloat dstRect, float depth = 0f)
			=> Draw(dstRect.Points, DefaultUvPoints, depth);

		public void Draw(RectFloat dstRect, RectFloat srcRect, float depth = 0f)
			=> Draw(dstRect.Points, srcRect.Points, depth);

		public void Draw(Vector4 vertexPoints, Vector4 uvPoints, float depth = 0f)
		{
			if (!began) {
				throw new InvalidOperationException($"Cannot call draw functions before {nameof(Begin)} has been called.");
			}

			int nextVertexCount = vertexCount + 4;
			int nextIndexCount = indexCount + 6;

			EnsureCapacity(nextVertexCount, nextIndexCount);

			var vertices = bufferMesh.Vertices;
			var uv0 = bufferMesh.Uv0;
			uint[] indices = bufferMesh.Indices;

			vertices[vertexCount] = new Vector3(vertexPoints.x, vertexPoints.y, depth);
			vertices[vertexCount + 1] = new Vector3(vertexPoints.x, vertexPoints.w, depth);
			vertices[vertexCount + 2] = new Vector3(vertexPoints.z, vertexPoints.w, depth);
			vertices[vertexCount + 3] = new Vector3(vertexPoints.z, vertexPoints.y, depth);

			uv0[vertexCount] = new Vector2(uvPoints.x, uvPoints.y);
			uv0[vertexCount + 1] = new Vector2(uvPoints.x, uvPoints.w);
			uv0[vertexCount + 2] = new Vector2(uvPoints.z, uvPoints.w);
			uv0[vertexCount + 3] = new Vector2(uvPoints.z, uvPoints.y);

			uint unsignedVertexCount = (uint)vertexCount;

			indices[indexCount] = unsignedVertexCount;
			indices[indexCount + 1] = unsignedVertexCount + 1;
			indices[indexCount + 2] = unsignedVertexCount + 2;
			indices[indexCount + 3] = unsignedVertexCount;
			indices[indexCount + 4] = unsignedVertexCount + 2;
			indices[indexCount + 5] = unsignedVertexCount + 3;

			vertexCount = nextVertexCount;
			indexCount = nextIndexCount;
		}

		public void Dispose()
		{
			bufferMesh.Dispose();

			GC.SuppressFinalize(this);
		}

		private void EnsureCapacity(int minVertexCount, int minIndexCount)
		{
			if (bufferMesh.Vertices.Length < minVertexCount) {
				int newSize = bufferMesh.Vertices.Length * 2;

				Array.Resize(ref bufferMesh.Vertices, newSize);
				Array.Resize(ref bufferMesh.Uv0, newSize);
			}

			if (bufferMesh.Indices.Length < minIndexCount) {
				Array.Resize(ref bufferMesh.Indices, bufferMesh.Indices.Length * 2);
			}
		}
	}
}
