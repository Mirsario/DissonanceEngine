using GameEngine;
using System;

namespace AbyssCrusaders.Core
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

		public static T Instantiate<T>(string name = default,Vector2 position = default,float? depth = null,float rotation = default,Vector2? scale = null,bool init = true) where T : GameObject2D
			=> (T)Instantiate(typeof(T),name,position,depth,rotation,scale,init);
		public static GameObject Instantiate(Type type,string name = default,Vector2 position = default,float? depth = null,float rotation = default,Vector2? scale = null,bool init = true)
		{
			if(type==null) {
				throw new ArgumentNullException(nameof(type));
			}

			if(!typeof(GameObject2D).IsAssignableFrom(type)) {
				throw new ArgumentException($"Type does not derive from '{nameof(GameObject2D)}'.",nameof(type));
			}

			Quaternion qRotation = rotation==0f ? default : Quaternion.FromEuler(0f,0f,rotation);
			Vector3 vec3Position = new Vector3(position.x,-position.y,depth ?? 0f);
			Vector3? vec3Scale = scale.HasValue ? new Vector3(scale.Value.x,scale.Value.y,1f) : (Vector3?)null;

			GameObject2D result = (GameObject2D)Instantiate(type,name,vec3Position,qRotation,vec3Scale,init);

			result.Position = position;

			if(depth.HasValue) {
				result.Depth = depth.Value;
			}

			return result;
		}
	}
}
