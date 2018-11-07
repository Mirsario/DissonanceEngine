using System;
using System.Collections.Generic;	
using System.Runtime.InteropServices;
using Matrix3 = OpenTK.Matrix3;

namespace GameEngine
{
	public struct Rect
	{
		public float x;
		public float y;
		public float width;
		public float height;
		
		public float Left => x;
		public float Top => y;
		public float Right => x+width;
		public float Bottom => y+height;
		public int X => (int)x;
		public int Y => (int)y;
		public int Width => (int)width;
		public int Height => (int)height;

		public Rect(float X,float Y,float Z,float W,bool fromPoints = false)
		{
			x = X;
			y = Y;
			if(fromPoints) {
				width = Z-X;
				height = W-Y;
			}else{
				width = Z;
				height = W;
			}
		}
		public bool Contains(Vector2 point,bool notOnBorders = false)
		{
			if(notOnBorders) {
				return point.x>Left && point.x<Right && point.y>Top && point.y<Bottom;
			}
			return point.x>=Left && point.x<=Right && point.y>=Top && point.y<=Bottom;
		}
	}
}