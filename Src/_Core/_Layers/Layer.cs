using Dissonance.Engine.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine;

[JsonConverter(typeof(LayerJsonConverter))]
public readonly struct Layer
{
	private readonly byte id;

	public int Index => id;
	public string Name => Layers.layerNames[id];
	public LayerMask Mask => Layers.layerMasks[id];

	public Layer(byte id)
	{
		this.id = id;
	}

	public override string ToString()
		=> $"Layer {Index} - {Name}";
}
