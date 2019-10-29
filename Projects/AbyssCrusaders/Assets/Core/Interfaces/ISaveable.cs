using System.IO;

namespace AbyssCrusaders.Core
{
	interface ISaveable
	{
		void Save(BinaryWriter writer);
		void Load(BinaryReader reader);
	}
}
