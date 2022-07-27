using System;
using System.Collections.Generic;

namespace Dissonance.Engine;

public sealed class Layers : EngineModule
{
	internal static readonly List<Layer> layers = new();
	internal static readonly List<string> layerNames = new();
	internal static readonly List<LayerMask> layerMasks = new();
	internal static readonly Dictionary<string, Layer> layersByName = new();

	/// <summary> The maximum amount of layers that can be defined. </summary>
	public static int MaxLayers { get; } = sizeof(ulong) * 8;
	/// <summary> The default layer. </summary>
	public static Layer DefaultLayer { get; private set; }
	/// <summary> A read-only list containing all registered layers. </summary>
	public static IReadOnlyList<Layer> AllLayers { get; private set; } = layers.AsReadOnly();

	/// <summary> The current amount of registered layers. </summary>
	public static int LayerCount => layers.Count;

	protected override void Init()
	{
		DefaultLayer = AddLayer("Default");
	}

	/// <summary> Declares a new layer with the provided name and returns it. </summary>
	public static Layer AddLayer(string layerName)
	{
		int id = LayerCount;

		if (id >= MaxLayers) {
			throw new InvalidOperationException($"Cannot add more than {MaxLayers} layers.");
		}

		if (layersByName.ContainsKey(layerName)) {
			throw new InvalidOperationException($"Layer {layerName} is already defined.");
		}

		var layer = new Layer((byte)id);
		LayerMask layerMask = new LayerMask(1ul << id);

		layersByName[layerName] = layer;

		layerMasks.Add(layerMask);
		layerNames.Add(layerName);
		layers.Add(layer);

		return layer;
	}

	/// <summary> Safely attempts to find and return the layer with the provided name. </summary>
	public static bool TryGetLayer(string layerName, out Layer result)
		=> layersByName.TryGetValue(layerName, out result);

	/// <summary> Finds and returns the layer with the provided name, if successful. Throws an exception on failure. </summary>
	/// <exception cref="KeyNotFoundException"/>
	public static Layer GetLayer(string layerName)
	{
		if (!layersByName.TryGetValue(layerName, out var result)) {
			throw new KeyNotFoundException($"Unknown layer: '{layerName}'.");
		}

		return result;
	}
}
