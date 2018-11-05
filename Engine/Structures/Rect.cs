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
		
		public float Left	{get{return x;}}
		public float Top	{get{return y;}}
		public float Right	{get{return x+width;}}
		public float Bottom	{get{return y+height;}}
		public int X		{get{return(int)x;}}
		public int Y		{get{return(int)y;}}
		public int Width	{get{return(int)width;}}
		public int Height	{get{return(int)height;}}
		
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