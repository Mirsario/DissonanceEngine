using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GameEngine
{
	public class Transform
	{
		public Transform parent = null;
		public GameObject gameObject;

		internal Matrix4x4 matrix = Matrix4x4.Identity;
		internal bool updatePhysicsPosition = true;
		internal bool updatePhysicsRotation = true;
		internal bool updatePhysicsScale = true;

		//Properties
		public Transform Root => parent==null ? this : GetParents().Last();
		public Vector3 Forward {
			get {
				var m = WorldMatrix;
				m.ClearScale();
				return new Vector3(m.m20,m.m21,m.m22).Normalized;
			}
		}
		public Vector3 Right {
			get {
				var m = WorldMatrix;
				m.ClearScale();
				return new Vector3(m.m00,m.m01,m.m02).Normalized;
			}
		}
		public Vector3 Up {
			get {
				var m = WorldMatrix;
				m.ClearScale();
				return new Vector3(m.m10,m.m11,m.m12).Normalized;
			}
		}
		public Vector3 Position {
			get => WorldMatrix.ExtractTranslation();
			set {
				var m = matrix;
				m.SetTranslation(value);
				if(parent!=null) {
					m = ToLocalSpace(m);
				}
				matrix.SetTranslation(m.ExtractTranslation());
				updatePhysicsPosition = true;
			}
		}
		public Vector3 LocalPosition {
			get => matrix.ExtractTranslation();
			set {
				matrix.SetTranslation(value);
				updatePhysicsPosition = true;
			}
		}
		public Vector3 LocalScale {
			get => matrix.ExtractScale();
			set {
				matrix.SetScale(value);
				updatePhysicsScale = true;
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
				updatePhysicsRotation = true;
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
				updatePhysicsRotation = true;
			}
		}
		public Vector3 EulerRot {
			get => WorldMatrix.ExtractEuler();
			set {
				var tempPos = matrix.ExtractTranslation();
				var tempScale = matrix.ExtractScale();
				
				matrix = parent==null ? Matrix4x4.CreateRotation(value) : ToLocalSpace(Matrix4x4.CreateRotation(value));
				
				matrix.SetTranslation(tempPos);
				matrix.SetScale(tempScale);
				updatePhysicsRotation = true;
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
				updatePhysicsRotation = true;
			}
		}
		public Matrix4x4 Matrix {
			get => matrix;
			set {
				matrix = value;
				updatePhysicsPosition = true;
				updatePhysicsRotation = true;
				updatePhysicsScale = true;
			}
		}
		public Matrix4x4 WorldMatrix {
			get => parent==null ? matrix : ToWorldSpace(matrix);
			set {
				matrix = parent==null ? value : ToLocalSpace(value);
				updatePhysicsPosition = true;
				updatePhysicsRotation = true;
				updatePhysicsScale = true;
			}
		}

		public Transform(GameObject gameObject = null)
		{
			this.gameObject = gameObject;
		}

		public IEnumerable<Transform> GetParents() => parent==null ? null : GetParentsIterator(this);
		private static IEnumerable<Transform> GetParentsIterator(Transform transform)
		{
			while(true) {
				transform = transform.parent;
				if(transform==null) {
					break;
				}
				yield return transform;
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToLocalSpace(Matrix4x4 matrix)
		{
			Transform p = this;
			while((p = p.parent)!=null) {
				matrix *= p.Matrix.Inverted;
			}
			return matrix;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToWorldSpace(Matrix4x4 matrix)
		{
			Transform p = this;
			while((p = p.parent)!=null) {
				matrix *= p.Matrix;
			}
			return matrix;
		}
	}
}