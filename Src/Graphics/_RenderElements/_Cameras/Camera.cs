using System;

namespace Dissonance.Engine.Graphics;

public struct Camera
{
	public unsafe struct FrustumData
	{
		public const int Width = 6;
		public const int Height = 4;
		public const int Length = Width * Height;

		private fixed float values[Length];

		public ref float this[int i] => ref values[i];

		public ref float this[int x, int y] => ref values[y + (x * 4)];
	}

	public RectFloat View { get; set; } = RectFloat.Default;
	public LayerMask LayerMask { get; set; } = LayerMask.All;
	public float FieldOfView { get; set; } = 90f;
	public float NearClip { get; set; } = 0.01f;
	public float FarClip { get; set; } = 2000f;
	public bool Orthographic { get; set; } = false;
	public float OrthographicSize { get; set; } = 16f;
	public FrustumData Frustum { get; set; } = new();

	public Matrix4x4 ViewMatrix { get; internal set; } = default;
	public Matrix4x4 ProjectionMatrix { get; internal set; } = default;
	public Matrix4x4 InverseViewMatrix { get; internal set; } = default;
	public Matrix4x4 InverseProjectionMatrix { get; internal set; } = default;

	public RectInt ViewPixel {
		get => new(
			(int)(View.X * Screen.Width),
			(int)(View.Y * Screen.Height),
			(int)(View.Width * Screen.Width),
			(int)(View.Height * Screen.Height)
		);
		set => View = new RectFloat(
			value.X / Screen.Width,
			value.Y / Screen.Height,
			value.Width / Screen.Width,
			value.Height / Screen.Height
		);
	}

	public Camera() { }

	//TODO: Move to a system.
	public void CalculateFrustum(Matrix4x4 clip)
	{
		// Right
		Frustum[0, 0] = clip[3] - clip[0];
		Frustum[0, 1] = clip[7] - clip[4];
		Frustum[0, 2] = clip[11] - clip[8];
		Frustum[0, 3] = clip[15] - clip[12];

		NormalizePlane(0);

		// Left
		Frustum[1, 0] = clip[3] + clip[0];
		Frustum[1, 1] = clip[7] + clip[4];
		Frustum[1, 2] = clip[11] + clip[8];
		Frustum[1, 3] = clip[15] + clip[12];

		NormalizePlane(1);

		// Bottom
		Frustum[2, 0] = clip[3] + clip[1];
		Frustum[2, 1] = clip[7] + clip[5];
		Frustum[2, 2] = clip[11] + clip[9];
		Frustum[2, 3] = clip[15] + clip[13];

		NormalizePlane(2);

		// Top
		Frustum[3, 0] = clip[3] - clip[1];
		Frustum[3, 1] = clip[7] - clip[5];
		Frustum[3, 2] = clip[11] - clip[9];
		Frustum[3, 3] = clip[15] - clip[13];

		NormalizePlane(3);

		// Back
		Frustum[4, 0] = clip[3] - clip[2];
		Frustum[4, 1] = clip[7] - clip[6];
		Frustum[4, 2] = clip[11] - clip[10];
		Frustum[4, 3] = clip[15] - clip[14];

		NormalizePlane(4);

		// Front
		Frustum[5, 0] = clip[3] + clip[2];
		Frustum[5, 1] = clip[7] + clip[6];
		Frustum[5, 2] = clip[11] + clip[10];
		Frustum[5, 3] = clip[15] + clip[14];

		NormalizePlane(5);
	}

	public bool PointInFrustum(Vector3 point)
	{
		for (int i = 0; i < 6; i++) {
			if (Frustum[i, 0] * point.X + Frustum[i, 1] * point.Y + Frustum[i, 2] * point.Z + Frustum[i, 3] <= 0) {
				return false;
			}
		}

		return true;
	}

	public bool BoxInFrustum(Bounds box)
	{
		float x1 = box.Min.X;
		float x2 = box.Max.X;
		float y1 = box.Min.Y;
		float y2 = box.Max.Y;
		float z1 = box.Min.Z;
		float z2 = box.Max.Z;

		for (int i = 0; i < 6; i++) {
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

			if (f0x1 + f1y1 + f2z1 + f3 > 0
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
		float magnitude = MathF.Sqrt(
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
