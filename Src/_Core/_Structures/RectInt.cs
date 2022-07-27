namespace Dissonance.Engine;

public struct RectInt
{
	public int X;
	public int Y;
	public int Width;
	public int Height;

	public int Right {
		get => X + Width;
		set => X = value - Width;
	}
	public int Bottom {
		get => Y + Height;
		set => Y = value - Height;
	}
	public Vector2Int Position {
		get => new(X, Y);
		set {
			X = value.X;
			Y = value.Y;
		}
	}
	public Vector2Int Size {
		get => new(Width, Height);
		set {
			Width = value.X;
			Height = value.Y;
		}
	}

	public RectInt(Vector2Int position, Vector2Int size) : this(position.X, position.Y, size.X, size.Y) { }

	public RectInt(Vector2Int position, int width, int height) : this(position.X, position.Y, width, height) { }

	public RectInt(int x, int y, Vector2Int size) : this(x, y, size.X, size.Y) { }

	public RectInt(int x, int y, int width, int height)
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

	public static RectInt FromPoints(int x1, int y1, int x2, int y2)
	{
		RectInt rect;

		rect.X = x1;
		rect.Y = y1;
		rect.Width = x2 - x1;
		rect.Height = y2 - y1;

		return rect;
	}

	public static implicit operator RectInt(RectFloat rectF)
		=> new((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
}
