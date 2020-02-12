using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using Ionic.Zip;
using System.Text;
using System.Runtime.Serialization;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine
{
	//TODO: Still refactoring...
	//TODO: AssetManager getter methods aren't finished.
	public static partial class Resources
	{
		public const string BuiltInAssetsFolder = "BuiltInAssets/";

		private static readonly string[] DefaultSearchPattern = { "*" };

		internal static Dictionary<string,List<AssetManager>> assetManagers;
		internal static IEnumerable<IEnumerable<AssetManager>> autoloadOrder;
		internal static Dictionary<string,string> nameToPath;
		internal static Dictionary<Type,Dictionary<string,object>> cacheByPath;
		internal static Dictionary<Type,Dictionary<string,object>> cacheByName;
		internal static Dictionary<string,byte[]> builtInAssets;
		internal static bool importingBuiltInAssets;

		public static void AddToCache<T>(string filePath,T content)
		{
			var type = typeof(T);

			if(!cacheByPath.TryGetValue(typeof(T),out var dict)) {
				cacheByPath[type] = dict = new Dictionary<string,object>(InternalUtils.strComparerInvariantIgnoreCase);
			}

			dict[filePath] = content;
		}
		public static void SetDefaultManager(string ext,Type type)
		{
			ext = ext.ToLower();

			if(!assetManagers.TryGetValue(ext,out var list)) {
				throw new NotImplementedException($"Could not find any ''{ext}'' asset managers.");
			}

			for(int i = 0;i<list.Count;i++) {
				var manager = list[i];
				var generics = manager.GetType().GetGenericArguments();

				if(generics!=null && generics.Length>0 && generics[0]==type) {
					list.RemoveAt(i);
					list.Insert(0,manager);
					return;
				}
			}

			throw new NotImplementedException($"Could not find any ''{ext}'' asset managers which would return a {type.Name}.");
		}
		public static bool TryGetFileAssetManager(string file,out AssetManager outManager)
		{
			if(!assetManagers.TryGetValue(Path.GetExtension(file).ToLower(),out var list)) {
				outManager = null;

				return false;
			}

			outManager = list[0];

			return outManager.Autoload(file);
		}
		public static T1 GetAssetManager<T1,T2>()
			where T1 : AssetManager<T2>
			where T2 : Asset
		{
			var type = typeof(T1);

			//TODO: This loop is idiotic. Cache data for this exact method.
			foreach(var pair in assetManagers) {
				foreach(var assetManager in pair.Value) {
					if(assetManager.GetType()==type) {
						return (T1)assetManager;
					}
				}
			}

			return null;
		}
		public static void RegisterFormats<T>(AssetManager<T> importer,string[] formats,bool allowOverwriting = false) where T : class
			=> RegisterFormats(typeof(T),importer,formats,allowOverwriting);

		internal static void Init()
		{
			assetManagers = new Dictionary<string,List<AssetManager>>();
			nameToPath = new Dictionary<string,string>(InternalUtils.strComparerInvariantIgnoreCase);
			cacheByPath = new Dictionary<Type,Dictionary<string,object>>();
			cacheByName = new Dictionary<Type,Dictionary<string,object>>();
			builtInAssets = new Dictionary<string,byte[]>(InternalUtils.strComparerInvariantIgnoreCase);

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			LoadManagers();
			AutoloadResources();
		}
		internal static string[] GetFilesRecursive(string path,string[] searchPattern = null,string[] ignoredPaths = null)
		{
			if(searchPattern==null) {
				searchPattern = DefaultSearchPattern;
			}

			var files = new List<string>();

			void GetFilesRecursion(string currentPath)
			{
				//Iterate files
				foreach(string pattern in searchPattern) {
					foreach(string current in Directory.GetFiles(currentPath,pattern)) {
						if(ignoredPaths?.Any(s => current.StartsWith(s))==true) {
							continue;
						}

						files.Add(current);
					}
				}

				//Iterate directories
				foreach(string folderPath in Directory.GetDirectories(currentPath)) {
					if(ignoredPaths?.Any(s => folderPath.StartsWith(s))==true) {
						continue;
					}

					GetFilesRecursion(folderPath);
				}
			}

			GetFilesRecursion(path);

			return files.ToArray();
		}

		private static void LoadManagers()
		{
			var newOrder = new List<List<AssetManager>> { new List<AssetManager>() };

			foreach(var type in ReflectionCache.allTypes) {
				if(type.IsAbstract || !typeof(AssetManager).IsAssignableFrom(type)) {
					continue;
				}

				var manager = (AssetManager)FormatterServices.GetUninitializedObject(type);

				var generics = type?.BaseType.GetGenericArguments();
				var returnType = generics?.Length==1 ? generics[0] : null;

				if(returnType==null) {
					continue;
				}

				manager.Init();

				var realReturnType = returnType.IsArray ? returnType.GetElementType() : returnType;

				RegisterFormats(realReturnType,manager,manager.Extensions); //Allow overwriting engine's types by games'

				newOrder[0].Add(manager);
			}

			//Sort everything based on dependencies

			for(int i = 0;i<newOrder.Count;i++) {
				var list = newOrder[i];
				var move = new bool[list.Count];
				bool moveAny = false;

				for(int j = 0;j<list.Count;j++) {
					var aManager = list[j];
					var aDependsOn = aManager.GetType().GetCustomAttributes<AutoloadRequirement>()?.Select(att => att.requirements).FirstOrDefault()?.ToArray(); //Only for error checks

					var aType = aManager.GetType();

					for(int k = 0;k<list.Count;k++) {
						var bManager = list[k];
						var bDependsOn = bManager.GetType().GetCustomAttributes<AutoloadRequirement>()?.Select(att => att.requirements).FirstOrDefault()?.ToArray(); //For sorting

						if(bDependsOn==null || !bDependsOn.Any(t => t==aType)) {
							continue;
						}

						if(aManager==bManager) {
							throw new ArgumentException($"AssetManager '{aType.Name}' can't depend on itself.");
						}

						var bType = bManager.GetType();

						if(aDependsOn!=null && aDependsOn.Any(t => t==bType)) {
							throw new ArgumentException($"AssetManagers '{aType.Name}' and '{bType.Name}' depend on each other in Autoload. This is unacceptable.");
						}

						move[k] = true;
						moveAny = true;
					}
				}

				if(moveAny) {
					if(i+1==newOrder.Count) {
						newOrder.Add(new List<AssetManager>());
					}

					var nextList = newOrder[i+1];
					int movedNum = 0;

					for(int j = 0;j<move.Length;j++) {
						if(move[j]) {
							nextList.Add(list[j-movedNum]);
							list.RemoveAt(j-movedNum);

							movedNum++;
						}
					}
				}
			}

			autoloadOrder = newOrder;
		}
		private static void AutoloadResources()
		{
			AutoloadBuiltInResources();
			AutoloadGameResources();
		}
		private static void AutoloadBuiltInResources()
		{
			importingBuiltInAssets = true;

			//Unzip
			using(var stream = new MemoryStream(Properties.Resources.DefaultResources)) {
				using var zipFile = ZipFile.Read(stream);

				foreach(var zipEntry in zipFile) {
					string filePath = zipEntry.FileName;

					using var entryStream = new MemoryStream();

					zipEntry.Extract(entryStream);

					builtInAssets[BuiltInAssetsFolder+filePath] = entryStream.ToArray();
				}
			}

			//Autoload

			//Save assets which could be imported with an AssetManager into a dictionary,with AssetManagers being the keys.
			var engineAssetsByManager = new Dictionary<AssetManager,List<KeyValuePair<string,byte[]>>>();

			foreach(var pair in builtInAssets) {
				string filePath = pair.Key;
				if(!TryGetFileAssetManager(filePath,out var manager)) {
					continue;
				}

				if(engineAssetsByManager.TryGetValue(manager,out var list)) {
					list.Add(pair);
				} else {
					engineAssetsByManager.Add(manager,new List<KeyValuePair<string,byte[]>> { pair });
				}
			}

			//Enumerate through these assets in loading order
			foreach(var managers in autoloadOrder) {
				foreach(var manager in managers) {
					if(!engineAssetsByManager.TryGetValue(manager,out var pairList)) {
						continue;
					}

					for(int k = 0;k<pairList.Count;k++) {
						var pair = pairList[k];
						ImportBuiltInAsset(pair.Key,manager,pair.Value);
					}
				}
			}

			engineAssetsByManager.Clear();

			importingBuiltInAssets = false;
		}
		private static void AutoloadGameResources()
		{
			//Save assets which could be imported with an AssetManager into a dictionary, with AssetManagers being the keys.

			var allGameFiles = Directory.GetFiles(Game.assetsPath,"*.*",SearchOption.AllDirectories);
			var gameAssetsByManager = new Dictionary<AssetManager,List<string>>();

			for(int i = 0;i<allGameFiles.Length;i++) {
				string file = allGameFiles[i].Replace(@"\\","/").Replace(@"\","/");
				string fileName = Path.GetFileName(file);

				if(!nameToPath.ContainsKey(fileName)) {
					nameToPath.Add(fileName,file);
				} else {
					//If path is null, it means that there's multiple files with such name, so trying to access them with just their filename will throw an error.
					nameToPath[fileName] = null;
				}

				if(!TryGetFileAssetManager(file,out var manager)) {
					continue;
				}

				if(gameAssetsByManager.TryGetValue(manager,out var list)) {
					list.Add(file);
				} else {
					gameAssetsByManager.Add(manager,new List<string> { file });
				}
			}

			//Enumerate through these assets in loading order
			foreach(var managers in autoloadOrder) {
				foreach(var manager in managers) {
					if(!gameAssetsByManager.TryGetValue(manager,out var fileList)) {
						continue;
					}

					for(int k = 0;k<fileList.Count;k++) {
						try {
							//TODO: This could use some improving.

							var tType = manager.GetType().BaseType?.GetGenericArguments()[0];
							if(tType==null) {
								continue;
							}

							var method = typeof(Resources).GetMethod("Import",BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(tType);

							method.Invoke(manager,new object[] { fileList[k],true,manager,true });
						}
						catch(TargetInvocationException e) {
							throw e.InnerException;
						}
					}
				}
			}
		}
		private static void NameToPath(ref string filePath,out bool multiplePathsFound)
		{
			multiplePathsFound = false;
			
			string fileName = Path.GetFileName(filePath);

			if(nameToPath.TryGetValue(fileName,out string fullPath)) {
				if(fullPath==null) {
					multiplePathsFound = true;
				}else{
					filePath = fullPath;
				}
			}
		}
		private static void RegisterFormats(Type type,AssetManager assetManager,string[] extensions,bool allowOverwriting = false)
		{
			//TODO: Use the unused parameters.

			for(int i = 0;i<extensions.Length;i++) {
				string ext = extensions[i];

				if(assetManagers.TryGetValue(ext,out var list)) {
					list.Add(assetManager);
				} else {
					list = new List<AssetManager> { assetManager };
				}

				assetManagers[ext] = list;
			}
		}
		private static void ReadyPath(ref string path)
		{
			if(!Path.IsPathRooted(path)) {
				string lowerPath = path.ToLower();

				if(!lowerPath.StartsWith(BuiltInAssetsFolder.ToLower()) && !lowerPath.StartsWith("assets/")) {
					path = importingBuiltInAssets ? BuiltInAssetsFolder+path : "Assets/"+path;
				}
			}
		}
	}
}