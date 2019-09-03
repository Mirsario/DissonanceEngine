//#define LOOP_WORLD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine;
using GameEngine.Physics;

namespace SurvivalGame
{
	public partial class World
	{
		public static void SaveWorld(World w,string path)
		{
			//WIP
			using var stream = File.OpenWrite(path);
			using var writer = new BinaryWriter(stream);

			writer.Write(FileHeader);
			writer.Write(w.worldName);
			writer.Write(w.worldDisplayName);
			writer.Write(w.xSize);
			writer.Write(w.ySize);

			//Make a (chunkId > dataPos) map here

			for(int y = 0;y<w.ySizeInChunks;y++) {
				for(int x = 0;x<w.xSizeInChunks;x++) {
					w.chunks[x,y].Save(writer);
				}
			}
		}
		public static T LoadWorld<T>(string path) where T : World
		{
			//WIP
			using var stream = File.OpenRead(path);
			using var reader = new BinaryReader(stream);

			if(!ReadInfoHeader(reader,out var info)) {
				throw new IOException("World is corrupt.");
			}

			var w = Instantiate<T>(info.displayName,info.xSize,info.ySize,path);

			//Make a (chunkId > dataIOPosition) map here?

			for(int y = 0;y<w.ySizeInChunks;y++) {
				for(int x = 0;x<w.xSizeInChunks;x++) {
					var chunk = w.chunks[x,y] = Chunk.Create(w,x,y);
					chunk.Load(reader);
				}
			}

			w.IsReady = true;
			Main.MainMenu = false;

			return null;
		}
		public static bool ReadInfoHeader(BinaryReader reader,out WorldInfo info)
		{
			info = default;
			try {
				if(string.Compare(new string(reader.ReadChars(16)),new string(FileHeader))!=0) {
					return false;
				}
				info.name = reader.ReadString();
				info.displayName = reader.ReadString();
				info.xSize = reader.ReadInt32();
				info.ySize = reader.ReadInt32();
				if(info.xSize%Chunk.ChunkSize!=0 || info.ySize%Chunk.ChunkSize!=0) {
					return false;
				}
				return true;
			}
			catch {
				return false;
			}
		}
	}
}

