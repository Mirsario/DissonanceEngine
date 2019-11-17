using AbyssCrusaders.Core.DataStructures;
using System.Collections.Generic;
using System.IO;

namespace AbyssCrusaders.Core
{
	public interface ILootSource
	{
		IEnumerable<(int itemId,int amount)> GetLoot();
	}
}
