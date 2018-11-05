using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using GameEngine;

namespace Game
{
	public class Tile : IDisposable
	{
		public ushort type;
		public float height;

		public void Save(BinaryWriter writer)
		{
			writer.Write(type);
			writer.Write(height);
		}
		public void Load(BinaryReader reader)
		{
			type = reader.ReadUInt16();
			height = reader.ReadSingle();
		}
		public void Dispose() {}
	}
}
