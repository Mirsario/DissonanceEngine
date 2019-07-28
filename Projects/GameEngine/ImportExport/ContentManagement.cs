using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using GameEngine.Graphics;
using Ionic.Zip;

namespace GameEngine
{
	public static class Resources
	{
		public const string builtInAssetsFolder = "BuiltInAssets/";

		private static readonly string[] DefaultSearchPattern = { "*" };

		internal static Dictionary<string,List<AssetManager>> assetManagers;
		internal static AssetManager[][] autoloadOrder;
		internal static Dictionary<string,string> nameToPath;
		internal static Dictionary<string,object> cacheByPath;
		internal static Dictionary<Type,Dictionary<string,object>> cacheByName;
		internal static Dictionary<string,byte[]> builtInAssets;
		internal static bool importingBuiltInAssets;
		
		public static void Init()
		{
			assetManagers = new Dictionary<string,List<AssetManager>>();
			nameToPath = new Dictionary<string,string>(StringComparer.CurrentCultureIgnoreCase);
			cacheByPath = new Dictionary<string,object>(StringComparer.CurrentCultureIgnoreCase);
			cacheByName = new Dictionary<Type,Dictionary<string,object>>();
			builtInAssets = new Dictionary<string,byte[]>(StringComparer.CurrentCultureIgnoreCase);

			#region ContentManagers
			#region RegisterManagers
			//Get all assemblies which aren't referenced by engine and aren't default microsoft assemblies
			var engineAssembly = Assembly.GetExecutingAssembly();
			var references = engineAssembly.GetReferencedAssemblies();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !InternalUtils.IsMicrosoftAssembly(a) && !references.Any(r => r.Name.Equals(a.GetName().Name))).ToList();
			assemblies.Remove(engineAssembly);
			assemblies.Insert(0,engineAssembly);

			var newOrder = new List<List<AssetManager>> { new List<AssetManager>() };
			foreach(var type in ReflectionCache.engineTypes) {
				if(!type.IsAbstract && typeof(AssetManager).IsAssignableFrom(type)) {
					var manager = (AssetManager)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
					var generics = type?.BaseType.GetGenericArguments();
					var returnType = generics?.Length==1 ? generics[0] : null;
					if(returnType==null) {
						continue;
					}

					var realReturnType = returnType.IsArray ? returnType.GetElementType() : returnType;
					RegisterFormats(realReturnType,manager,manager.Extensions); //Allow overwriting engine's types by games'

					newOrder[0].Add(manager);
				}
			}
			#endregion

			#region SortLoadOrder
			//Sort everything based on dependencies
			for(int i=0;i<newOrder.Count;i++) {
				var list = newOrder[i];
				var move = new bool[list.Count];
				bool moveAny = false;
				for(int j=0;j<list.Count;j++) {
					var aManager = list[j];
					var aDependsOn = aManager.GetType().GetCustomAttributes<AutoloadRequirement>()?.Select(att => att.requirements).FirstOrDefault()?.ToArray();//Only for error checks
					/*if(aDependsOn?.Length>0) {
						Console.WriteLine(aManager.GetType().Name+" depends on type "+aDependsOn[0].Name);
					}*/
					var aType = aManager.GetType();
					for(int k=0;k<list.Count;k++) {
						var bManager = list[k];
						var bDependsOn = bManager.GetType().GetCustomAttributes<AutoloadRequirement>()?.Select(att => att.requirements).FirstOrDefault()?.ToArray();//For sorting
						/*if(bDependsOn?.Length>0==true) {
							Console.WriteLine(bManager.GetType().Name+" depends on type "+bDependsOn[0].Name);
						}*/
						var bType = bManager.GetType();
						if(bDependsOn==null) {
							continue;
						}
						if(bDependsOn.Any(t => t==aType)) {
							if(aManager==bManager) {
								throw new ArgumentException("AssetManager "+aType.Name+" can't require itself to be loaded first.");
							}
							if(aDependsOn!=null && aDependsOn.Any(t => t==bType)) {
								throw new ArgumentException("AssetManagers '"+aType.Name+"' and '"+bType.Name+"' depend on each other in Autoload. This is unacceptable.");
							}
							move[k] = true;
							moveAny = true;
						}
					}
				}
				if(moveAny) {
					if(i+1==newOrder.Count) {
						newOrder.Add(new List<AssetManager>());
					}
					var nextList = newOrder[i+1];
					int movedNum = 0;
					for(int j=0;j<move.Length;j++) {
						if(move[j]) {
							nextList.Add(list[j-movedNum]);
							list.RemoveAt(j-movedNum);
							movedNum++;
						}
					}
				}
			}

			//Convert our lists to arrays for performance?
			autoloadOrder = newOrder.Select(list => {
				var array = list.ToArray();
				list.Clear();
				return array;
			}).ToArray();
			newOrder.Clear();
			#endregion
			#endregion

			AutoloadResources();
		}
		private static void AutoloadResources()
		{
			bool GetAndCheckManager(string file,out AssetManager outManager)
			{
				if(!assetManagers.TryGetValue(Path.GetExtension(file).ToLower(),out var list)) {
					outManager = null;
					return false;
				}
				outManager = list[0];
				return outManager.Autoload(file);
			}

			AssetManager manager;
			#region BuiltInAssets
			importingBuiltInAssets = true;
			#region Unzip
			using(var stream = new MemoryStream(Properties.Resources.DefaultResources)) {
				using(var zipFile = ZipFile.Read(stream)) {
					foreach(var zipEntry in zipFile) {
						string filePath = zipEntry.FileName;
						using(var entryStream = new MemoryStream()) {
							zipEntry.Extract(entryStream);
							builtInAssets[builtInAssetsFolder+filePath] = entryStream.ToArray();
						}
					}
				}
			}
			#endregion
			#region Autoload
			//Save assets which could be imported with an AssetManager into a dictionary,with AssetManagers being the keys.
			var engineAssetsByManager = new Dictionary<AssetManager,List<KeyValuePair<string,byte[]>>>();
			foreach(var pair in builtInAssets) {
				string filePath = pair.Key;
				if(!GetAndCheckManager(filePath,out manager)) {
					continue;
				}
				if(engineAssetsByManager.TryGetValue(manager,out var list)) {
					list.Add(pair);
				}else{
					engineAssetsByManager.Add(manager,new List<KeyValuePair<string,byte[]>> { pair });
				}
			}

			//Enumerate through these assets in loading order
			for(int i=0;i<autoloadOrder.Length;i++) {
				var managers = autoloadOrder[i];

				for(int j=0;j<managers.Length;j++) {
					manager = managers[j];
					
					if(!engineAssetsByManager.TryGetValue(manager,out var pairList)) {
						continue;
					}

					for(int k=0;k<pairList.Count;k++) {
						var pair = pairList[k];
						ImportBuiltInAsset(pair.Key,manager,pair.Value);
					}
				}
			}
			engineAssetsByManager.Clear();
			#endregion
			importingBuiltInAssets = false;
			#endregion
			#region GameResources
			//Save assets which could be imported with an AssetManager into a dictionary,with AssetManagers being the keys.
			var allGameFiles = Directory.GetFiles(Game.assetsPath,"*.*",SearchOption.AllDirectories);
			var gameAssetsByManager = new Dictionary<AssetManager,List<string>>();
			for(int i=0;i<allGameFiles.Length;i++) {
				string file = allGameFiles[i].Replace(@"\\","/").Replace(@"\","/");
				string fileName = Path.GetFileName(file);
				if(!nameToPath.ContainsKey(fileName)) {
					nameToPath.Add(fileName,file);
				}else{
					nameToPath[fileName] = null;//If path is null,it means that there's multiple files with such name,and trying to access them with just their filename will throw an error.
				}
				if(!GetAndCheckManager(file,out manager)) {
					continue;
				}
				if(gameAssetsByManager.TryGetValue(manager,out var list)) {
					list.Add(file);
				}else{
					gameAssetsByManager.Add(manager,new List<string> { file });
				}
			}
			//Enumerate through these assets in loading order
			for(int i=0;i<autoloadOrder.Length;i++) {
				var managers = autoloadOrder[i];
				for(int j=0;j<managers.Length;j++) {
					manager = managers[j];
					if(!gameAssetsByManager.TryGetValue(manager,out var fileList)) {
						continue;
					}
					for(int k=0;k<fileList.Count;k++) {
						//TODO: This is badddd
						try {
							var method = typeof(Resources).GetMethod("Import",BindingFlags.Public | BindingFlags.Static);
							var tType = manager.GetType().BaseType?.GetGenericArguments()[0];
							if(tType==null) { continue; }
							method.MakeGenericMethod(tType).Invoke(manager,new object[] { fileList[k],true,manager,true });
						}
						catch(TargetInvocationException e) {
							throw e.InnerException;
						}
					}
				}
			}
			#endregion
		}

		private static void NameToPath(ref string filePath,out bool multiplePathsFound)
		{
			multiplePathsFound = false;
			//if(filePath.Any(c => c=='/' || c=='\\')) {
			string fileName = Path.GetFileName(filePath);
			if(nameToPath.TryGetValue(fileName,out string fullPath)) {
				if(fullPath==null) {
					multiplePathsFound = true;
				}else{
					filePath = fullPath;
				}
			}
			//}
		}

		#region Import
		//Simple file importing, optionally caching them for Get.
		public static T Import<T>(string filePath,bool addToCache = true,AssetManager<T> assetManager = null,bool throwOnFail = true) where T : class
		{
			NameToPath(ref filePath,out bool ntpMultiplePaths);

			return ImportInternal(filePath,addToCache,assetManager,ntpMultiplePaths);
		}
		internal static T ImportInternal<T>(string filePath,bool addToCache,AssetManager<T> assetManager,bool ntpMultiplePaths) where T : class
		{
			ReadyPath(ref filePath);

			if(importingBuiltInAssets || filePath.ToLower().StartsWith(builtInAssetsFolder.ToLower())) {
				return ImportBuiltInAsset(filePath,assetManager) as T;
			}

			if(!File.Exists(filePath)) {
				if(ntpMultiplePaths) {
					throw new FileNotFoundException($"Couldn't find file '{filePath}' for import. There were multiple path aliases for that filename.");
				}
				throw new FileNotFoundException($"Couldn't find file '{filePath}' for import.");
			}

			using var stream = File.OpenRead(filePath);
			var content = ImportFromStream(stream,assetManager,Path.GetFileName(filePath));
			cacheByPath[filePath] = content;

			return content;
		}
		internal static object ImportBuiltInAsset(string filePath,AssetManager manager = null,byte[] data = null)
		{
			ReadyPath(ref filePath);

			if(data==null && !builtInAssets.TryGetValue(filePath,out data)) {
				throw new FileNotFoundException($"Couldn't find built-in asset at '{filePath}'.");
			}

			if(manager==null) {
				if(!assetManagers.TryGetValue(Path.GetExtension(filePath).ToLower(),out var list)) {
					return null;
				}

				manager = list[0];
			}

			using var entryStream = new MemoryStream(data);

			var method = typeof(Resources).GetMethod("ImportFromStream",BindingFlags.Public | BindingFlags.Static);

			var tType = manager.GetType().BaseType?.GetGenericArguments()[0];
			if(tType==null) {
				return null;
			}

			var result = method.MakeGenericMethod(tType).Invoke(manager,new object[] { entryStream,manager,Path.GetFileName(filePath) });
			cacheByPath[filePath] = result;
			return result;
		}
		public static T ImportFromStream<T>(Stream stream,AssetManager<T> assetManager = null,string fileName = null) where T : class
		{
			var type = typeof(T);
			if(assetManager==null) {
				if(fileName==null) {
					throw new ArgumentException(
						"Could not figure out an AssetManager for import, since ''fileName'' parameter is null."+
						"\nProvide a proper file name, or provide an AssetManager with the ''assetManager'' parameter."
					);
				}

				string ext = Path.GetExtension(fileName).ToLower();
				if(!assetManagers.TryGetValue(ext,out var managers)) {
					throw new NotImplementedException("Could not find any asset managers for the ''"+ext+"'' extension.");
				}

				var results = managers.SelectIgnoreNull(q => q as AssetManager<T>).ToArray();
				if(results.Length==0) {
					throw new NotImplementedException("Could not find any ''"+ext+"'' asset managers which would return a "+type.Name+".");
				}

				assetManager = results[0];
			}

			var output = assetManager.Import(stream,fileName);
			InternalUtils.ObjectOrCollectionCall<Asset>(output,asset => asset.InitAsset(),false);
			return output;
		}

		public static string ImportText(string filePath,bool addToCache = false,bool throwOnFail = true) => Import(filePath,addToCache,(AssetManager<string>)assetManagers[".txt"][0],throwOnFail);
		public static byte[] ImportBytes(string filePath,bool addToCache = false,bool throwOnFail = true) => Import(filePath,addToCache,(AssetManager<byte[]>)assetManagers[".bytes"][0],throwOnFail);
		#endregion
		#region Get
		//Imports and caches files,or gets them from cache, if they have already been loaded.
		public static T Get<T>(string filePath) where T : class
		{
			ReadyPath(ref filePath);
			NameToPath(ref filePath,out bool ntpMultiplePaths);

			if(cacheByPath.TryGetValue(filePath,out var obj) && obj is T content) {
				return content;
			}

			return ImportInternal<T>(filePath,true,null,ntpMultiplePaths);
		}
		#endregion
		#region Find
		//Finds already loaded resources by their internal asset names, if they have them. Exists mostly for stuff like shaders.
		public static bool Find<T>(string assetName,out T asset) where T : class
			=> (asset = Find<T>(assetName))!=null;
		public static T Find<T>(string assetName,bool throwOnFail = true) where T : class
		{
			var type = typeof(T);

			if(cacheByName.TryGetValue(type,out var realDict) && realDict.TryGetValue(assetName, out var obj) && obj is T content) {
				return content;
			}

			if(throwOnFail) {
				throw new Exception($"Couldn't find '{typeof(T).Name}' asset with name '{assetName}'.");
			}

			return null;
		}
		#endregion
		#region Export
		//Exports a file
		public static void Export<T>(T asset,string filePath,AssetManager<T> assetManager = null) where T : class
		{
			var type = typeof(T);
			ReadyPath(ref filePath);
			
			if(assetManager==null) {
				string ext = Path.GetExtension(filePath).ToLower();
				if(!assetManagers.TryGetValue(ext,out var managers)) {
					throw new NotImplementedException("Could not find any asset managers for the ''"+ext+"'' extension.");
				}
				var results = managers.SelectIgnoreNull(q => q as AssetManager<T>).ToArray();
				if(results.Length!=1) {
					if(results.Length==0) {
						throw new NotImplementedException("Could not find any ''"+ext+"'' asset managers which would return a "+type.Name+".");
					}
					throw new NotImplementedException(
						"Found more than 1 ''"+ext+"'' asset managers which would return a "+type.Name+
						":\n"+string.Join(",\n",results.Select(q => q.GetType().Name))+
						"\n\nPlease specify which asset manager should be used via the ''assetManager'' parameter."
					);
				}
				assetManager = results[0];
			}

			using(var stream = File.OpenWrite(filePath)) {
				assetManager.Export(asset,stream);
			}
		}
		#endregion

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

		public static void AddToCache<T>(string filePath,T content)
		{
			cacheByPath[filePath] = content;
		}
		public static void RegisterFormats<T>(AssetManager<T> importer,string[] formats,bool allowOverwriting = false) where T : class
		{
			RegisterFormats(typeof(T),importer,formats,allowOverwriting);
		}
		private static void RegisterFormats(Type type,AssetManager assetManager,string[] extensions,bool allowOverwriting = false)
		{
			//TODO: There's unused parameters here?
			for(int i=0;i<extensions.Length;i++) {
				string ext = extensions[i];
				if(assetManagers.TryGetValue(ext,out var list)) {
					list.Add(assetManager);
				}else{
					list = new List<AssetManager> { assetManager };
				}
				assetManagers[ext] = list;
			}

			/*for(int i=0;i<formats.Length;i++) {
				string format = formats[i];
				if(!dict.ContainsKey(format) || allowOverwriting) {
					dict[format] = assetManager;
				}else{
					throw new Exception(type.Name+" importer for "+format+" extension is already defined: "+dict[format].GetType().FullName);
				}
			}*/
		}
		public static void SetDefaultManager(string ext,Type type)
		{
			ext = ext.ToLower();

			if(!assetManagers.TryGetValue(ext,out var list)) {
				throw new NotImplementedException("Could not find any ''"+ext+"'' asset managers.");
			}

			for(int i=0;i<list.Count;i++) {
				var manager = list[i];
				var generics = manager.GetType().GetGenericArguments();
				if(generics!=null && generics.Length>0 && generics[0]==type) {
					list.RemoveAt(i);
					list.Insert(0,manager);
					return;
				}
			}

			throw new NotImplementedException("Could not find any ''"+ext+"'' asset managers which would return a "+type.Name+".");
		}

		private static void ReadyPath(ref string path)
		{
			string lowerPath = path.ToLower();

			if(!Path.IsPathRooted(path)) {
				if(!lowerPath.StartsWith(builtInAssetsFolder.ToLower()) && !lowerPath.StartsWith("assets/")) {
					if(importingBuiltInAssets) {
						path = builtInAssetsFolder+path;
					}else{
						path = "Assets/"+path;
					}
				}
			}
		}
	}
}