using System;
using System.Collections.Generic;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	[Reads<GeometryPassData>]
	[Reads<Transform>]
	[Reads<Sprite>]
	[Writes<GeometryPassData>]
	public sealed class SpriteSystem : GameSystem
	{
		private class BatchData : IDisposable
		{
			public int EntityCount;
			public int NextEntityId;
			public Entity[] Entities;
			public Material Material;
			public LayerMask LayerMask;
			public Mesh CompoundMesh;

			public void Dispose()
			{
				CompoundMesh.Dispose();
			}
		}

		private EntitySet entities;
		private Dictionary<ulong, BatchData> batches;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Sprite>() && e.Has<Transform>());
			batches = new();
		}

		protected internal override void RenderUpdate()
		{
			var entitiesSpan = entities.ReadEntities();
			ref var geometryPassData = ref GlobalGet<GeometryPassData>();

			static ulong GetBatchIndex(uint materialId, uint layerId)
				=> (layerId << 32) | materialId;

			// Enumerate entities to define batches and count entities for them
			foreach (var entity in entitiesSpan) {
				if (entity.Get<Sprite>().Material?.TryGetOrRequestValue(out var material) != true) {
					continue;
				}

				var layer = entity.Has<Layer>() ? entity.Get<Layer>() : Layers.DefaultLayer;
				ulong batchKey = GetBatchIndex((uint)material.Id, (uint)layer.Index);

				if (!batches.TryGetValue(batchKey, out var batch)) {
					batches[batchKey] = batch = new() {
						Material = material,
						LayerMask = layer.Mask
					};
				}

				batch.EntityCount++;
			}

			var batchKeysToRemove = new List<ulong>();

			// Prepare batches
			foreach (var pair in batches) {
				var batch = pair.Value;

				// Mark unused last-frame batches for removal
				if (batch.EntityCount == 0) {
					batchKeysToRemove.Add(pair.Key);
					continue;
				}

				// Create entity arrays for batches
				batch.Entities = new Entity[batch.EntityCount];
				// Reset entity count, as it'll be reused as a counter on the second entity enumeration
				batch.EntityCount = 0;
			}

			// Cleanup batches that existed in the previous frame but are unneeded in this one
			foreach (ulong key in batchKeysToRemove) {
				if (batches.Remove(key, out var batch)) {
					batch.Dispose();
				}
			}

			// Enumerate entities for the second time to fill batches' entity arrays
			foreach (var entity in entitiesSpan) {
				if (entity.Get<Sprite>().Material?.TryGetOrRequestValue(out var material) != true) {
					continue;
				}

				uint layerId = entity.Has<Layer>() ? (uint)entity.Get<Layer>().Index : 0u;
				ulong batchKey = GetBatchIndex((uint)material.Id, layerId);
				var batchData = batches[batchKey];

				batchData.Entities[batchData.EntityCount++] = entity;
			}

			// Enumerate and run batches
			foreach (var pair in batches) {
				ulong key = pair.Key;
				var batch = pair.Value;

				if (batch.CompoundMesh == null) {
					batch.CompoundMesh = new Mesh() {
						BufferUsage = BufferUsageHint.DynamicDraw
					};
				}

				batchKeysToRemove.Remove(key);

				var compoundMesh = batch.CompoundMesh;
				var entities = batch.Entities;

				var vertices = compoundMesh.Vertices = new Vector3[entities.Length * 4];
				var uv0 = compoundMesh.Uv0 = new Vector2[entities.Length * 4];
				uint[] indices = compoundMesh.Indices = new uint[entities.Length * 6];

				uint vertex = 0;
				uint index = 0;

				foreach (var entity in entities) {
					ref var sprite = ref entity.Get<Sprite>();
					var transform = entity.Get<Transform>();
					var mesh = PrimitiveMeshes.Quad;
					var worldMatrix = transform.WorldMatrix;

					var sourceRect = sprite.SourceRectangle;
					var sourceUV = new Vector4(
						sourceRect.X,
						sourceRect.Y,
						sourceRect.Right,
						sourceRect.Bottom
					);
					var uvPoints = sprite.Effects switch {
						Sprite.SpriteEffects.FlipHorizontally | Sprite.SpriteEffects.FlipVertically => new Vector4(sourceUV.Z, sourceUV.W, sourceUV.X, sourceUV.Y),
						Sprite.SpriteEffects.FlipHorizontally => new Vector4(sourceUV.Z, sourceUV.Y, sourceUV.X, sourceUV.W),
						Sprite.SpriteEffects.FlipVertically => new Vector4(sourceUV.X, sourceUV.W, sourceUV.Z, sourceUV.Y),
						_ => sourceUV
					};

					if (sprite.verticesNeedRecalculation) {
						RecalculateVertices(ref sprite);
					}

					vertices[vertex] = worldMatrix * new Vector3(sprite.vertices.X, sprite.vertices.Y, 0f);
					vertices[vertex + 1] = worldMatrix * new Vector3(sprite.vertices.Z, sprite.vertices.Y, 0f);
					vertices[vertex + 2] = worldMatrix * new Vector3(sprite.vertices.Z, sprite.vertices.W, 0f);
					vertices[vertex + 3] = worldMatrix * new Vector3(sprite.vertices.X, sprite.vertices.W, 0f);

					uv0[vertex] = new Vector2(uvPoints.X, uvPoints.W);
					uv0[vertex + 1] = new Vector2(uvPoints.Z, uvPoints.W);
					uv0[vertex + 2] = new Vector2(uvPoints.Z, uvPoints.Y);
					uv0[vertex + 3] = new Vector2(uvPoints.X, uvPoints.Y);

					indices[index] = vertex + 2;
					indices[index + 1] = vertex + 1;
					indices[index + 2] = vertex;
					indices[index + 3] = vertex + 3;
					indices[index + 4] = vertex + 2;
					indices[index + 5] = vertex;

					vertex += 4;
					index += 6;
				}

				compoundMesh.Apply();

				geometryPassData.RenderEntries.Add(new(new Transform(), compoundMesh, batch.Material, batch.LayerMask));

				batch.EntityCount = 0; // Reset entity count once again, will be used for disposing the batch in the next frame
			}
		}

		private static void RecalculateVertices(ref Sprite sprite)
		{
			float xSize = sprite.FrameSize.X * sprite.PixelSize;
			float ySize = sprite.FrameSize.Y * sprite.PixelSize;
			float yOrigin = 1f - sprite.Origin.Y;

			sprite.vertices = new Vector4(
				-sprite.Origin.X * xSize,
				-yOrigin * ySize,
				(1f - sprite.Origin.X) * xSize,
				(1f - yOrigin) * ySize
			);
			sprite.verticesNeedRecalculation = false;
		}
	}
}
