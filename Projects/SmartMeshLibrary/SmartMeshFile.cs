using System.Collections.Generic;

namespace SmartMeshLibrary
{
	public class SmartMeshFile : ChunkContainer
	{
		public List<SmartMeshObject> objects;

		public SmartMeshFile(IEnumerable<SmartMeshChunk> chunksToAdd = null) : base(chunksToAdd)
		{
			
		}
		public override void Dispose()
		{
			base.Dispose();
			if(objects!=null) {
				for(int i = 0;i<objects.Count;i++) {
					objects[i]?.Dispose();
				}
				objects.Clear();
				objects = null;
			}
		}
	}
}
