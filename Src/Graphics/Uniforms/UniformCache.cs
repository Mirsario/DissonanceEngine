#pragma warning disable CS0649

namespace Dissonance.Engine.Graphics
{
	internal static class UniformCache<TUniform, TValue>
		where TUniform : AutomaticUniform<TValue>
		where TValue : unmanaged
	{
		public static TValue Value;
	}
}
