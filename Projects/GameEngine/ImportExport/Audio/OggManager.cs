using System.IO;
using NVorbis;

namespace GameEngine
{
	public class OggManager : AssetManager<AudioClip>
	{
		public override string[] Extensions => new [] { ".ogg" };
		
		public override AudioClip Import(Stream stream,string fileName)
		{
			using (var r = new VorbisReader(stream,true)) {
				long bufferSize = r.TotalSamples*r.Channels;
				var data32 = new float[bufferSize];
				int readSamples = r.ReadSamples(data32,0,(int)bufferSize);
				var data16 = new short[bufferSize];

				CastBuffer(data32,data16,bufferSize);
				
				var clip = new AudioClip();
				clip.SetData(data16,r.Channels,sizeof(short),r.SampleRate*r.Channels);
				return clip;
			}
		}

        private static void CastBuffer(float[] inBuffer,short[] outBuffer,long length)
        {
            for(long i=0;i<length;i++)  {
                int temp = (int)(32767f*inBuffer[i]);
                if(temp>short.MaxValue) {
					temp = short.MaxValue;
                }else if(temp<short.MinValue) {
					temp = short.MinValue;
				}
                outBuffer[i] = (short)temp;
            }
		}
	}
}
