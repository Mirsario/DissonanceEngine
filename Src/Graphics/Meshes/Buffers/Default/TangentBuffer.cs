﻿using System;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.Meshes.Buffers.Default
{
	public class TangentBuffer : CustomVertexBuffer<Vector4>
	{
		public void Recalculate(Vector2[] uv = null)
		{
			var indices = mesh.Indices ?? throw new InvalidOperationException($"{nameof(Recalculate)}() requires {nameof(mesh.Indices)} array to be ready.");
			var vertices = mesh.Vertices ?? throw new InvalidOperationException($"{nameof(Recalculate)}() requires {nameof(VertexBuffer)} to be ready.");
			var normals = mesh.Normals ?? throw new InvalidOperationException($"{nameof(Recalculate)}() requires {nameof(NormalBuffer)} to be ready. Call {nameof(NormalBuffer)}.{nameof(NormalBuffer.Recalculate)}() first?");

			uv ??= mesh.Uv0 ?? throw new InvalidOperationException($"{nameof(Recalculate)}() requires {nameof(Uv0Buffer)} to be ready, or a ready UV map to be passed through '{nameof(uv)}' parameter.");

			int vertexCount = vertices.Length;

			var tan1 = new Vector3[vertexCount];
			var tan2 = new Vector3[vertexCount];

			data = new Vector4[vertexCount];

			for(int i = 0; i < indices.Length; i += 3) {
				uint i1 = indices[i];
				uint i2 = indices[i + 1];
				uint i3 = indices[i + 2];

				var v1 = vertices[i1];
				var v2 = vertices[i2];
				var v3 = vertices[i3];

				var w1 = uv[i1];
				var w2 = uv[i2];
				var w3 = uv[i3];

				float x1 = v2.x - v1.x;
				float x2 = v3.x - v1.x;
				float y1 = v2.y - v1.y;
				float y2 = v3.y - v1.y;
				float z1 = v2.z - v1.z;
				float z2 = v3.z - v1.z;

				float s1 = w2.x - w1.x;
				float s2 = w3.x - w1.x;
				float t1 = w2.y - w1.y;
				float t2 = w3.y - w1.y;

				float r = 1f / (s1 * t2 - s2 * t1);
				var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
				var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;

				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;
			}

			for(int i = 0; i < vertexCount; i++) {
				var n = normals[i];
				var t = tan1[i];

				//Gram-Schmidt orthogonalization
				float w = Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0f ? -1f : 1f;

				data[i] = new Vector4((t - n * Vector3.Dot(n, t)).Normalized, w);
			}
		}
	}
}
