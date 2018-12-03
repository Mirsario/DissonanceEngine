using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeshLibrary
{
	public abstract class ChunkContainer : IDisposable
	{
		public List<SmartMeshChunk> chunks;

		protected ChunkContainer(IEnumerable<SmartMeshChunk> chunksToAdd = null)
		{
			chunks = new List<SmartMeshChunk>();
			if(chunksToAdd!=null) {
				chunks.AddRange(chunksToAdd);
			}
		}

		public virtual void Dispose()
		{
			if(chunks!=null) {
				for(int i = 0;i<chunks.Count;i++) {
					chunks[i]?.Dispose();
				}
				chunks.Clear();
				chunks = null;
			}
		}

		public bool GetChunk(string name,out SmartMeshChunk result)
		{
			result = chunks.FirstOrDefault(c => c.name==name);
			return result!=null;
		}
		public bool GetChunks(string name,out SmartMeshChunk[] result)
		{
			result = chunks.Where(c => c.name==name).ToArray();
			return result!=null && result.Length>0;
		}
		public SmartMeshChunk GetChunk(string name) => chunks.FirstOrDefault(c => c.name==name);
		public SmartMeshChunk[] GetChunks(string name) => chunks.Where(c => c.name==name).ToArray();

		public TResult TryReadChunk<TResult>(string chunkName,Func<BinaryReader,TResult> func)
		{
			if(GetChunk(chunkName,out var chunk)) {
				using(var reader = chunk.GetReader()) {
					return func(reader);
				}
			}
			return default(TResult);
		}
		public void TryWriteChunk(string chunkName,Action<BinaryWriter> action)
		{
			var chunk = new SmartMeshChunk(chunkName,new MemoryStream());
			using(var writer = chunk.GetWriter()) {
				action(writer);
			}
			chunks.Add(chunk);
		}
	}
}
