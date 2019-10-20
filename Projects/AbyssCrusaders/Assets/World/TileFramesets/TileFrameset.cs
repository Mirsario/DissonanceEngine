using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;
using AbyssCrusaders.DataStructures;

namespace AbyssCrusaders
{
	public abstract class TileFrameset : IDisposable //ICloneable, 
	{
		//Static
		public static Dictionary<Type,TileFrameset> byType;
		
		//Instance
		public readonly string Name;
		public int tileTextureSize;
		public int tileTextureFrameSize;
		public Vector2UShort[][] tileFramesets;
		public WallDrawInfo[][] wallFrameset;

		protected TileFrameset()
		{
			Name = GetType().Name;

			tileTextureSize = tileTextureFrameSize = 10;

			OnInit();

			if(tileTextureSize<=0) {
				throw new Exception($"Value of {tileTextureSize} is invalid. Set it properly in {GetType().Name}.OnInit()");
			}
			if(tileTextureFrameSize<=0) {
				throw new Exception($"Value of {tileTextureFrameSize} is invalid. Set it properly in {GetType().Name}.OnInit()");
			}
		}

		public abstract void OnInit();

		public void Dispose()
		{
			tileFramesets = null;
			wallFrameset = null;
		}

		/*public WallDrawInfo[] GetWallDrawInfo(int x,int y,byte frame)
		{
			var variants = wallFrameset[frame];

			var drawInfo = variants[0]; //TODO:

			return drawInfo;
		}*/

		public static void Initialize()
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			Type thisType = typeof(TileFrameset);
			Type[] tileTypes = assembly.GetTypes().Where(t => t!=thisType && !t.IsAbstract && thisType.IsAssignableFrom(t)).ToArray();

			byType = new Dictionary<Type,TileFrameset>();
			for(ushort i = 0;i<tileTypes.Length;i++) {
				var type = tileTypes[i];
				byType[type] = (TileFrameset)Activator.CreateInstance(type);
			}
		}

		public static T GetInstance<T>() where T : TileFrameset => (T)byType[typeof(T)];
	}
}
