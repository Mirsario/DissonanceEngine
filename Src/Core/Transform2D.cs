using Dissonance.Engine.Structures;
using System;

namespace Dissonance.Engine.Core
{
	//TODO: Avoid gimbal-locking on rotation somehow.
	/// <summary>
	/// A wrapper around the normal Transform. Provides properties for easier use in a 2D context.
	/// <para/> Note that the Y and Z coordinates are flipped here.
	/// </summary>
	public class Transform2D
	{
		public Transform BaseTransform { get; }

		public Transform2D Parent {
			get => BaseTransform.Parent.GameObject.Transform2D;
			set => BaseTransform.Parent = value.BaseTransform.GameObject.Transform;
		}
		public float Depth {
			get => -BaseTransform.Position.z;
			set {
				var current = BaseTransform.Position;

				BaseTransform.Position = new Vector3(current.x, current.y, -value);
			}
		}
		public Vector2 Position {
			get {
				var current = BaseTransform.Position;

				return new Vector2(current.x, -current.y);
			}
			set => BaseTransform.Position = new Vector3(value.x, -value.y, BaseTransform.Position.z);
		}
		public float Rotation {
			get => BaseTransform.EulerRot.z;
			set => BaseTransform.EulerRot = new Vector3(0f, 0f, value);
		}
		public float LocalDepth {
			get => -BaseTransform.LocalPosition.z;
			set {
				var current = BaseTransform.LocalPosition;

				BaseTransform.LocalPosition = new Vector3(current.x, current.y, -value);
			}
		}
		public Vector2 LocalPosition {
			get {
				var current = BaseTransform.LocalPosition;

				return new Vector2(current.x, -current.y);
			}
			set => BaseTransform.LocalPosition = new Vector3(value.x, -value.y, BaseTransform.LocalPosition.z);
		}
		public float LocalRotation {
			get => BaseTransform.LocalEulerRot.z;
			set => BaseTransform.LocalEulerRot = new Vector3(0f, 0f, value);
		}

		public Transform2D(Transform baseTransform)
		{
			BaseTransform = baseTransform ?? throw new ArgumentNullException(nameof(baseTransform));
		}
	}
}
