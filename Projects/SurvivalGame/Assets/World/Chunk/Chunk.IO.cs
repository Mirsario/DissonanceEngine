//#define NO_CHUNK_LODS

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public partial class Chunk
	{
		public void Save(BinaryWriter writer)
		{
			for(int y=0;y<ChunkSize;y++) {
				for(int x=0;x<ChunkSize;x++) {
					tiles[x,y].Save(writer);
				}
			}
		}
		public void Load(BinaryReader reader)
		{
			for(int y=0;y<ChunkSize;y++) {
				for(int x=0;x<ChunkSize;x++) {
					tiles[x,y].Load(reader);
				}
			}
		}
	}
}
