using System;

namespace Dissonance.Engine.Graphics
{
	// Rename to PositionBuffer?
	public class VertexBuffer : CustomVertexBuffer<Vector3>
	{
		public Bounds CalculateBounds()
		{
			if (data == null) {
				throw new InvalidOperationException($"{nameof(CalculateBounds)}() expects data array to not be null.");
			}

			Vector3 min = default;
			Vector3 max = default;

			for (int i = 0; i < data.Length; i++) {
				var v = data[i];

				if (v.X > max.X) {
					max.X = v.X;
				} else if (v.X < min.X) {
					min.X = v.X;
				}

				if (v.Y > max.Y) {
					max.Y = v.Y;
				} else if (v.Y < min.Y) {
					min.Y = v.Y;
				}

				if (v.Z > max.Z) {
					max.Z = v.Z;
				} else if (v.Z < min.Z) {
					min.Z = v.Z;
				}
			}

			return new Bounds(min, max);
		}
	}
}
