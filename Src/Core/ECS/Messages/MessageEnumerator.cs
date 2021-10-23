using System.Collections.Generic;

namespace Dissonance.Engine
{
	public ref struct MessageEnumerator<T>
	{
		private readonly List<T> list;

		private int i;
		private int count;
		private T current;

		public T Current => current;

		public MessageEnumerator(List<T> list)
		{
			this.list = list;

			i = -1;
			count = list.Count;
			current = default;
		}

		public bool MoveNext()
		{
			if (++i < count || i < (count = list?.Count ?? 0)) {
				current = list[i];

				return true;
			}

			current = default;

			return false;
		}

		public MessageEnumerator<T> GetEnumerator() => this;
	}
}
