using System;
using System.Threading;

namespace Dissonance.Engine.IO;

internal readonly struct ContinuationScheduler
{
	public readonly Asset Asset;

	internal ContinuationScheduler(Asset asset)
	{
		Asset = asset;
	}

	public void OnCompleted(Action continuation)
	{
		// Make the action only runnable once
		continuation = MakeOnlyRunnableOnce(continuation);

		Assets.AssetTransferQueue.Enqueue(continuation);

		Asset.Continuation = continuation;
	}

	public static Action MakeOnlyRunnableOnce(Action action)
		=> () => Interlocked.Exchange(ref action, null)?.Invoke();
}
