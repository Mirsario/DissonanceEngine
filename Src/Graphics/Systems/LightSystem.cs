namespace Dissonance.Engine.Graphics
{
	[Reads<LightingPassData>]
	[Reads<Light>]
	[Reads<Transform>]
	[Writes<LightingPassData>]
	public sealed class LightSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Light>() && e.Has<Transform>());
		}

		protected internal override void RenderUpdate()
		{
			ref var lightingPassData = ref GlobalGet<LightingPassData>();

			foreach (var entity in entities.ReadEntities()) {
				var light = entity.Get<Light>();
				var transform = entity.Get<Transform>();
				LightingPassData.LightData lightData = default;

				lightData.Type = light.Type;
				lightData.Intensity = light.Intensity;
				lightData.Color = light.Color;
				lightData.Matrix = Matrix4x4.Identity;

				switch (lightData.Type) {
					case Light.LightType.Point:
						lightData.Range = light.Range;
						lightData.Position = transform.Position;
						lightData.Matrix = Matrix4x4.CreateScale(light.Range) * Matrix4x4.CreateTranslation(lightData.Position.Value);
						break;
					case Light.LightType.Directional:
						lightData.Direction = transform.Forward;
						break;
				}

				lightingPassData.Lights.Add(lightData);
			}
		}
	}
}
