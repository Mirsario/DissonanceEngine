using System;
using System.IO;

namespace SurvivalGame
{
	public struct Tile
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
	}
}
