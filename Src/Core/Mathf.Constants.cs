using System.Runtime.CompilerServices;

namespace Dissonance.Engine
{
	//Very aggressive class, be careful
	partial class Mathf
	{
		public const float NegativeInfinity = float.NegativeInfinity;
		public const float Infinity = float.PositiveInfinity;
		public const float Epsilon = 1.401298E-45f;
		public const float FourPI = 12.56637061435917295385f;
		public const float TwoPI = 6.283185307179586476925f;
		public const float PI = 3.14159265358979323846264f;
		public const float HalfPI = 1.570796326794896619231f;
		public const float QuarterPI = 0.7853981633974483096157f;
		public const float Deg2Rad = PI / 180f;
		public const float Rad2Deg = 180f / PI;

		private const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining;
	}
}