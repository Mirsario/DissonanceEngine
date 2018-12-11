using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BulletSharp;
using OpenTK;

namespace GameEngine
{
	public class Transform//: MotionState
	{
		internal Matrix4x4 _matrix = Matrix4x4.identity;
		public Transform parent = null;
		public GameObject gameObject;
		internal bool updatePhysics = true;

		#region Properties
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
				var m = _matrix;
				m.SetTranslation(value);
				if(parent!=null) {
					m = ToLocalSpace(m);
				}
				_matrix.SetTranslation(m.ExtractTranslation());
				updatePhysics = true;
				//Debug.Log(gameObject.name+"-Changed transform's position");
			}
		}
		public Vector3 LocalPosition {
			get => _matrix.ExtractTranslation();
			set {
				_matrix.SetTranslation(value);
				updatePhysics = true;
			}
		}
		/*public Vector3 scale {
			get {
				return worldMatrix.ExtractScale();
			}
			set {
				Matrix4x4 m = Matrix;
				m.SetScale(value);
				updatePhysics = true;
			}
		}*/
		public Vector3 LocalScale {
			get => _matrix.ExtractScale();
			set {
				_matrix.SetScale(value);
				updatePhysics = true;
			}
		}
		public Quaternion Rotation {
			get => WorldMatrix.ExtractQuaternion();
			set {
				var tempPos = _matrix.ExtractTranslation();
				var tempScale = _matrix.ExtractScale();
				
				var m = Matrix4x4.CreateRotation(value);
				_matrix = m;
				
				_matrix.SetTranslation(tempPos);
				_matrix.SetScale(tempScale);
				updatePhysics = true;
			}
		}
		public Quaternion LocalRotation {
			get => _matrix.ExtractQuaternion();
			set {
				var tempPos = LocalPosition;
				var tempScale = LocalScale;
				_matrix = Matrix4x4.CreateRotation(value);
				LocalPosition = tempPos;
				LocalScale = tempScale;
				updatePhysics = true;
			}
		}
		public Vector3 EulerRot {
			get => WorldMatrix.ExtractEuler();
			set {
				var tempPos = _matrix.ExtractTranslation();
				var tempScale = _matrix.ExtractScale();
				
				_matrix = parent==null ? Matrix4x4.CreateRotation(value) : ToLocalSpace(Matrix4x4.CreateRotation(value));
				
				_matrix.SetTranslation(tempPos);
				_matrix.SetScale(tempScale);
				updatePhysics = true;
			}
		}
		public Vector3 LocalEulerRot {
			get => _matrix.ExtractEuler();
			set {
				var tempPos = _matrix.ExtractTranslation();
				var tempScale = _matrix.ExtractScale();
				
				value.NormalizeEuler();
				_matrix = Matrix4x4.CreateRotation(value);
				
				_matrix.SetTranslation(tempPos);
				_matrix.SetScale(tempScale);
				updatePhysics = true;
			}
		}
		public Matrix4x4 Matrix {
			get => _matrix;
			set {
				_matrix = value;
				updatePhysics = true;
			}
		}
		public Matrix4x4 WorldMatrix {
			get => parent==null ? _matrix : ToWorldSpace(_matrix);
			set {
				_matrix = parent==null ? value : ToLocalSpace(value);
				updatePhysics = true;
			}
		}
		#endregion

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
				matrix = matrix*p.Matrix.Inverted;
			}
			return matrix;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToWorldSpace(Matrix4x4 matrix)
		{
			Transform p = this;
			while((p = p.parent)!=null) {
				matrix = matrix*p.Matrix;
			}
			return matrix;
		}
	}
}