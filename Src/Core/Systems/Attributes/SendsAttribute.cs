namespace Dissonance.Engine
{
	public sealed class SendsAttribute<T> : SystemTypeDataAttribute where T : struct
	{
		public override void ModifySystemTypeData(SystemTypeData systemTypeData)
		{
			systemTypeData.SendTypes.Add(typeof(T));
		}
	}
}
