//Based on https://github.com/Auburns/FastNoise_CSharp/blob/master/FastNoise.cs under MIT License
//Copyright(c) 2017 Jordan Peck

using DECIMAL = System.Single;

namespace GameEngine.Utils
{
	public partial class FastNoise
	{
		private struct Float2
		{
			public readonly DECIMAL x,y;

			public Float2(DECIMAL x,DECIMAL y)
			{
				this.x = x;
				this.y = y;
			}
		}
	}
}
