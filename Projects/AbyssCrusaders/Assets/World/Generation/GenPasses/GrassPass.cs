using AbyssCrusaders.Tiles;
using GameEngine;

namespace AbyssCrusaders.Generation.GenPasses
{
	public class GrassPass : GenPass
	{
		public override void Run(int seed,int index,World world)
		{
			ushort dirt = TilePreset.GetTypeId<Dirt>();
			ushort grass = TilePreset.GetTypeId<Grass>();
			ushort tallGrass = TilePreset.GetTypeId<TallGrass>();

			for(int x = 0;x<world.width;x++) {
				for(int y = 1;y<world.height;y++) {
					ref var tile = ref world[x,y];

					if(tile.type==dirt && world[x,y-1].type==0) {
						tile.type = grass;

						world[x,y-1].type = tallGrass;
						world[x,y-1].style = (byte)Rand.Next(4);
					}
				}
			}
		}
	}
}
