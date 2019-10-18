//Based on https://github.com/Auburns/FastNoise_CSharp/blob/master/FastNoise.cs under MIT License
//Copyright(c) 2017 Jordan Peck

using DECIMAL = System.Single;//using FN_DECIMAL = System.Double;

namespace GameEngine.Utils
{
	public partial class FastNoise
	{
		private struct Float3
		{
			public readonly DECIMAL x, y, z;

			public Float3(DECIMAL x,DECIMAL y,DECIMAL z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}
	}
}
