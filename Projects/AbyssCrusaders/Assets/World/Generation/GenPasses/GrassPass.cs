using AbyssCrusaders.Tiles;

namespace AbyssCrusaders.Generation.GenPasses
{
	public class GrassPass : GenPass
	{
		public override void Run(World world)
		{
			ushort dirt = TilePreset.GetTypeId<Dirt>();
			ushort grass = TilePreset.GetTypeId<Grass>();

			bool Check(int x,int y) => (x>0 && world[x-1,y].type==0) || (x<world.width-1 && world[x+1,y].type==0) || (y>0 && world[x,y-1].type==0) || (y<world.height-1 && world[x,y+1].type==0);

			for(int x = 0;x<world.width;x++) {
				for(int y = 0;y<world.height;y++) {
					ref var tile = ref world[x,y];

					if(tile.type==dirt && Check(x,y)) {
						tile.type = grass;
					}
				}
			}
		}
	}
}
