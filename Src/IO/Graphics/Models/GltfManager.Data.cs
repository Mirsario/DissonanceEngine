using System.Collections.Generic;
using Dissonance.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO.Graphics.Models
{
	partial class GltfManager
	{
		public class GltfData
		{
			public class AccessorData
			{
				public ComponentType componentType;
			}

			public AccessorData[] accessorData;
		}
	}
}
