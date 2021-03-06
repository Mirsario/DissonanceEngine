using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dissonance.Engine
{
	public class Transform : Component
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
		private Transform parent;

		public Transform Root => Parent == null ? this : EnumerateParents().Last();
		public Transform Parent {
			get => parent;
			set {
				if(parent == value) {
					return;
				}

				if(parent != null) {
					parent.ChildrenInternal.Remove(this);
				}

				if(value != null) {
					value.ChildrenInternal.Add(this);
				}

				parent = value;
				GameObject.EnabledInHierarchy = parent?.Enabled ?? true;
			}
		}
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

				if(Parent != null) {
					m = ToLocalSpace(m);
				}

				matrix.SetTranslation(m.ExtractTranslation());
				OnModified?.Invoke(this, UpdateFlags.Position);
			}
		}
		public Vector3 LocalPosition {
			get => matrix.ExtractTranslation();
			set {
				matrix.SetTranslation(value);
				OnModified?.Invoke(this, UpdateFlags.Position);
			}
		}
		public Vector3 LocalScale {
			get => matrix.ExtractScale();
			set {
				matrix.SetScale(value);
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

		public event Action<Transform, UpdateFlags> OnModified;

		public Transform() : base()
		{
			Children = (ChildrenInternal = new List<Transform>()).AsReadOnly();
		}

		public override Component Clone(GameObject newGameObject)
		{
			var clone = (Transform)base.Clone(newGameObject);

			clone.parent = null;

			return clone;
		}

		public IEnumerable<Transform> EnumerateParents()
		{
			var transform = this;

			while((transform = transform.Parent) != null) {
				yield return transform;
			}
		}
		public IEnumerable<Transform> EnumerateChildren()
		{
			for(int i = 0; i < ChildrenInternal.Count; i++) {
				var child = ChildrenInternal[i];

				yield return child;

				foreach(var transform in child.EnumerateChildren()) {
					yield return transform;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToLocalSpace(Matrix4x4 matrix)
		{
			Transform p = this;

			while((p = p.Parent) != null) {
				matrix *= p.Matrix.Inverted;
			}

			return matrix;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToWorldSpace(Matrix4x4 matrix)
		{
			Transform p = this;

			while((p = p.Parent) != null) {
				matrix *= p.Matrix;
			}

			return matrix;
		}
	}
}
