namespace AbyssCrusaders.Core.Test
{
	public class CursorLightObj : LightObj
	{
		public override void RenderUpdate()
		{
			Position = Main.MouseWorld;
		}
	}
}