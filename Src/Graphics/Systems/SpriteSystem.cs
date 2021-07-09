using System.Collections.Generic;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	[Reads(typeof(Transform), typeof(Sprite))]
	[Writes(typeof(GeometryPassData))]
	public sealed class SpriteSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Sprite>() && e.Has<Transform>());
		}

		protected internal override void RenderUpdate()
		{
			ref var geometryPassData = ref GlobalGet<GeometryPassData>();

			var meshesByMaterial = new Dictionary<Material, Mesh>();
			var entitiesByMaterial = new Dictionary<Material, List<Entity>>();

			foreach(var entity in entities.ReadEntities()) {
				var sprite = entity.Get<Sprite>();
				var material = sprite.Material;

				if(material == null) {
					continue;
				}

				if(!entitiesByMaterial.TryGetValue(material, out var entityList)) {
					entitiesByMaterial[material] = entityList = new();
				}

				entityList.Add(entity);
			}

			foreach(var pair in entitiesByMaterial) {
				var material = pair.Key;
				var entities = pair.Value;

				if(!meshesByMaterial.TryGetValue(material, out var compoundMesh)) {
					meshesByMaterial[material] = compoundMesh = new();
				}

				var vertices = compoundMesh.Vertices = new Vector3[entities.Count * 4];
				var uv0 = compoundMesh.Uv0 = new Vector2[entities.Count * 4];
				uint[] indices = compoundMesh.Indices = new uint[entities.Count * 6];

				uint vertex = 0;
				uint index = 0;

				foreach(var entity in entities) {
					ref var sprite = ref entity.Get<Sprite>();
					var transform = entity.Get<Transform>();
					var mesh = PrimitiveMeshes.Quad;

					var sourceRect = sprite.SourceRectangle;
					var sourceUV = new Vector4(
						sourceRect.x,
						sourceRect.y,
						sourceRect.Right,
						sourceRect.Bottom
					);
					var uvPoints = sprite.Effects switch {
						Sprite.SpriteEffects.FlipHorizontally | Sprite.SpriteEffects.FlipVertically => new Vector4(sourceUV.z, sourceUV.w, sourceUV.x, sourceUV.y),
						Sprite.SpriteEffects.FlipHorizontally => new Vector4(sourceUV.z, sourceUV.y, sourceUV.x, sourceUV.w),
						Sprite.SpriteEffects.FlipVertically => new Vector4(sourceUV.x, sourceUV.w, sourceUV.z, sourceUV.y),
						_ => sourceUV
					};

					if(sprite.verticesNeedRecalculation) {
						RecalculateVertices(ref sprite);
					}

					var vertexPoints = sprite.vertices;
					var position = transform.Position;

					vertices[vertex] = position + new Vector3(sprite.vertices.x, sprite.vertices.y, 0f);
					vertices[vertex + 1] = position + new Vector3(sprite.vertices.z, sprite.vertices.y, 0f);
					vertices[vertex + 2] = position + new Vector3(sprite.vertices.z, sprite.vertices.w, 0f);
					vertices[vertex + 3] = position + new Vector3(sprite.vertices.x, sprite.vertices.w, 0f);

					uv0[vertex] = new Vector2(uvPoints.x, uvPoints.w);
					uv0[vertex + 1] = new Vector2(uvPoints.z, uvPoints.w);
					uv0[vertex + 2] = new Vector2(uvPoints.z, uvPoints.y);
					uv0[vertex + 3] = new Vector2(uvPoints.x, uvPoints.y);

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

				geometryPassData.RenderEntries.Add(new(Transform.Default, compoundMesh, material));
			}
		}

		private static void RecalculateVertices(ref Sprite sprite)
		{
			float xSize = sprite.FrameSize.x * sprite.PixelSize;
			float ySize = sprite.FrameSize.y * sprite.PixelSize;
			float yOrigin = 1f - sprite.Origin.y;

			sprite.vertices = new Vector4(
				-sprite.Origin.x * xSize,
				-yOrigin * ySize,
				(1f - sprite.Origin.x) * xSize,
				(1f - yOrigin) * ySize
			);
			sprite.verticesNeedRecalculation = false;
		}
	}
}
