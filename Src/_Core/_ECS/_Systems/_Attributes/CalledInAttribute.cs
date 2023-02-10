using System;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Method /*| AttributeTargets.Delegate*/, AllowMultiple = true, Inherited = true)]
public sealed class CalledInAttribute<T> : Attribute, ISystemAttribute//, ICallbackAttribute
	where T : Delegate
{
	void ISystemAttribute.ConfigureSystem(SystemHandle system)
	{
		Callbacks.AddSystemsToCallback(Callbacks.Get<T>(), stackalloc SystemHandle[] { system });
	}

	/*
	void ICallbackAttribute.ConfigureCallback(Callback callback)
	{
		if (Callbacks.TryGetCallback<T>(out var callback)) {
			callback.Systems.Add(system);
		}
	}
	*/
}
