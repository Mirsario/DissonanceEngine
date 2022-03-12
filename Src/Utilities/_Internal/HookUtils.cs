using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine.Utilities
{
	internal static class HookUtils
	{
		/// <summary> Fills delegate fields/properties on <typeparamref name="THookHolder"/> type with ordered combined delegates made from virtual methods of <paramref name="objects"/> array. </summary>
		public static void BuildHooksFromVirtualMethods<THookHolder, TMethodHolder>(IEnumerable<TMethodHolder> objects, THookHolder hookHolderInstance = null, Comparison<(TMethodHolder methodHolder, Delegate method, int hookPosition)> customSorting = null)
			where THookHolder : class
			where TMethodHolder : class
		{
			// Check the objects array before proceeding.
			foreach (var obj in objects) {
				if (obj == null) {
					throw new ArgumentException($"'{nameof(objects)}' cannot contain null values.");
				}
			}

			var hookHolderType = typeof(THookHolder);
			var methodHolderType = typeof(TMethodHolder);

			// Enumerate virtual method definitions
			foreach (var method in methodHolderType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
				var attribute = method.GetCustomAttribute<VirtualMethodHookAttribute>();

				if (attribute == null || attribute.HookHolder != hookHolderType) {
					continue;
				}

				if (!method.IsVirtual) {
					throw new InvalidOperationException($"Method '{methodHolderType.Name}.{method.Name}' is expected to be virtual because of the '{nameof(VirtualMethodHookAttribute)}' attribute on it.");
				}

				var hookFlags = BindingFlags.Public | BindingFlags.NonPublic | (attribute.IsStatic ? BindingFlags.Static : BindingFlags.Instance);

				MemberInfo hookMember = attribute.IsProperty
					? attribute.HookHolder.GetProperty(attribute.HookName)
					: attribute.HookHolder.GetField(attribute.HookName);

				if (hookMember == null) {
					throw new ArgumentException($"Could not find a hook {(attribute.IsProperty ? "property" : "field")} on type {attribute.HookHolder.Name}");
				}

				var hookMemberValueType = attribute.IsProperty
					? ((PropertyInfo)hookMember).PropertyType
					: ((FieldInfo)hookMember).FieldType;

				if (!typeof(Delegate).IsAssignableFrom(hookMemberValueType)) {
					throw new ArgumentException($"{(attribute.IsProperty ? "Property" : "Field")} '{attribute.HookName}' on type '{attribute.HookHolder.Name}' does not derive from type '{nameof(Delegate)}'.");
				}

				var methodFlags = BindingFlags.Instance | (method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);
				var delegates = new List<(TMethodHolder methodHolder, Delegate method, int hookPosition)>();

				// Enumerate objects to search for overrides of the current virtual method.
				foreach (var obj in objects) {
					var objType = obj.GetType();
					var objMethod = objType.GetMethod(method.Name, methodFlags);

					if (objMethod == null) {
						throw new Exception($"Couldn't find method '{objMethod.Name}' on type '{objType.Name}'. This shouldn't happen.");
					}

					if (objMethod.DeclaringType == methodHolderType) {
						continue;
					}

					var func = objMethod.CreateDelegate(hookMemberValueType, obj);
					int priority = objMethod.GetCustomAttribute<HookPositionAttribute>()?.Position ?? 0;

					delegates.Add((obj, func, priority));
				}

				delegates.Sort(customSorting ?? ((tupleA, tupleB) => tupleA.hookPosition > tupleB.hookPosition ? 1 : -1));

				var combinedDelegate = Delegate.Combine(delegates
					.Select(tuple => tuple.method)
					.ToArray()
				);

				if (attribute.IsProperty) {
					((PropertyInfo)hookMember).SetValue(hookHolderInstance, combinedDelegate);
				} else {
					((FieldInfo)hookMember).SetValue(hookHolderInstance, combinedDelegate);
				}
			}
		}
	}
}
