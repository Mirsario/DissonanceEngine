using GameEngine;

namespace AbyssCrusaders
{
	public class GameObject2D : GameObject
	{
		private float depth;
		private float rotation;
		private Vector2 position;

		public float Depth {
			get => depth;
			set {
				depth = value;
				Transform.LocalPosition = new Vector3(position.x,-position.y,depth);
			}
		}
		public float Rotation {
			get => rotation;
			set => Transform.EulerRot = new Vector3(0f,0f,rotation = value);
		}
		public Vector2 Position {
			get => position;
			set {
				position = value;
				Transform.LocalPosition = new Vector3(position.x,-position.y,depth);
			}
		}

		public static T Instantiate<T>(string name = default,Vector2 position = default,float depth = 0f,float rotation = default,Vector2? scale = null,bool init = true) where T : GameObject2D
		{
			T result = (T)Instantiate(
				typeof(T),name,
				new Vector3(position.x,-position.y,depth),
				rotation==0f ? default : Quaternion.FromEuler(0f,0f,rotation),
				scale.HasValue ? new Vector3(scale.Value.x,scale.Value.y,1f) : (Vector3?)null,
				init
			);
			result.position = position;
			result.depth = depth;
			return result;
		}
	}
}
