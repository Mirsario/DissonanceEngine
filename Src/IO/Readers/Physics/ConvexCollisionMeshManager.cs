/*
using System.Threading.Tasks;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.Physics;

namespace Dissonance.Engine.IO
{
	public class ConvexCollisionMeshManager : IAssetReader<ConvexCollisionMesh>
	{
		public string[] Extensions { get; } = { ".obj" };

		public async ValueTask<ConvexCollisionMesh> ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
		{
			string assetPath = assetFile.Path;
			var mesh = Assets.Get<Mesh>(assetPath, AssetRequestMode.ImmediateLoad).Value;
			var collisionMesh = new ConvexCollisionMesh();

			collisionMesh.SetupFromMesh(mesh);

			return collisionMesh;
		}
	}
}
*/
