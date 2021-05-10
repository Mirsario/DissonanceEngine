using BulletSharp;
using BulletSharp.Math;

namespace Dissonance.Engine.Physics
{
	/*internal class MotionStateInternal : MotionState
	{
		private readonly RigidbodyInternal RBInternal;
		private readonly Transform Transform;

		private Transform.UpdateFlags updateFlags;
		private bool ignoreNextUpdate;

		public MotionStateInternal(RigidbodyInternal rbInternal)
		{
			RBInternal = rbInternal;
			Transform = rbInternal.gameObject.Transform;

			Transform.OnModified += OnTransformModified;
		}

		//Called by bullet whenever a rigidbody is created, and every frame for kinematic rigidbodies.
		public override void GetWorldTransform(out Matrix matrix)
		{
			matrix = Transform.Parent == null ? Transform.Matrix : Transform.ToWorldSpace(Transform.Matrix);
		}
		//Called by bullet for every frame or update of an active rigidbody.
		public override void SetWorldTransform(ref Matrix matrix)
		{
			ignoreNextUpdate = true;

			Transform.Matrix = Transform.Parent == null ? matrix : (Matrix)Transform.ToLocalSpace(matrix);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			Transform.OnModified -= OnTransformModified;
		}

		//Updates bullet's rigidbody's transformation with ours if it has been updated.
		public void Update()
		{
			var btRigidbody = RBInternal.btRigidbody;

			if(updateFlags == Transform.UpdateFlags.None) {
				return;
			}

			if(updateFlags == Transform.UpdateFlags.All) {
				btRigidbody.WorldTransform = Transform.WorldMatrix;
			} else {
				Matrix4x4 matrix = btRigidbody.WorldTransform;

				if(updateFlags.HasFlag(Transform.UpdateFlags.Position)) {
					matrix.SetTranslation(Transform.Position);
				}

				switch(updateFlags) {
					case Transform.UpdateFlags.Scale | Transform.UpdateFlags.Rotation:
						matrix.SetRotationAndScale(Transform.Rotation, Transform.LocalScale);
						break;
					case Transform.UpdateFlags.Scale:
						matrix.SetScale(Transform.LocalScale);
						break;
					case Transform.UpdateFlags.Rotation:
						matrix.SetRotation(Transform.Rotation);
						break;
				}

				btRigidbody.WorldTransform = matrix;
			}

			updateFlags = Transform.UpdateFlags.None;

			//Activate the rigidbody if it's sleeping
			if(!btRigidbody.IsActive) {
				btRigidbody.Activate();
			}
		}

		//This callback is used to keep track of transform updates.
		private void OnTransformModified(Transform transform, Transform.UpdateFlags updateFlags)
		{
			if(ignoreNextUpdate) {
				ignoreNextUpdate = false;
				return;
			}

			this.updateFlags |= updateFlags;
		}
	}*/
}
