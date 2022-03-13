using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dissonance.Engine
{
	public struct Transform
	{
		[Flags]
		public enum UpdateFlags
		{
			None = 0,
			Position = 1,
			Rotation = 2,
			Scale = 4,
			All = Position | Rotation | Scale
		}

		public readonly IReadOnlyList<Transform> Children;

		private readonly List<Transform> ChildrenInternal;

		private Matrix4x4 matrix = Matrix4x4.Identity;

		public Entity? Parent { get; set; } = null;

		public Transform Root => Parent.HasValue ? EnumerateParents().Last() : this;

		public ref Transform ParentTransform => ref Parent.Value.Get<Transform>();

		public Vector3 Forward {
			get {
				var m = WorldMatrix;

				m.ClearScale();

				return new Vector3(m.m20, m.m21, m.m22).Normalized;
			}
		}

		public Vector3 Right {
			get {
				var m = WorldMatrix;

				m.ClearScale();

				return new Vector3(m.m00, m.m01, m.m02).Normalized;
			}
		}

		public Vector3 Up {
			get {
				var m = WorldMatrix;

				m.ClearScale();

				return new Vector3(m.m10, m.m11, m.m12).Normalized;
			}
		}

		public Vector3 Position {
			get => WorldMatrix.ExtractTranslation();
			set {
				var m = matrix;

				m.SetTranslation(value);

				if (Parent != null) {
					m = ToLocalSpace(m);
				}

				matrix.SetTranslation(m.ExtractTranslation());
				OnModified?.Invoke(this, UpdateFlags.Position);
			}
		}

		public Vector2 Position2D {
			get => Position.XY;
			set {
				var position3D = Position;

				position3D.XY = value;

				Position = position3D;
			}
		}

		public float Depth2D {
			get => Position.Z;
			set {
				var position3D = Position;

				position3D.Z = value;

				Position = position3D;
			}
		}

		public Vector3 LocalPosition {
			get => matrix.ExtractTranslation();
			set {
				matrix.SetTranslation(value);
				OnModified?.Invoke(this, UpdateFlags.Position);
			}
		}

		public Vector2 LocalPosition2D {
			get => LocalPosition.XY;
			set {
				var position3D = LocalPosition;

				position3D.XY = value;

				LocalPosition = position3D;
			}
		}

		public float LocalDepth2D {
			get => LocalPosition.Z;
			set {
				var position3D = LocalPosition;

				position3D.Z = value;

				LocalPosition = position3D;
			}
		}

		public Vector3 LocalScale {
			get => matrix.ExtractScale();
			set {
				matrix.SetScale(value);
				OnModified?.Invoke(this, UpdateFlags.Scale);
			}
		}

		public Vector2 LocalScale2D {
			get => matrix.ExtractScale2D();
			set {
				matrix.SetScale2D(value);
				OnModified?.Invoke(this, UpdateFlags.Scale);
			}
		}

		public Quaternion Rotation {
			get => WorldMatrix.ExtractQuaternion();
			set {
				var tempPos = matrix.ExtractTranslation();
				var tempScale = matrix.ExtractScale();

				var m = Matrix4x4.CreateRotation(value);

				matrix = m;

				matrix.SetTranslation(tempPos);
				matrix.SetScale(tempScale);

				OnModified?.Invoke(this, UpdateFlags.Rotation);
			}
		}

		public Quaternion LocalRotation {
			get => matrix.ExtractQuaternion();
			set {
				var tempPos = LocalPosition;
				var tempScale = LocalScale;

				matrix = Matrix4x4.CreateRotation(value);
				LocalPosition = tempPos;
				LocalScale = tempScale;

				OnModified?.Invoke(this, UpdateFlags.Rotation);
			}
		}

		public Vector3 EulerRot {
			get => WorldMatrix.ExtractEuler();
			set {
				var tempPos = matrix.ExtractTranslation();
				var tempScale = matrix.ExtractScale();

				matrix = Parent == null ? Matrix4x4.CreateRotation(value) : ToLocalSpace(Matrix4x4.CreateRotation(value));

				matrix.SetTranslation(tempPos);
				matrix.SetScale(tempScale);

				OnModified?.Invoke(this, UpdateFlags.Rotation);
			}
		}

		public float EulerRot2D {
			get => WorldMatrix.ExtractEulerZ();
			set {
				var tempPos = matrix.ExtractTranslation();
				var tempScale = matrix.ExtractScale();

				matrix = Parent == null ? Matrix4x4.CreateRotationZ(value) : ToLocalSpace(Matrix4x4.CreateRotationZ(value));

				matrix.SetTranslation(tempPos);
				matrix.SetScale(tempScale);

				OnModified?.Invoke(this, UpdateFlags.Rotation);
			}
		}

		public Vector3 LocalEulerRot {
			get => matrix.ExtractEuler();
			set {
				var tempPos = matrix.ExtractTranslation();
				var tempScale = matrix.ExtractScale();

				value.NormalizeEuler();

				matrix = Matrix4x4.CreateRotation(value);

				matrix.SetTranslation(tempPos);
				matrix.SetScale(tempScale);

				OnModified?.Invoke(this, UpdateFlags.Rotation);
			}
		}

		public Matrix4x4 Matrix {
			get => matrix;
			set {
				matrix = value;

				OnModified?.Invoke(this, UpdateFlags.All);
			}
		}

		public Matrix4x4 WorldMatrix {
			get => Parent == null ? matrix : ToWorldSpace(matrix);
			set {
				matrix = Parent == null ? value : ToLocalSpace(value);

				OnModified?.Invoke(this, UpdateFlags.All);
			}
		}

		public event Action<Transform, UpdateFlags> OnModified = null;

		public Transform()
		{
			Children = (ChildrenInternal = new()).AsReadOnly();
		}

		public Transform(Vector3? position = null, Vector3? eulerRotation = null, Vector3? localScale = null) : this()
		{
			matrix = Matrix4x4.Identity;
			Children = (ChildrenInternal = new List<Transform>()).AsReadOnly();
			OnModified = null;

			if (position.HasValue && position.Value != default) {
				Position = position.Value;
			}

			if (eulerRotation.HasValue && eulerRotation.Value != default) {
				EulerRot = eulerRotation.Value;
			}

			if (localScale.HasValue && localScale.Value != Vector3.One) {
				LocalScale = localScale.Value;
			}
		}

		public Transform(Vector2? position = null, float? eulerRotation = null, Vector2? localScale = null) : this()
		{
			matrix = Matrix4x4.Identity;
			Children = (ChildrenInternal = new List<Transform>()).AsReadOnly();
			OnModified = null;

			if (position.HasValue && position.Value != default) {
				Position2D = position.Value;
			}

			if (eulerRotation.HasValue && eulerRotation.Value != 0f) {
				EulerRot2D = eulerRotation.Value;
			}

			if (localScale.HasValue && localScale.Value != Vector2.One) {
				LocalScale2D = localScale.Value;
			}
		}

		public IEnumerable<Transform> EnumerateParents()
		{
			Transform transform = this;

			while (transform.Parent.HasValue) {
				transform = transform.ParentTransform;

				yield return transform;
			}
		}

		public IEnumerable<Transform> EnumerateChildren()
		{
			for (int i = 0; i < ChildrenInternal.Count; i++) {
				var child = ChildrenInternal[i];

				yield return child;

				foreach (var transform in child.EnumerateChildren()) {
					yield return transform;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToLocalSpace(Matrix4x4 matrix)
		{
			Transform transform = this;

			while (transform.Parent.HasValue) {
				transform = transform.ParentTransform;
				matrix *= transform.Matrix.Inverted;
			}

			return matrix;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToWorldSpace(Matrix4x4 matrix)
		{
			Transform transform = this;

			while (transform.Parent.HasValue) {
				transform = transform.ParentTransform;
				matrix *= transform.Matrix;
			}

			return matrix;
		}
	}
}
