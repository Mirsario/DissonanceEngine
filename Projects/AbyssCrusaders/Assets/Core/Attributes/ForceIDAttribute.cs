using System;

namespace AbyssCrusaders
{
	public class ForceIDAttribute : Attribute
	{
		public ushort id;

		public ForceIDAttribute(ushort id)
		{
			this.id = id;
		}
	}
}
