using System.Reflection;

namespace Dissonance.Engine
{
	public abstract class SingletonInputTrigger : InputTrigger
	{
		public static class Info<T> where T : SingletonInputTrigger
		{
			public static int id;
		}

		protected SingletonInputTrigger() : base() {}

		protected abstract void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue);

		internal override void Init(int id,string name,InputBinding[] bindings,float minValue,float maxValue)
		{
			Init(out name,out bindings,out float newMinValue,out float newMaxValue);

			base.Init(id,name,bindings,newMinValue,newMaxValue);
		}

		internal static void StaticInit()
		{
			foreach(var type in ReflectionCache.allTypes) {
				if(type.IsAbstract || !typeof(SingletonInputTrigger).IsAssignableFrom(type)) {
					continue;
				}

				var trigger = Input.RegisterTrigger(type,type.Name,null);

				typeof(Info<>).MakeGenericType(type)
					.GetField(nameof(Info<SingletonInputTrigger>.id),BindingFlags.Static|BindingFlags.Public)
					.SetValue(null,trigger.Id);
			}
		}
	}
}