namespace Dissonance.Engine
{
	public sealed class ReadsAttribute<T> : SystemTypeDataAttribute where T : struct
	{
		public override void ModifySystemTypeData(SystemTypeData systemTypeData)
		{
			systemTypeData.ReadTypes.Add(typeof(T));
		}
	}
}
