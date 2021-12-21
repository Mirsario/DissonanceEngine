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
			IEnumerable<GameSystem> GetDependencies(GameSystem system)
			{
				foreach (var systemType in system.TypeData.SortingDependencies) {
					foreach (var invokedSystem in invocationList) {
						if (invokedSystem.GetType() == systemType) {
							yield return invokedSystem;
						}
					}
				}
			}

			lock (invocationList) {
				// Performance could've been better.
				var sortedArray = DependencyUtils.DependencySort(invocationList, GetDependencies, throwOnRecursion: true).ToArray();

				invocationList.Clear();

				foreach (var entry in sortedArray) {
					invocationList.AddLast(entry);
				}
			}
		}
	}
}
