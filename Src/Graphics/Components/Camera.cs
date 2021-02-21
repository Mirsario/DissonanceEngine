using System;

namespace Dissonance.Engine.Graphics
{
	public class Camera : Component
	{
		public RectFloat view = new RectFloat(0f, 0f, 1f, 1f);

		public bool orthographic;
		public float orthographicSize = 16f;
		public float nearClip = 0.01f;
		public float farClip = 2000f;
		public float fov = 90f;
		public float[,] frustum = new float[6, 4];

		public Action<Camera> OnRenderStart;
		public Action<Camera> OnRenderEnd;

		internal Matrix4x4 matrix_view;
		internal Matrix4x4 matrix_proj;
		internal Matrix4x4 matrix_viewInverse;
		internal Matrix4x4 matrix_projInverse;

		public RectInt ViewPixel {
			get => new RectInt(
				(int)(view.x * Screen.Width),
				(int)(view.y * Screen.Height),
				(int)(view.width * Screen.Width),
				(int)(view.height * Screen.Height)
			);
			set {
				view.x = value.x / Screen.Width;
				view.y = value.y / Screen.Height;
				view.width = value.width / Screen.Width;
				view.height = value.height / Screen.Height;
			}
		}

		public void CalculateFrustum(Matrix4x4 clip)
		{
			//Right
			frustum[0, 0] = clip[3] - clip[0];
			frustum[0, 1] = clip[7] - clip[4];
			frustum[0, 2] = clip[11] - clip[8];
			frustum[0, 3] = clip[15] - clip[12];

			NormalizePlane(0);

			//Left
			frustum[1, 0] = clip[3] + clip[0];
			frustum[1, 1] = clip[7] + clip[4];
			frustum[1, 2] = clip[11] + clip[8];
			frustum[1, 3] = clip[15] + clip[12];

			NormalizePlane(1);

			//Bottom
			frustum[2, 0] = clip[3] + clip[1];
			frustum[2, 1] = clip[7] + clip[5];
			frustum[2, 2] = clip[11] + clip[9];
			frustum[2, 3] = clip[15] + clip[13];

			NormalizePlane(2);

			//Top
			frustum[3, 0] = clip[3] - clip[1];
			frustum[3, 1] = clip[7] - clip[5];
			frustum[3, 2] = clip[11] - clip[9];
			frustum[3, 3] = clip[15] - clip[13];

			NormalizePlane(3);

			//Back
			frustum[4, 0] = clip[3] - clip[2];
			frustum[4, 1] = clip[7] - clip[6];
			frustum[4, 2] = clip[11] - clip[10];
			frustum[4, 3] = clip[15] - clip[14];

			NormalizePlane(4);

			//Front
			frustum[5, 0] = clip[3] + clip[2];
			frustum[5, 1] = clip[7] + clip[6];
			frustum[5, 2] = clip[11] + clip[10];
			frustum[5, 3] = clip[15] + clip[14];

			NormalizePlane(5);
		}
		public bool PointInFrustum(Vector3 point)
		{
			for(int i = 0; i < 6; i++) {
				if(frustum[i, 0] * point.x + frustum[i, 1] * point.y + frustum[i, 2] * point.z + frustum[i, 3] <= 0) {
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
				float f0 = frustum[i, 0];
				float f1 = frustum[i, 1];
				float f2 = frustum[i, 2];
				float f3 = frustum[i, 3];

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
				frustum[side, 0] * frustum[side, 0] +
				frustum[side, 1] * frustum[side, 1] +
				frustum[side, 2] * frustum[side, 2]
			);

			frustum[side, 0] /= magnitude;
			frustum[side, 1] /= magnitude;
			frustum[side, 2] /= magnitude;
			frustum[side, 3] /= magnitude;
		}
	}
}
