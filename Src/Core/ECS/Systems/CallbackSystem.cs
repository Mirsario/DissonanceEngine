using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public abstract class CallbackSystem : GameSystem
	{
		private readonly List<GameSystem> invocationList;
		private readonly IReadOnlyList<GameSystem> invocationListReadOnly;

		private bool needsSorting;

		public IReadOnlyList<GameSystem> InvocationList {
			get {
				SortSystemsIfNeeded();

				return invocationListReadOnly;
			}
		}

		public CallbackSystem()
		{
			invocationListReadOnly = (invocationList = new()).AsReadOnly();
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
			invocationList.Add(system);

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

			// Performance could've been better.
			var sortedArray = DependencyUtils.DependencySort(invocationList, GetDependencies, throwOnRecursion: true).ToArray();

			invocationList.Clear();
			invocationList.AddRange(sortedArray);
		}
	}
}
