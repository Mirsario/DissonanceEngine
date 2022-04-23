using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public abstract class CallbackSystem : GameSystem
	{
		private readonly LinkedList<GameSystem> invocationList = new();

		private bool needsSorting;

		public IEnumerable<GameSystem> InvocationList {
			get {
				SortSystemsIfNeeded();

				return invocationList;
			}
		}

		protected sealed override void Execute()
		{
			SortSystemsIfNeeded();

			foreach (var subscriber in invocationList) {
				subscriber.Update();
			}
		}

		internal void AddSystem(GameSystem system)
		{
			invocationList.AddLast(system);

			needsSorting = true;
		}

		private void SortSystemsIfNeeded()
		{
			if (needsSorting) {
				SortSystems();

				needsSorting = false;
			}
		}

		private void SortSystems()
		{
			IEnumerable<int> GetDependencyIndices(GameSystem system)
			{
				foreach (var systemType in system.TypeData.SortingDependencies) {
					int index = 0;

					foreach (var invokedSystem in invocationList) {
						if (invokedSystem.GetType() == systemType) {
							yield return index;
						}

						index++;
					}
				}
			}

			lock (invocationList) {
				// Performance could've been better.
				var sortedArray = new GameSystem[invocationList.Count];
				int index = 0;

				foreach (var system in invocationList) {
					sortedArray[index++] = system;
				}

				sortedArray.DependencySort(GetDependencyIndices, throwOnRecursion: true);

				invocationList.Clear();

				for (int i = 0; i < sortedArray.Length; i++) {
					invocationList.AddLast(sortedArray[i]);
				}
			}
		}
	}
}
