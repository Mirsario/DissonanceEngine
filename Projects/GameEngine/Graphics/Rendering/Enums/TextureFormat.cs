namespace GameEngine.Graphics
{
	public enum TextureFormat
	{
		#region A
		A8,     //8
		A16,    //16
		#endregion
		#region R
		R8,     //8
		R8i,
		R8ui,
		R16,    //16
		R16f,
		R16i,
		R16ui,
		R32f,   //32
		R32i,
		R32ui,
		#endregion
		#region RG
		RG8,    //8
		RG8i,
		RG8ui,
		RG16,   //16
		RG16f,
		RG16i,
		RG16ui,
		RG32f,  //32
		RG32i,
		RG32ui,
		#endregion
		#region RGB
		RGB8,   //8
		RGB8i,
		RGB8ui,
		RGB16,  //16
		RGB16f,
		RGB16i,
		RGB16ui,
		RGB32f, //32
		RGB32i,
		RGB32ui,
		#endregion
		#region RGBA
		RGBA8,      //8
		RGBA8i,
		RGBA8ui,
		RGBA16,     //16
		RGBA16f,
		RGBA16i,
		RGBA16ui,
		RGBA32f,    //32
		RGBA32i,
		RGBA32ui,
		#endregion
		#region Depth/Stencil
		Depth16,
		Depth32,
		Depth32f,
		Depth24Stencil8,
		Depth32fStencil8,
		#endregion
	}
}