using System;

namespace Dissonance.Engine.Graphics
{
	//Rename to PositionBuffer?
	public class VertexBuffer : CustomVertexBuffer<Vector3>
	{
		public Bounds CalculateBounds()
		{
			if(data == null) {
				throw new InvalidOperationException($"{nameof(CalculateBounds)}() expects data array to not be null.");
			}

			Vector3 min = default;
			Vector3 max = default;

			for(int i = 0; i < data.Length; i++) {
				var v = data[i];

				if(v.x > max.x) {
					max.x = v.x;
				} else if(v.x < min.x) {
					min.x = v.x;
				}

				if(v.y > max.y) {
					max.y = v.y;
				} else if(v.y < min.y) {
					min.y = v.y;
				}

				if(v.z > max.z) {
					max.z = v.z;
				} else if(v.z < min.z) {
					min.z = v.z;
				}
			}

			var boundsCenter = new Vector3(
				(min.x + max.x) * 0.5f,
				(min.y + max.y) * 0.5f,
				(min.z + max.z) * 0.5f
			);
			var boundsExtents = new Vector3(
				Mathf.Max(Mathf.Abs(min.x), Mathf.Abs(max.x)) - boundsCenter.x,
				Mathf.Max(Mathf.Abs(min.y), Mathf.Abs(max.y)) - boundsCenter.y,
				Mathf.Max(Mathf.Abs(min.z), Mathf.Abs(max.z)) - boundsCenter.z
			);

			return new Bounds(boundsCenter, boundsExtents);
		}
	}
}
