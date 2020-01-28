using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine
{
	public abstract class Asset : IDisposable
	{
		protected static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone",BindingFlags.NonPublic | BindingFlags.Instance);

		public virtual string AssetName => null;

		public virtual void Dispose() { }

		public void RegisterAsset()
		{
			string name = AssetName;

			if(name!=null) {
				var type = GetType();

				if(!Resources.cacheByName.TryGetValue(type,out var dict)) {
					Resources.cacheByName[type] = dict = new Dictionary<string,object>();
				}

				dict[name] = this;
			}
		}
	}

	public abstract class Asset<T> : Asset where T : class
	{
		public virtual T Clone() => (T)InternalClone(this);
		public virtual T DeepClone() => (T)InternalDeepClone(this,new Dictionary<object,object>(new ReferenceEqualityComparer()));

		#region ShallowClone
		//Copy only original object and arrays
		private static object InternalClone(object obj)
		{
			if(obj==null) {
				return null;
			}

			var type = obj.GetType();
			if(IsPrimitive(type)) {
				return obj;
			}

			var clone = CloneMethod.Invoke(obj,null);

			RecreateArraysRecursive(obj,clone,type);

			return clone;
		}
		private static void RecreateArraysRecursive(object originalObject,object cloneObject,Type reflectionType)
		{
			foreach(var fieldInfo in reflectionType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)) {
				if(IsPrimitive(fieldInfo.FieldType)) {
					continue;
				}

				var originalFieldValue = fieldInfo.GetValue(originalObject);
				var clonedFieldValue = InternalClone(originalFieldValue);

				fieldInfo.SetValue(cloneObject,clonedFieldValue);
			}
		}
		#endregion
		#region DeepClone
		//Deep Copy
		private static object InternalDeepClone(object originalObject,IDictionary<object,object> visited)
		{
			if(originalObject==null) {
				return null;
			}

			var reflectionType = originalObject.GetType();
			if(IsPrimitive(reflectionType)) {
				return originalObject;
			}

			if(visited.ContainsKey(originalObject)) {
				return visited[originalObject];
			}

			if(typeof(Delegate).IsAssignableFrom(reflectionType)) {
				return null;
			}

			var cloneObject = CloneMethod.Invoke(originalObject,null);
			if(reflectionType.IsArray) {
				var arrayType = reflectionType.GetElementType();

				if(!IsPrimitive(arrayType)) {
					var clonedArray = (Array)cloneObject;

					clonedArray.ForEach((array,indices)=>array.SetValue(InternalDeepClone(clonedArray.GetValue(indices),visited),indices));
				}
			}

			visited.Add(originalObject,cloneObject);

			CopyFields(originalObject,visited,cloneObject,reflectionType);
			RecursiveCopyBaseTypePrivateFields(originalObject,visited,cloneObject,reflectionType);

			return cloneObject;
		}
		private static void RecursiveCopyBaseTypePrivateFields(object originalObject,IDictionary<object,object> visited,object cloneObject,Type reflectionType)
		{
			if(reflectionType.BaseType!=null) {
				RecursiveCopyBaseTypePrivateFields(originalObject,visited,cloneObject,reflectionType.BaseType);
				CopyFields(originalObject,visited,cloneObject,reflectionType.BaseType,BindingFlags.Instance | BindingFlags.NonPublic,info => info.IsPrivate);
			}
		}
		private static void CopyFields(object originalObject,IDictionary<object,object> visited,object cloneObject,Type reflectionType,BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,Func<FieldInfo,bool> filter = null)
		{
			foreach(var fieldInfo in reflectionType.GetFields(bindingFlags)) {
				if(IsPrimitive(fieldInfo.FieldType) || (filter!=null && !filter(fieldInfo))) {
					continue;
				}

				var originalFieldValue = fieldInfo.GetValue(originalObject);
				var clonedFieldValue = InternalDeepClone(originalFieldValue,visited);

				fieldInfo.SetValue(cloneObject,clonedFieldValue);
			}
		}
		#endregion

		private static bool IsPrimitive(Type type) => type==typeof(string) || type.IsValueType & type.IsPrimitive;
	}

	//TODO: Move all these to their own files, jeez
	internal class ReferenceEqualityComparer : EqualityComparer<object>
	{
		public override bool Equals(object x,object y) => ReferenceEquals(x,y);
		public override int GetHashCode(object obj) => obj==null ? 0 : obj.GetHashCode();
	}
	internal static class ArrayExtensions
	{
		public static void ForEach(this Array array,Action<Array,int[]> action)
		{
			if(array.LongLength==0) {
				return;
			}

			var walker = new ArrayTraverse(array);

			do action(array,walker.position);
			while(walker.Step());
		}

		private class ArrayTraverse
		{
			private readonly int[] MaxLengths;

			public int[] position;

			public ArrayTraverse(Array array)
			{
				MaxLengths = new int[array.Rank];

				for(int i = 0;i < array.Rank;++i) {
					MaxLengths[i] = array.GetLength(i)-1;
				}

				position = new int[array.Rank];
			}
			public bool Step()
			{
				for(int i = 0;i < position.Length;++i) {
					if(position[i] < MaxLengths[i]) {
						position[i]++;

						for(int j = 0;j < i;j++) {
							position[j] = 0;
						}

						return true;
					}
				}

				return false;
			}
		}
	}
}
