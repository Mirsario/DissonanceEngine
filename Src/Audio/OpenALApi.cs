using Silk.NET.OpenAL;

namespace Dissonance.Engine.Audio;

[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
public sealed class OpenALApi : EngineModule
{
	public static AL OpenAL { get; private set; }
	public static ALContext OpenALContext { get; private set; }

	public readonly bool SoftwareAL;

	private OpenALApi() : this(true) { }

	public OpenALApi(bool softwareAL)
	{
		SoftwareAL = softwareAL;
	}

	protected override void Init()
	{
		OpenAL = AL.GetApi(soft: SoftwareAL);
		OpenALContext = ALContext.GetApi(soft: SoftwareAL);
	}

	protected override void OnDispose()
	{
		if (OpenAL != null) {
			OpenAL.Dispose();

			OpenAL = null;
		}

		if (OpenALContext != null) {
			OpenALContext.Dispose();

			OpenALContext = null;
		}
	}
}
