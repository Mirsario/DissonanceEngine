namespace AbyssCrusaders
{
	public class CursorLightObj : LightObj
	{
		public override void RenderUpdate()
		{
			Position = Main.MouseWorld;
		}
	}
}