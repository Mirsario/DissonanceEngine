using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbyssCrusaders.Core.DataStructures
{
	//TODO: Implement 'chooseOne' boolean, which'd have only one lootInfo proceed in GetLoot()
	public class LootTable : ILootSource
	{
		//public bool chooseOne;
		public (float chance,ILootSource lootSource)[] lootInfo;

		public LootTable(params (float chance,ILootSource lootSource)[] lootInfo)
		{
			this.lootInfo = lootInfo ?? throw new ArgumentNullException(nameof(lootInfo));
		}

		public IEnumerable<(int,int)> GetLoot()
		{
			foreach(var (chance,lootSource) in lootInfo) {
				if(chance>Rand.Next(1f)) {
					continue;
				}

				foreach(var tuple in lootSource.GetLoot()) {
					yield return tuple;
				}
			}
		}
	}
}
