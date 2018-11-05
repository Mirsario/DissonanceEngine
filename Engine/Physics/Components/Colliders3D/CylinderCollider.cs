using System;
using System.Collections.Generic;
using BulletSharp;

namespace GameEngine
{
	public class CylinderCollider : Collider
	{
		public Vector3 size = Vector3.one;

		protected override void OnInit()
		{
			base.OnInit();
			UpdateCollider();
		}
		internal override void UpdateCollider()
		{
			if(collShape!=null) {
				collShape.Dispose();
				collShape = null;
			}
			Debug.Log("Cylinder size: "+size);
			collShape = new CylinderShape(size*0.5f);
			/*OpenTK.Vector3 a,b;
			((CylinderShape)collShape).GetAabb(Matrix4x4.identity,out a,out b);
			Debug.Log("AABB: "+a+"-> "+b);
			Debug.Log("SIZE: "+(b-a));*/
			base.UpdateCollider();
		}
	}
}