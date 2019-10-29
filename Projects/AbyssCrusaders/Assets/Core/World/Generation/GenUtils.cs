namespace AbyssCrusaders.Core.Generation
{
	public static class GenUtils
	{
		public delegate void TileAction(ref Tile tile);
		
		public static void Ellipse(World world,int xCenter,int yCenter,int width,int height,TileAction action)
		{
			float widthSqr = width*width;
			float heightSqr = height*height;
			float widthHeightSqr = widthSqr*heightSqr;

			for(int y = -height+1;y<height;y++) {
				for(int x = -width+1;x<width;x++) {
					int X = xCenter+x;
					int Y = yCenter+y;
					if(X>=0 && Y>=0 && X<world.width && Y<world.height && x*x*heightSqr+y*y*widthSqr<=widthHeightSqr) {
						action(ref world[xCenter+x,yCenter+y]);
					}
				}
			}
		}
	}
}
