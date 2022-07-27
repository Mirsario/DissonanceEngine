using System;
using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics;

public abstract class CustomVertexAttribute
{
	public string NameId { get; protected set; }
	public VertexAttribPointerType PointerType { get; protected set; }
	public bool IsNormalized { get; protected set; }
	public int Size { get; protected set; }
	public uint Stride { get; protected set; }
	public int Offset { get; protected set; }

	public abstract Type BufferType { get; }

	internal CustomVertexAttribute() { }
}

public abstract class CustomVertexAttribute<TBuffer> : CustomVertexAttribute where TBuffer : CustomVertexBuffer, new()
{
	public override Type BufferType => typeof(TBuffer);

	protected CustomVertexAttribute() : base()
	{
		// This is quite weird.

		Init(out string nameId, out var pointerType, out bool isNormalized, out int size, out uint stride, out int offset);

		NameId = nameId;
		PointerType = pointerType;
		IsNormalized = isNormalized;
		Size = size;
		Stride = stride;
		Offset = offset;
	}

	public abstract void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out uint stride, out int offset);
}
