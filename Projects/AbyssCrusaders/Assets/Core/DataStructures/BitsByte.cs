namespace AbyssCrusaders.DataStructures
{
	public struct BitsByte
	{
		public byte value;
		
		public bool this[int id] {
			get => (value & 1 << id) != 0;
			set {
				if(value) {
					this.value |= (byte)(1 << id);
					return;
				}
				this.value &= (byte)~(byte)(1 << id);
			}
		}
		
		public BitsByte(byte byteVal)
		{
			value = byteVal;
		}
		public BitsByte(bool bit0 = false,bool bit1 = false,bool bit2 = false,bool bit3 = false,bool bit4 = false,bool bit5 = false,bool bit6 = false,bool bit7 = false)
		{
			value = 0;

			if(bit0) { this[0] = bit0; }
			if(bit1) { this[1] = bit1; }
			if(bit2) { this[2] = bit2; }
			if(bit3) { this[3] = bit3; }
			if(bit4) { this[4] = bit4; }
			if(bit5) { this[5] = bit5; }
			if(bit6) { this[6] = bit6; }
			if(bit7) { this[7] = bit7; }
		}

		public static implicit operator byte(BitsByte bitsByte) => bitsByte.value;
		public static implicit operator BitsByte(byte byteVal) => new BitsByte(byteVal);
	}
}
