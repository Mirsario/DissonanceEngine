using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Structures;
using System;

namespace Dissonance.Engine.Core
{
	/// <summary>
	/// A wrapper around the normal Transform. Provides properties for easier use in a 2D context.
	/// <para/> Note that the Y and Z coordinates are flipped here.
	/// </summary>
	public class Transform2D : Component
	{
		public Transform2D Parent {
			get => Transform.Parent.GameObject.Transform2D;
			set => Transform.Parent = value.Transform.GameObject.Transform;
		}
		public float Depth {
			get => -Transform.Position.z;
			set {
				var current = Transform.Position;

				Transform.Position = new Vector3(current.x, current.y, -value);
			}
		}
		public Vector2 Position {
			get {
				var current = Transform.Position;

				return new Vector2(current.x, -current.y);
			}
			set => Transform.Position = new Vector3(value.x, -value.y, Transform.Position.z);
		}
		public float Rotation {
			get => Transform.EulerRot.z;
			set => Transform.EulerRot = new Vector3(0f, 0f, value);
		}
		public float LocalDepth {
			get => -Transform.LocalPosition.z;
			set {
				var current = Transform.LocalPosition;

				Transform.LocalPosition = new Vector3(current.x, current.y, -value);
			}
		}
		public Vector2 LocalPosition {
			get {
				var current = Transform.LocalPosition;

				return new Vector2(current.x, -current.y);
			}
			set => Transform.LocalPosition = new Vector3(value.x, -value.y, Transform.LocalPosition.z);
		}
		public float LocalRotation {
			get => Transform.LocalEulerRot.z;
			set => Transform.LocalEulerRot = new Vector3(0f, 0f, value);
		}
	}
}
