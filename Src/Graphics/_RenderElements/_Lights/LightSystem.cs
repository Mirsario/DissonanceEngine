namespace Dissonance.Engine.Graphics;

[Callback<RenderingCallback>]
public sealed partial class LightSystem : GameSystem
{
	[EntitySubsystem]
	partial void UpdateEntities(in Light light, in Transform transform, [FromGlobal] ref LightingPassData lightingPassData)
	{
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
