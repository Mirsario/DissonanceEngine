using System;
using System.Collections.Generic;
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
		public Transform Root {
			get {
				if(parent==null) {
					return this;
				}
				var parents = GetParents();
				return parents[parents.Length-1];
			}
		}
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
				m = ToLocalSpace(m);
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
			get => LocalRotation;
			set {
				LocalRotation = value;
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
				
				var m = Matrix4x4.CreateRotation(value);
				_matrix = ToLocalSpace(m);
				
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
				Matrix = ToLocalSpace(value);
				updatePhysics = true;
			}
		}
		#endregion

		public Transform(GameObject gameObject = null)
		{
			this.gameObject = gameObject;
		}
		
		//public override void GetWorldTransform(out BulletSharp.Matrix matrix) => matrix = parent==null ? _matrix : ToWorldSpace(_matrix);
		//public override void SetWorldTransform(ref BulletSharp.Matrix matrix) => _matrix = parent==null ? matrix : (BulletSharp.Math.Matrix)ToLocalSpace(matrix);

		public Transform[] GetParents()
		{
			if(parent==null) {
				return new Transform[0];
			}
			var transforms = new List<Transform>();
			GetParentsLoop(parent,ref transforms);
			return transforms.ToArray();
		}
		internal void GetParentsLoop(Transform transform,ref List<Transform> transforms)
		{
			transforms.Add(transform);
			if(transform.parent!=null) {
				GetParentsLoop(transform.parent,ref transforms);
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToLocalSpace(Matrix4x4 matrix)
		{
			var parents = GetParents();
			for(int i=0;i<parents.Length;i++) {
				matrix = matrix*parents[i].Matrix.Inverted;
			}
			return matrix;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4x4 ToWorldSpace(Matrix4x4 matrix)
		{
			var parents = GetParents();
			for(int i=0;i<parents.Length;i++) {
				matrix = matrix*parents[i].Matrix;
			}
			return matrix;
		}
	}
}