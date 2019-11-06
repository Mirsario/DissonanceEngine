using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbyssCrusaders.Core
{
	public abstract class PlacingRule
	{
		public abstract bool CanPlace(int x,int y,int width,int height);
		public abstract void GetTileEntityShape(ref bool[,] shapeArray,int width,int height);
	}
}
