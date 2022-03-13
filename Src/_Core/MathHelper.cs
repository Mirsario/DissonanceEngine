using System;

namespace Dissonance.Engine
{
	public static partial class MathHelper
	{
		// Angle conversions
		public const float Deg2Rad = PI / 180f;
		public const float Rad2Deg = 180f / PI;
		// PI
		public const float PI = MathF.PI;
		public const float TwoPI = MathF.PI * 2f;
		public const float FourPI = MathF.PI * 4f;
		public const float HalfPI = MathF.PI / 2f;
		public const float QuarterPI = MathF.PI / 4f;
		// Etc
		public const float Epsilon = 1.401298E-45f;
	}
}
