namespace Dissonance.Engine
{
	public sealed class ReceivesAttribute<T> : SystemTypeDataAttribute where T : struct
	{
		public override void ModifySystemTypeData(SystemTypeData systemTypeData)
		{
			systemTypeData.ReceiveTypes.Add(typeof(T));
		}
	}
}
