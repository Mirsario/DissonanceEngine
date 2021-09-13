namespace Dissonance.Engine
{
	public struct RectFloat
	{
		public static readonly RectFloat Default = new(0f, 0f, 1f, 1f);
		public static readonly RectFloat Empty = new(0f, 0f, 0f, 0f);

		public float x;
		public float y;
		public float width;
		public float height;

		public float Right {
			get => x + width;
			set => x = value - width;
		}
		public float Bottom {
			get => y + height;
			set => y = value - height;
		}
		public Vector2 Position {
			get => new(x, y);
			set {
				x = value.x;
				y = value.y;
			}
		}
		public Vector2 Size {
			get => new(width, height);
			set {
				width = value.x;
				height = value.y;
			}
		}
		public Vector4 Points {
			get => new(x, y, x + width, y + height);
			set {
				x = value.x;
				y = value.y;
				width = value.z - x;
				height = value.w - y;
			}
		}

		public RectFloat(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public bool Contains(Vector2 point, bool inclusive = false)
		{
			if (inclusive) {
				return point.x > x && point.x < x + width && point.y > y && point.y < y + height;
			}

			return point.x >= x && point.x <= x + width && point.y >= y && point.y <= y + height;
		}

		public static RectFloat FromPoints(float x1, float y1, float x2, float y2)
		{
			RectFloat rect;

			rect.x = x1;
			rect.y = y1;
			rect.width = x2 - x1;
			rect.height = y2 - y1;

			return rect;
		}

		public static implicit operator RectFloat(RectInt rectI)
			=> new(rectI.x, rectI.y, rectI.width, rectI.height);
	}
}
