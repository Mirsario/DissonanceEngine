using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbyssCrusaders.Core.DataStructures
{
	public struct LootInfo : ILootSource
	{
		public int itemId;
		public int minItemAmount;
		public int maxItemAmount;

		public LootInfo(int itemId) : this(itemId,1,1) {}
		public LootInfo(int itemId,int minItemAmount) : this(itemId,minItemAmount,minItemAmount) {}
		public LootInfo(int itemId,int minItemAmount,int maxItemAmount)
		{
			this.itemId = itemId;
			this.minItemAmount = minItemAmount;
			this.maxItemAmount = maxItemAmount;
		}

		public IEnumerable<(int,int)> GetLoot()
		{
			yield return (itemId,minItemAmount==maxItemAmount ? minItemAmount : Rand.Range(minItemAmount,maxItemAmount));
		}
	}
}
