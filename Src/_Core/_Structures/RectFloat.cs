using Dissonance.Engine.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine
{
	[JsonConverter(typeof(RectFloatJsonConverter))]
	public struct RectFloat
	{
		public static readonly RectFloat Default = new(0f, 0f, 1f, 1f);
		public static readonly RectFloat Empty = new(0f, 0f, 0f, 0f);

		public float X;
		public float Y;
		public float Width;
		public float Height;

		public float Right {
			get => X + Width;
			set => X = value - Width;
		}
		public float Bottom {
			get => Y + Height;
			set => Y = value - Height;
		}
		public Vector2 Position {
			get => new(X, Y);
			set {
				X = value.X;
				Y = value.Y;
			}
		}
		public Vector2 Size {
			get => new(Width, Height);
			set {
				Width = value.X;
				Height = value.Y;
			}
		}
		public Vector4 Points {
			get => new(X, Y, X + Width, Y + Height);
			set {
				X = value.X;
				Y = value.Y;
				Width = value.Z - X;
				Height = value.W - Y;
			}
		}

		public RectFloat(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public bool Contains(Vector2 point, bool inclusive = false)
		{
			if (inclusive) {
				return point.X > X && point.X < X + Width && point.Y > Y && point.Y < Y + Height;
			}

			return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
		}

		public static RectFloat FromPoints(float x1, float y1, float x2, float y2)
		{
			RectFloat rect;

			rect.X = x1;
			rect.Y = y1;
			rect.Width = x2 - x1;
			rect.Height = y2 - y1;

			return rect;
		}

		public static implicit operator RectFloat(RectInt rectI)
			=> new(rectI.X, rectI.Y, rectI.Width, rectI.Height);
	}
}
