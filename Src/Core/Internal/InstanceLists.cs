using System.Collections.Generic;

namespace Dissonance.Engine
{
	internal class InstanceLists<T>
	{
		public List<T> all;
		public List<T> enabled;
		public List<T> disabled;
		public IReadOnlyList<T> allReadOnly;
		public IReadOnlyList<T> enabledReadOnly;
		public IReadOnlyList<T> disabledReadOnly;

		public InstanceLists()
		{
			allReadOnly = (all = new List<T>()).AsReadOnly();
			enabledReadOnly = (enabled = new List<T>()).AsReadOnly();
			disabledReadOnly = (disabled = new List<T>()).AsReadOnly();
		}
	}
}
