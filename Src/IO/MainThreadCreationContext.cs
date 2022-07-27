using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Dissonance.Engine.IO;

/// <summary> Await this to switch execution of the method to the main thread. </summary>
public readonly struct MainThreadCreationContext : INotifyCompletion
{
	internal ContinuationScheduler ContinuationScheduler { private get; init; }

	public bool IsCompleted => Thread.CurrentThread == GameEngine.MainThread;
	public MainThreadCreationContext GetAwaiter() => this;

	internal MainThreadCreationContext(ContinuationScheduler continuationScheduler)
	{
		ContinuationScheduler = continuationScheduler;
	}

	public void OnCompleted(Action continuation)
		=> ContinuationScheduler.OnCompleted(continuation);

	public void GetResult() { }
}
