using System;

namespace Dissonance.Engine.Graphics
{
	public class GraphicsException : Exception
	{
		public GraphicsException(string message = null) : base(message) { }
	}
}
