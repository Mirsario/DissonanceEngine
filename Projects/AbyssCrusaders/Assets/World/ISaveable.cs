using System.IO;

namespace AbyssCrusaders
{
	interface ISaveable
	{
		void Save(BinaryWriter writer);
		void Load(BinaryReader reader);
	}
}
