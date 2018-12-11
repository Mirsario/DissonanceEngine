using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using GameEngine;

namespace Game2DTest
{
	public sealed class Tile : IDisposable
	{
		public TileType Type => TileType.byId[typeId];
		
		public ushort typeId;

		public Tile(ushort type)
		{
			typeId = type;
		}

		public void Save(BinaryWriter writer)
		{
			writer.Write(typeId);
		}
		public void Load(BinaryReader reader)
		{
			typeId = reader.ReadUInt16();
		}
		public void Dispose() {}
	}
}
