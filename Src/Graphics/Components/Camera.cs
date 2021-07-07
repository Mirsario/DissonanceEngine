namespace Dissonance.Engine.Graphics
{
	public struct Camera
	{
		public RectFloat View { get; set; }
		public float Fov { get; set; }
		public float NearClip { get; set; }
		public float FarClip { get; set; }
		public bool Orthographic { get; set; }
		public float OrthographicSize { get; set; }

		public float[,] Frustum { get; internal set; }
		public Matrix4x4 ViewMatrix { get; internal set; }
		public Matrix4x4 ProjectionMatrix { get; internal set; }
		public Matrix4x4 InverseViewMatrix { get; internal set; }
		public Matrix4x4 InverseProjectionMatrix { get; internal set; }

		public RectInt ViewPixel {
			get => new RectInt(
				(int)(View.x * Screen.Width),
				(int)(View.y * Screen.Height),
				(int)(View.width * Screen.Width),
				(int)(View.height * Screen.Height)
			);
			set => View = new RectFloat(
				value.x / Screen.Width,
				value.y / Screen.Height,
				value.width / Screen.Width,
				value.height / Screen.Height
			);
		}

		public Camera(bool orthographic = false, float fov = 90f, float nearClip = 0.01f, float farClip = 2000f, RectFloat? view = null, float ortographicSize = 16f)
		{
			Fov = fov;
			Orthographic = orthographic;
			NearClip = nearClip;
			FarClip = farClip;
			View = view ?? RectFloat.Default;
			OrthographicSize = ortographicSize;
			Frustum = new float[6, 4];

			ViewMatrix = default;
			ProjectionMatrix = default;
			InverseViewMatrix = default;
			InverseProjectionMatrix = default;
		}

		public void CalculateFrustum(Matrix4x4 clip)
		{
			Frustum ??= new float[6, 4];

			//Right
			Frustum[0, 0] = clip[3] - clip[0];
			Frustum[0, 1] = clip[7] - clip[4];
			Frustum[0, 2] = clip[11] - clip[8];
			Frustum[0, 3] = clip[15] - clip[12];

			NormalizePlane(0);

			//Left
			Frustum[1, 0] = clip[3] + clip[0];
			Frustum[1, 1] = clip[7] + clip[4];
			Frustum[1, 2] = clip[11] + clip[8];
			Frustum[1, 3] = clip[15] + clip[12];

			NormalizePlane(1);

			//Bottom
			Frustum[2, 0] = clip[3] + clip[1];
			Frustum[2, 1] = clip[7] + clip[5];
			Frustum[2, 2] = clip[11] + clip[9];
			Frustum[2, 3] = clip[15] + clip[13];

			NormalizePlane(2);

			//Top
			Frustum[3, 0] = clip[3] - clip[1];
			Frustum[3, 1] = clip[7] - clip[5];
			Frustum[3, 2] = clip[11] - clip[9];
			Frustum[3, 3] = clip[15] - clip[13];

			NormalizePlane(3);

			//Back
			Frustum[4, 0] = clip[3] - clip[2];
			Frustum[4, 1] = clip[7] - clip[6];
			Frustum[4, 2] = clip[11] - clip[10];
			Frustum[4, 3] = clip[15] - clip[14];

			NormalizePlane(4);

			//Front
			Frustum[5, 0] = clip[3] + clip[2];
			Frustum[5, 1] = clip[7] + clip[6];
			Frustum[5, 2] = clip[11] + clip[10];
			Frustum[5, 3] = clip[15] + clip[14];

			NormalizePlane(5);
		}

		public bool PointInFrustum(Vector3 point)
		{
			for(int i = 0; i < 6; i++) {
				if(Frustum[i, 0] * point.x + Frustum[i, 1] * point.y + Frustum[i, 2] * point.z + Frustum[i, 3] <= 0) {
					return false;
				}
			}

			return true;
		}

		public bool BoxInFrustum(Bounds box)
		{
			float x1 = box.min.x;
			float x2 = box.max.x;
			float y1 = box.min.y;
			float y2 = box.max.y;
			float z1 = box.min.z;
			float z2 = box.max.z;

			for(int i = 0; i < 6; i++) {
				float f0 = Frustum[i, 0];
				float f1 = Frustum[i, 1];
				float f2 = Frustum[i, 2];
				float f3 = Frustum[i, 3];

				float f0x1 = f0 * x1;
				float f0x2 = f0 * x2;
				float f1y1 = f1 * y1;
				float f1y2 = f1 * y2;
				float f2z1 = f2 * z1;
				float f2z2 = f2 * z2;

				if(f0x1 + f1y1 + f2z1 + f3 > 0
				|| f0x2 + f1y1 + f2z1 + f3 > 0
				|| f0x1 + f1y2 + f2z1 + f3 > 0
				|| f0x2 + f1y2 + f2z1 + f3 > 0
				|| f0x1 + f1y1 + f2z2 + f3 > 0
				|| f0x2 + f1y1 + f2z2 + f3 > 0
				|| f0x1 + f1y2 + f2z2 + f3 > 0
				|| f0x2 + f1y2 + f2z2 + f3 > 0) {
					continue;
				}

				return false;
			}

			return true;
		}

		private void NormalizePlane(int side)
		{
			float magnitude = Mathf.Sqrt(
				Frustum[side, 0] * Frustum[side, 0] +
				Frustum[side, 1] * Frustum[side, 1] +
				Frustum[side, 2] * Frustum[side, 2]
			);

			Frustum[side, 0] /= magnitude;
			Frustum[side, 1] /= magnitude;
			Frustum[side, 2] /= magnitude;
			Frustum[side, 3] /= magnitude;
		}
	}
}
