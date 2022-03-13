using Silk.NET.OpenAL;

namespace Dissonance.Engine.Audio
{
	public static class OpenALApi
	{
		public static AL OpenAL { get; private set; }
		public static ALContext OpenALContext { get; private set; }

		internal static void InitOpenAL(bool softwareAL)
		{
			OpenAL = AL.GetApi(soft: softwareAL);
			OpenALContext = ALContext.GetApi(soft: softwareAL);
		}
	}
}
