using System.Reflection;

#pragma warning disable CS0649

namespace Dissonance.Engine.Input;

public abstract class SingletonInputTrigger : InputTrigger
{
	protected SingletonInputTrigger() : base() { }

	protected abstract void Init(out string name, out InputBinding[] bindings, out float minValue, out float maxValue);

	internal override void Init(int id, string name, InputBinding[] bindings, float minValue, float maxValue)
	{
		Init(out name, out bindings, out float newMinValue, out float newMaxValue);

		base.Init(id, name, bindings, newMinValue, newMaxValue);
	}
}
