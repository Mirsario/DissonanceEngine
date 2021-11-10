using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public interface IAssetManager<T>
	{
		ValueTask OnAssetAdded(Asset<T> asset);
	}
}
