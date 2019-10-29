using GameEngine;

namespace AbyssCrusaders.Core.Test
{
	public class CursorSoundObj : GameObject2D
	{
		public override void OnInit()
		{
			base.OnInit();

			AddComponent<AudioSource>(c => {
				c.Clip = Resources.Get<AudioClip>("Audio/Test.ogg");
				c.Loop = true;
			}).Play();
		}

		public override void RenderUpdate()
		{
			Position = Main.camera.Position;

			if(Input.GetKeyDown(Keys.M)) {
				var audio = GetComponent<AudioSource>();
				if(audio.IsPlaying) {
					audio.Pause();
				} else {
					audio.Play();
				}
			}
		}
	}
}