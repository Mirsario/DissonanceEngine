using System;
using GameEngine.Graphics;

namespace GameEngine
{
	public class Camera : Component
	{
		public RectFloat view = new RectFloat(0f,0f,1f,1f);
		public RectInt ViewPixel
		{
			get => new RectInt(
				(int)(view.x*Screen.Width),
				(int)(view.y*Screen.Height),
				(int)(view.width*Screen.Width),
				(int)(view.height*Screen.Height)
			);
			set {
				view.x = value.x/Screen.Width;
				view.y = value.y/Screen.Height;
				view.width = value.width/Screen.Width;
				view.height = value.height/Screen.Height;
			}
		}
		internal Matrix4x4 matrix_view;
		internal Matrix4x4 matrix_proj;
		internal Matrix4x4 matrix_viewInverse;
		internal Matrix4x4 matrix_projInverse;

		public bool orthographic;
		public float orthographicSize = 16f;
		public float nearClip = 0.01f;
		public float farClip = 16000f;	
		public float fov = 90f;
		public float[,] frustum = new float[6,4];

		public Action<Camera> OnRenderStart;
		public Action<Camera> OnRenderEnd;

		protected override void OnEnable() => Rendering.cameraList.Add(this);
		protected override void OnDisable() => Rendering.cameraList.Remove(this);
		protected override void OnDispose() => Rendering.cameraList.Remove(this);
		public override void FixedUpdate()
		{
			
		}
		public void CalculateFrustum(Matrix4x4 clip)
		{
			//Right
			frustum[0,0] = clip[ 3]-clip[ 0]; 
			frustum[0,1] = clip[ 7]-clip[ 4];
			frustum[0,2] = clip[11]-clip[ 8];
			frustum[0,3] = clip[15]-clip[12];
			NormalizePlane(0);
			//Left
			frustum[1,0] = clip[ 3]+clip[ 0]; 
			frustum[1,1] = clip[ 7]+clip[ 4];
			frustum[1,2] = clip[11]+clip[ 8];
			frustum[1,3] = clip[15]+clip[12];
			NormalizePlane(1);
			//Bottom
			frustum[2,0] = clip[ 3]+clip[ 1]; 
			frustum[2,1] = clip[ 7]+clip[ 5];
			frustum[2,2] = clip[11]+clip[ 9];
			frustum[2,3] = clip[15]+clip[13];
			NormalizePlane(2);
			//Top
			frustum[3,0] = clip[ 3]-clip[ 1];
			frustum[3,1] = clip[ 7]-clip[ 5];
			frustum[3,2] = clip[11]-clip[ 9];
			frustum[3,3] = clip[15]-clip[13];
			NormalizePlane(3);
			//Back
			frustum[4,0] = clip[ 3]-clip[ 2]; 
			frustum[4,1] = clip[ 7]-clip[ 6];
			frustum[4,2] = clip[11]-clip[10];
			frustum[4,3] = clip[15]-clip[14];
			NormalizePlane(4);
			//Front
			frustum[5,0] = clip[ 3]+clip[ 2]; 
			frustum[5,1] = clip[ 7]+clip[ 6];
			frustum[5,2] = clip[11]+clip[10];
			frustum[5,3] = clip[15]+clip[14];
			NormalizePlane(5);
		}
		private void NormalizePlane(int side)
		{
			float magnitude = Mathf.Sqrt(frustum[side,0]*frustum[side,0]+frustum[side,1]*frustum[side,1]+frustum[side,2]*frustum[side,2]);
			frustum[side,0] /= magnitude;
			frustum[side,1] /= magnitude;
			frustum[side,2] /= magnitude;
			frustum[side,3] /= magnitude;
		}
		public bool PointInFrustum(Vector3 point)
		{
			for(int i=0;i<6;i++) {
				if(frustum[i,0]*point.x+frustum[i,1]*point.y+frustum[i,2]*point.z+frustum[i,3]<=0) {
					return false;
				}
			}
			return true;
		}
		public bool BoxInFrustum(Vector3 point,Vector3 extents)
		{
			float x1 = point.x-extents.x;
			float x2 = point.x+extents.x;
			float y1 = point.y-extents.y;
			float y2 = point.y+extents.y;
			float z1 = point.z-extents.z;
			float z2 = point.z+extents.z;
			for(int i=0;i<6;i++) {
				if(frustum[i,0]*x1+frustum[i,1]*y1+frustum[i,2]*z1+frustum[i,3]>0) {
					continue;
				}
				if(frustum[i,0]*x2+frustum[i,1]*y1+frustum[i,2]*z1+frustum[i,3]>0) {
					continue;
				}
				if(frustum[i,0]*x1+frustum[i,1]*y2+frustum[i,2]*z1+frustum[i,3]>0) {
					continue;
				}
				if(frustum[i,0]*x2+frustum[i,1]*y2+frustum[i,2]*z1+frustum[i,3]>0) {
					continue;
				}
				if(frustum[i,0]*x1+frustum[i,1]*y1+frustum[i,2]*z2+frustum[i,3]>0) {
					continue;
				}
				if(frustum[i,0]*x2+frustum[i,1]*y1+frustum[i,2]*z2+frustum[i,3]>0) {
					continue;
				}
				if(frustum[i,0]*x1+frustum[i,1]*y2+frustum[i,2]*z2+frustum[i,3]>0) {
					continue;
				}
				if(frustum[i,0]*x2+frustum[i,1]*y2+frustum[i,2]*z2+frustum[i,3]>0) {
					continue;
				}
				return false;
			}
			return true;
		}
	}
}