using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	[Reads(typeof(Light), typeof(Transform))]
	[Writes(typeof(LightingPassData))]
	public sealed class LightSystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Light>() && e.Has<Transform>());
		}

		public override void RenderUpdate()
		{
			ref var lightingPassData = ref GlobalGet<LightingPassData>();

			foreach(var entity in entities.ReadEntities()) {
				var light = entity.Get<Light>();
				var transform = entity.Get<Transform>();
				LightingPassData.LightData lightData = default;

				lightData.Type = light.Type;
				lightData.Intensity = light.Intensity;
				lightData.Color = light.Color;

				switch(lightData.Type) {
					case Light.LightType.Point:
						lightData.Range = light.Range;
						lightData.Position = transform.Position;
						lightData.Matrix = Matrix4x4.CreateScale(light.Range) * Matrix4x4.CreateTranslation(lightData.Position.Value);
						break;
				}


				lightingPassData.Lights.Add(lightData);
			}
		}
	}
}
