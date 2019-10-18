using GameEngine;

namespace AbyssCrusaders.Generation.GenPasses
{
	public class TunnelsPass : GenPass
	{
		public ushort tileType;
		public int numTunnels;
		public int minSteps;
		public int maxSteps;
		public int minSize;
		public int maxSize;
		public float stepLength;
		public float maxDegreeTurn;

		public TunnelsPass(int numTunnels,int minSteps,int maxSteps,int minSize,int maxSize,float stepLength = 4f,float maxDegreeTurn = 30f,ushort tileType = 0)
		{
			this.numTunnels = numTunnels;
			this.minSteps = minSteps;
			this.maxSteps = maxSteps;
			this.minSize = minSize;
			this.maxSize = maxSize;
			this.stepLength = stepLength;
			this.maxDegreeTurn = maxDegreeTurn;
			this.tileType = tileType;
		}
		
		public override void Run(int seed,int index,World world)
		{
			//ushort wood = TilePreset.GetTypeId<Wood>();
			//GenUtils.Ellipse(world,128,128,32,32,(ref Tile t) => t.type = wood);
			//GenUtils.Ellipse(world,128,128,24,24,(ref Tile t) => t.type = 0);
			int i = 0;
			while(i<numTunnels) {
				int X = Rand.Next(world.width);
				int Y = Rand.Next(world.height);
				if(world[X,Y].type==0) {
					continue;
				}
				int size = Rand.Range(minSize,maxSize+1);
				int numSteps = Rand.Range(minSteps,maxSteps+1);
				Vector2 position = new Vector2(X,Y);
				Vector2 direction = new Vector2(Rand.Range(-1f,1f),Rand.Range(-1f,1f)).Normalized;

				for(int j = 0;j<numSteps;j++) {
					int x = (int)position.x;
					int y = (int)position.y;

					GenUtils.Ellipse(world,x,y,size,size,(ref Tile tile) => tile.type = tileType);

					position += direction;
					if(position.x<0f || position.y<0f || position.x>=world.width || position.y>=world.height) {
						break;
					}
					direction = Vector2.Rotate(direction,Rand.Range(-maxDegreeTurn,maxDegreeTurn));
				}
				i++;
			}
		}
	}
}
