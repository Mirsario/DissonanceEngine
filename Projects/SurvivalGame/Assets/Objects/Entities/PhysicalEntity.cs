using GameEngine;
using GameEngine.Physics;
using ImmersionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public abstract class PhysicalEntity : Entity, IPhysicalEntity
	{
		public Vector3 velocity;
		public Vector3 prevVelocity;
		public Renderer[] renderers;
		public Collider[] colliders;
		public Rigidbody rigidbody;

		private bool isVisible = true;
		private bool renderersSetUp;

		public bool IsAttached => Transform.parent!=null;

		Vector3 IPhysicalEntity.Velocity {
			get => velocity;
			set => velocity = value;
		}
		Vector3 IPhysicalEntity.PrevVelocity {
			get => prevVelocity;
			set => prevVelocity = value;
		}

		public virtual bool IsVisible {
			get => isVisible;
			set {
				isVisible = value;

				PrepareRenderers();

				OnUpdateIsVisible(isVisible);
			}
		}

		public virtual void SetupRenderers(ref Renderer[] renderers) {}
		public virtual void SetupPhysicsComponents(ref Collider[] colliders,ref Rigidbody rigidbody) {}

		protected virtual void OnUpdateIsVisible(bool isVisible)
		{
			if(renderers!=null) {
				for(int i = 0;i<renderers.Length;i++) {
					var renderer = renderers[i];
					if(renderer!=null) {
						renderer.Enabled = isVisible;
					}
				}
			}

			if(colliders!=null) {
				for(int i = 0;i<colliders.Length;i++) {
					var collider = colliders[i];
					if(colliders!=null) {
						collider.Enabled = isVisible;
					}
				}
			}

			if(rigidbody!=null) {
				rigidbody.Enabled = isVisible;
			}
		}
		protected virtual void OnAttachedTo(PhysicalEntity entity) {}
		protected virtual void OnDetachedFrom(PhysicalEntity entity) {}

		public override void OnInit()
		{
			base.OnInit();

			PrepareRenderers();

			SetupPhysicsComponents(ref colliders,ref rigidbody);
		}

		public void AttachTo(PhysicalEntity entity,bool hide = true)
		{
			Transform.parent = entity.Transform;
			Transform.LocalPosition = Vector3.Zero;

			if(hide) {
				IsVisible = false;
			}

			OnAttachedTo(entity);
		}
		public void Detach()
		{
			var parent = Transform.parent;
			if(parent==null) {
				return;
			}

			if(parent.gameObject is PhysicalEntity entity) {
				OnDetachedFrom(entity);
			}

			IsVisible = true;

			var position = Transform.Position;
			Transform.parent = null;
			Transform.Position = position;
		}

		private void PrepareRenderers()
		{
			if(isVisible && !renderersSetUp) {
				SetupRenderers(ref renderers);

				renderersSetUp = true;
			}
		}
	}
}
