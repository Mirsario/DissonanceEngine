namespace SmartMeshLibrary
{
	public delegate TVector2 NewVector2<out TVector2>(float x,float y);
	public delegate TVector3 NewVector3<out TVector3>(float x,float y,float z);
	public delegate TVector4 NewVector4<out TVector4>(float x,float y,float z,float w);
	public delegate (float x,float y) Vector2ToXY<in TVector2>(TVector2 vector2);
	public delegate (float x,float y,float z) Vector3ToXYZ<in TVector3>(TVector3 vector3);
	public delegate (float x,float y,float z,float w) Vector4ToXYZW<in TVector4>(TVector4 vector4);
}
