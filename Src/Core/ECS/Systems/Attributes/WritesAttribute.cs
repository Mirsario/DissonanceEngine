namespace Dissonance.Engine
{
	public sealed class WritesAttribute<T> : SystemTypeDataAttribute where T : struct
	{
		public override void ModifySystemTypeData(SystemTypeData systemTypeData)
		{
			systemTypeData.WriteTypes.Add(typeof(T));
		}
	}
}
