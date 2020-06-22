using System;
using Dissonance.Engine.Graphics.Textures;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Utils.Extensions
{
	//TODO: Move regions into their own extension classes
	public static class PixelExtensions
	{
		//Textures
		public static void CopyPixels(this Pixel[,] from,RectInt? sourceRect,Pixel[,] to,Vector2Int destPoint,Func<Pixel,Pixel,Pixel> mixFunc = null)
		{
			if(mixFunc==null) {
				mixFunc = (pxlA,pxlB) => pxlB;
			}

			var srcRect = sourceRect ?? new RectInt(0,0,from.GetLength(0),from.GetLength(1));
			int xLength1 = from.GetLength(0);
			int yLength1 = from.GetLength(1);
			int xLength2 = to.GetLength(0);
			int yLength2 = to.GetLength(1);

			for(int y = 0;y<srcRect.height;y++) {
				for(int x = 0;x<srcRect.width;x++) {
					int X1 = srcRect.x+x;
					int Y1 = srcRect.y+y;
					int X2 = destPoint.x+x;
					int Y2 = destPoint.y+y;

					if(X1>=0 && Y1>=0 && X2>=0 && Y2>=0 && X1<xLength1 && Y1<yLength1 && X2<xLength2 && Y2<yLength2) {
						//to[X2,Y2] = from[X1,Y1];
						to[X2,Y2] = mixFunc(to[X2,Y2],from[X1,Y1]);
					}
				}
			}
		}
	}
}