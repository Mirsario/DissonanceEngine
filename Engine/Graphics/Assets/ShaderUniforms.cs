namespace GameEngine
{
	internal class DefaultShaderUniforms
	{
		//Awful looking internal code, all for the sake of performance
		public static string[] names = {
			//Matrices
			"world",
			"worldInverse",
			"worldView",
			"worldViewInverse",
			"worldViewProj",
			"worldViewProjInverse",
			"view",
			"viewInverse",
			"proj",
			"projInverse",
			//Camera
			"nearClip",
			"farClip",
			"screenWidth",
			"screenHeight",
			"cameraPosition",
			//Other
			"color"
		};
		//Matrices
		public const int World = 0;
		public const int WorldInverse = 1;
		public const int WorldView = 2;
		public const int WorldViewInverse = 3;
		public const int WorldViewProj = 4;
		public const int WorldViewProjInverse = 5;
		public const int View = 6;
		public const int ViewInverse = 7;
		public const int Proj = 8;
		public const int ProjInverse = 9;
		//Camera
		public const int NearClip = 10;
		public const int FarClip = 11;
		public const int ScreenWidth = 12;
		public const int ScreenHeight = 13;
		public const int CameraPosition = 14;
		//Other
		public const int Color = 15;
		//Count, important
		public const int Count = 16;
	}
}