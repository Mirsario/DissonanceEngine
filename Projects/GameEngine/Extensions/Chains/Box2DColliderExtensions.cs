using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GameEngine.Extensions.Chains
{
	public static class Box2DColliderExtensions
	{
		public static T WithSize<T>(this T obj,Vector2 size) where T : Box2DCollider
		{
			obj.Size = size;
			return obj;
		}
	}
}