using System.Collections.Generic;
using System.Linq;
using GameEngine;
using AbyssCrusaders.DataStructures;

namespace AbyssCrusaders
{
	public static class TilesetHelper
	{
		/*public const byte None = 0;		//00000000
		public const byte TopLeft = 1;		//10000000
		public const byte Top = 2;			//01000000
		public const byte TopRight = 4;		//00100000
		public const byte Right = 8;		//00010000
		public const byte BottomRight = 16;	//00001000
		public const byte Bottom = 32;		//00000100
		public const byte BottomLeft = 64;	//00000010
		public const byte Left = 128;		//00000001*/

		public static byte[] CalculateMasks(bool? topLeft,bool? top,bool? topRight,bool? left,bool? right,bool? bottomLeft,bool? bottom,bool? bottomRight)
		{
			var bytes = new List<BitsByte> {
				new BitsByte(
					topLeft==true,		top==true,		topRight==true,
					left==true,							right==true,
					bottomLeft==true,	bottom==true,	bottomRight==true
				)
			};

			var values = new[] {
				topLeft,	top,	topRight,
				left,				right,
				bottomLeft,	bottom,	bottomRight
			};

			for(int i = 0;i<8;i++) {
				var value = values[i];
				if(value==null) {
					for(int j = 0;j<bytes.Count;j += 2) {
						var bitsByte = bytes[j];
						bitsByte[i] = true;
						bytes.Insert(0,bitsByte);
					}
				}
			}

			return bytes.Select(bb => (byte)bb).ToArray();
		}
		public static Vector2UShort[] CreateTileset(Vector2UShort defaultFrame,Dictionary<Vector2UShort,byte[]> frameDictionary = null)
		{
			var array = new Vector2UShort[byte.MaxValue+1];

			for(int i = 0;i<array.Length;i++) {
				array[i] = defaultFrame;
			}

			if(frameDictionary!=null) {
				foreach(var pair in frameDictionary) {
					Vector2UShort frame = pair.Key;
					byte[] arr = pair.Value;

					for(int i = 0;i<arr.Length;i++) {
						array[arr[i]] = frame;
					}
				}
			}

			return array;
		}

		public static WallDrawInfo[][] CreateWallset(
			WallDrawInfo frameTopLeft,		WallDrawInfo frameTop,		WallDrawInfo frameTopRight,
			WallDrawInfo frameLeft,			WallDrawInfo frameCenter,	WallDrawInfo frameRight,
			WallDrawInfo frameBottomLeft,	WallDrawInfo frameBottom,	WallDrawInfo frameBottomRight,
			Dictionary<byte[],WallDrawInfo[]> manualOverrides = null
		){
			var result = new WallDrawInfo[byte.MaxValue+1][];
			var frameList = new List<WallDrawInfo>();
			for(int i = 0;i<=byte.MaxValue;i++) {
				BitsByte bitsByte = new BitsByte((byte)i);

				bool topLeft = bitsByte[0];		bool top = bitsByte[1];		bool topRight = bitsByte[2];
				bool left = bitsByte[3];									bool right = bitsByte[4];
				bool bottomLeft = bitsByte[5];	bool bottom = bitsByte[6];	bool bottomRight = bitsByte[7];

				if(!left && !topLeft && !top) { frameList.Add(frameTopLeft); }				if(!top) { frameList.Add(frameTop); }		if(!right && !topRight && !top) { frameList.Add(frameTopRight); }
				if(!left) { frameList.Add(frameLeft); }										frameList.Add(frameCenter);					if(!right) { frameList.Add(frameRight); }
				if(!left && !bottomLeft && !bottom) { frameList.Add(frameBottomLeft); }		if(!bottom) { frameList.Add(frameBottom); }	if(!right && !bottomRight && !bottom) { frameList.Add(frameBottomRight); }

				result[i] = frameList.ToArray();

				frameList.Clear();
			}

			if(manualOverrides!=null) {
				foreach(var pair in manualOverrides) {
					byte[] mask = pair.Key;
					WallDrawInfo[] drawSteps = pair.Value;
					for(int i = 0;i<mask.Length;i++) {
						result[mask[i]] = drawSteps;
					}
				}
			}

			return result;
		}

		public static Vector2UShort[] FrameArray(params (ushort x,ushort y)[] tuples)
		{
			var result = new Vector2UShort[tuples.Length];

			for(int i = 0;i<tuples.Length;i++) {
				(ushort x, ushort y) = tuples[i];
				result[i] = new Vector2UShort(x,y);
			}

			return result;
		}

		/*public static WallDrawInfo[][] CreateWallset(WallDrawInfo[] defaultDrawSteps,Dictionary<byte[],WallDrawInfo[]> frameDictionary = null)
		{
			var result = new WallDrawInfo[byte.MaxValue+1][];
			for(int i = 0;i<result.Length;i++) {
				result[i] = defaultDrawSteps;
			}
			if(frameDictionary!=null) {
				foreach(var pair in frameDictionary) {
					byte[] mask = pair.Key;
					WallDrawInfo[] drawSteps = pair.Value;
					for(int i = 0;i<mask.Length;i++) {
						result[mask[i]] = drawSteps;
					}
				}
			}
			return result;
		}*/
	}
}
