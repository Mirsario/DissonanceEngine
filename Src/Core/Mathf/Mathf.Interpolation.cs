using System.Runtime.CompilerServices;

namespace Dissonance.Engine
{
	public static partial class Mathf
	{
		[MethodImpl(Inline)]
		public static float Lerp(float a,float b,float time) => a+(b-a)*Clamp01(time);

		[MethodImpl(Inline)]
		public static double Lerp(double a,double b,double time) => a+(b-a)*Clamp01(time);

		[MethodImpl(Inline)]
		public static float LerpAngle(float a,float b,float t)
		{
			float num = Repeat(b-a,360f);

			return a+(num>180f ? num-360f : num)*(t<0f ? 0f : (t>1f ? 1f : t));
		}

		[MethodImpl(Inline)]
		public static double LerpAngle(double a,double b,double t)
		{
			double num = Repeat(b-a,360d);

			return a+(num>180d ? num-360d : num)*(t<0d ? 0d : (t>1d ? 1d : t));
		}

		public static float BiLerp(float valueTopLeft,float valueTopRight,float valueBottomLeft,float valueBottomRight,Vector2 topLeft,Vector2 bottomRight,Vector2 point)
		{
			float x2x1 = bottomRight.x-topLeft.x;
			float y2y1 = bottomRight.y-topLeft.y;
			float x2x = bottomRight.x-point.x;
			float y2y = bottomRight.y-point.y;
			float yy1 = point.y-topLeft.y;
			float xx1 = point.x-topLeft.x;

			return 1f/(x2x1*y2y1)*(valueBottomLeft*x2x*y2y+valueBottomRight*xx1*y2y+valueTopLeft*x2x*yy1+valueTopRight*xx1*yy1);
		}
	}
}