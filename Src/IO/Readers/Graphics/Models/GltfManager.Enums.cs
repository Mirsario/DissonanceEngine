namespace Dissonance.Engine.IO
{
	partial class GltfReader
	{
		public enum ComponentType
		{
			SByte = 5120,
			Byte = 5121,
			Short = 5122,
			UShort = 5123,
			UInt = 5125,
			Float = 5126
		}

		public enum InterpolationAlgorithm
		{
			Linear,
			Step,
			CubicSpline
		}

		public enum BufferTarget
		{
			/// <summary> Vertex attributes. </summary>
			ArrayBuffer = 34962,
			/// <summary> Vertex array indices. </summary>
			ElementArrayBuffer = 34963,
		}

		public enum CameraType
		{
			Perspective,
			Ortographic
		}
	}
}
