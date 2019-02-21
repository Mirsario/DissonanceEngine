using GameEngine;

namespace AbyssCrusaders.Generation.GenPasses
{
	public class TileFramePass : GenPass
	{
		public override void Run(World world)
		{
			for(int y = 0;y<world.height;y++) {
				for(int x = 0;x<world.width;x++) {
					world.TileFrame(x,y);
				}
			}
			Debug.Log("Tileframes set");
		}
	}
}
