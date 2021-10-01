namespace Dissonance.Engine.Graphics
{
	internal struct AutomaticUniformUsageInfo
	{
		public int Index;
		public int? Location;

		public AutomaticUniformUsageInfo(int index, int? location)
		{
			Index = index;
			Location = location;
		}
	}
}
