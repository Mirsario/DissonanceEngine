namespace AbyssCrusaders.DataStructures
{
	public class WallDrawInfo
	{
		public ushort srcX;
		public ushort srcY;
		public ushort srcWidth;
		public ushort srcHeight;
		public short destOffsetX;
		public short destOffsetY;

		public WallDrawInfo(ushort srcX,ushort srcY,ushort srcWidth,ushort srcHeight,short destOffsetX,short destOffsetY)
		{
			this.srcX = srcX;
			this.srcY = srcY;
			this.srcWidth = srcWidth;
			this.srcHeight = srcHeight;
			this.destOffsetX = destOffsetX;
			this.destOffsetY = destOffsetY;
		}
	}
}
