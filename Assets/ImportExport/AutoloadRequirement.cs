using System;

namespace GameEngine
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AutoloadRequirement : Attribute
	{
		public Type[] requirements;

		public AutoloadRequirement(params Type[] types)
		{
			if(types==null || types.Length==0) {
				throw new ArgumentException("'types' array cannot be null or empty");
			}
			requirements = types;
		}
	}
}