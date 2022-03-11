using Silk.NET.OpenAL;

namespace Dissonance.Engine.Audio
{
	public static class OpenALApi
	{
		public static AL OpenAL { get; private set; }
		public static ALContext OpenALContext { get; private set; }

		internal static void Initialize()
		{
			const bool SoftwareAL = true;

			OpenAL = AL.GetApi(soft: SoftwareAL);
			OpenALContext = ALContext.GetApi(soft: SoftwareAL);
		}
	}
}
