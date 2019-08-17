using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GameEngine.Extensions.Chains
{
	public static class SphereColliderExtensions
	{
		public static T WithRadius<T>(this T obj,float radius) where T : SphereCollider
		{
			obj.Radius = radius;
			return obj;
		}
	}
}