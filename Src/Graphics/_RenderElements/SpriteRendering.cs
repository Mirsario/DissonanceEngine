﻿using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics;

public static partial class SpriteRendering
{
	private class BatchData : IDisposable
	{
		public int EntityCount;
		public int LastEntityCount;
		public Entity[] EntityBuffer;
		public Material Material;
		public LayerMask LayerMask;
		public Mesh CompoundMesh;

		public void Dispose()
		{
			CompoundMesh.Dispose();
		}
	}

	private struct BatchDataContainer
	{
		public Dictionary<ulong, BatchData> Batches = new();

		public BatchDataContainer() { }
	}

	[System, CalledIn<EngineInitialization>]
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	static partial void PrepareSpriteBatching()
	{
		Global.Set(new BatchDataContainer());
	}

	// Enumerate entities to define batches and fill their entity arrays
	[EntitySystem, CalledIn<Render>, Tags("RenderData", "SpriteBatching")]
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	static partial void PopulateSpriteBatches(Entity entity, in Sprite _, [FromGlobal] in BatchDataContainer batchDataContainer)
	{
		if (entity.Get<Sprite>().Material?.TryGetOrRequestValue(out var material) != true) {
			return;
		}

		var batches = batchDataContainer.Batches;
		var layer = entity.Has<Layer>() ? entity.Get<Layer>() : Layers.DefaultLayer;
		ulong batchKey = GetBatchIndex((uint)material.Id, (uint)layer.Index);

		if (!batches.TryGetValue(batchKey, out var batch)) {
			batches[batchKey] = batch = new() {
				Material = material,
				LayerMask = layer.Mask
			};
		}

		int entityIndex = batch.EntityCount++;
		int currentSize = batch.EntityBuffer?.Length ?? 0;

		if (currentSize <= entityIndex) {
			int newSize = Math.Max(currentSize, 8);

			while (newSize <= entityIndex) {
				newSize *= 2;
			}

			Array.Resize(ref batch.EntityBuffer, newSize);
		}

		batch.EntityBuffer[entityIndex] = entity;
	}

	// Enumerate and run batches, pushing them to geometry data, and removing ones without data.
	[System, CalledIn<Render>, Tags("RenderData", "SpritePush"), RequiresTags("SpriteBatching")]
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	static partial void PushSpriteBatches([FromGlobal] in BatchDataContainer batchDataContainer, [FromGlobal] ref GeometryPassData geometryPassData)
	{
		var batches = batchDataContainer.Batches;
		List<ulong> batchKeysToRemove = null;

		foreach (var pair in batches) {
			ulong key = pair.Key;
			var batch = pair.Value;

			if (batch.EntityCount == 0) {
				if (batch.LastEntityCount == 0) {
					(batchKeysToRemove ??= new()).Add(key);
				}

				return;
			}

			if (batch.CompoundMesh == null) {
				batch.CompoundMesh = new Mesh() {
					BufferUsage = BufferUsageARB.DynamicDraw
				};
			}

			var compoundMesh = batch.CompoundMesh;
			var vertices = compoundMesh.Vertices = new Vector3[batch.EntityCount * 4];
			var uv0 = compoundMesh.Uv0 = new Vector2[batch.EntityCount * 4];
			uint[] indices = compoundMesh.Indices = new uint[batch.EntityCount * 6];

			uint vertex = 0;
			uint index = 0;

			for (int i = 0; i < batch.EntityCount; i++) {
				Entity entity = batch.EntityBuffer[i];

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

			// Reset entity count
			batch.LastEntityCount = batch.EntityCount;
			batch.EntityCount = 0;
		}

		// Dispose unused batches
		if (batchKeysToRemove != null) {
			foreach (ulong key in batchKeysToRemove) {
				if (batches.Remove(key, out var batch)) {
					batch.Dispose();
				}
			}
		}
	}

	private static ulong GetBatchIndex(uint materialId, uint layerId)
		=> (layerId << 32) | materialId;

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
