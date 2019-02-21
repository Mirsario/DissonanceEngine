using System.IO;
using System.Collections.Generic;
using GameEngine;

namespace AbyssCrusaders.UI.Menu
{
	public class WorldSelect : MenuState
	{
		public WorldInfo[] worlds;
		
		public override void OnActivated()
		{
			RefreshWorlds();
		}
		public override void OnGUI()
		{
			const float ButtonWidth = 512f;
			const float ButtonHeight = 64f;
			const float ButtonWidthHalf = ButtonWidth*0.5f;
			const float ButtonHeightHalf = ButtonHeight*0.5f;

			Vector2 screenHalf = Screen.Center;

			int y = 0;
			bool doRefresh = false;
			for(int i = 0;i<worlds.Length;i++) {
				var info = worlds[i];
				GUI.Button(new RectFloat(screenHalf.x-ButtonWidthHalf,256+(y*ButtonHeight),ButtonWidth-ButtonHeight,ButtonHeight),info.displayName,!info.isCorrupt);

				if(GUI.Button(new RectFloat(screenHalf.x+ButtonWidthHalf-ButtonHeight,256+(y*ButtonHeight),ButtonHeight,ButtonHeight),"X")) {
					File.Delete(info.localPath);
					doRefresh = true;
				}
				y++;
			}

			if(GUI.Button(new RectFloat(screenHalf.x-ButtonWidthHalf,256+(y*ButtonHeight)+ButtonHeightHalf,ButtonWidthHalf,ButtonHeight),"Back")) {
				controller.SetState<MainMenuState>();
			}
			if(GUI.Button(new RectFloat(screenHalf.x,256+(y*ButtonHeight)+ButtonHeightHalf,ButtonWidthHalf,ButtonHeight),"New World")) {
				controller.SetState<WorldCreate>();
			}

			if(doRefresh) {
				RefreshWorlds();
			}
		}

		public void RefreshWorlds()
		{
			var saveFiles = Directory.GetFiles(Main.savePath,"*.save");
			var worldsList = new List<WorldInfo>();
			for(int i = 0;i<saveFiles.Length;i++) {
				string file = saveFiles[i];
				try {
					var reader = new BinaryReader(File.OpenRead(file));
					if(!World.ReadInfoHeader(reader,out WorldInfo worldInfo)) {
						string fileName = Path.GetFileName(file);
						worldInfo = new WorldInfo {
							displayName = "Corrupt World - "+fileName,
							isCorrupt = true
						};
					}
					worldInfo.localPath = file;
					worldsList.Add(worldInfo);
				}
				catch {}
			}
			worlds = worldsList.ToArray();
		}
	}
}
