using System;
using System.Collections.Generic;
using Dissonance.Engine.Utilities;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics;

//TODO: WIP
//TODO: Some fields shouldn't be public
public class Framebuffer : IDisposable
{
	public static Framebuffer DefaultFramebuffer { get; set; }

	public static Framebuffer ActiveBuffer { get; private set; }

	public readonly string Name;
	public readonly uint Id;

	private readonly Dictionary<RenderTexture, FramebufferAttachment> textureToAttachment;

	public List<RenderTexture> renderTextures;
	public Renderbuffer[] renderbuffers;

	private DrawBufferMode[] drawBuffers;

	internal FramebufferAttachment nextDefaultAttachment = FramebufferAttachment.ColorAttachment0;
	internal int maxTextureWidth;
	internal int maxTextureHeight;

	protected Framebuffer(string name)
	{
		Name = name;
		Id = OpenGL.GenFramebuffer();

		renderTextures = new List<RenderTexture>();

		textureToAttachment = new Dictionary<RenderTexture, FramebufferAttachment>();
	}

	public void AttachRenderTexture(RenderTexture texture, FramebufferAttachment? attachmentType = null)
	{
		Rendering.CheckGLErrors($"At the start of '{nameof(Framebuffer)}.{nameof(AttachRenderTexture)}'.");

		Bind(this);

		var attachment = attachmentType ?? nextDefaultAttachment++;

		OpenGL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, texture.Id, 0);

		renderTextures.Add(texture);

		textureToAttachment[texture] = attachment;

		var drawBuffersEnum = (DrawBufferMode)attachment;

		if (Enum.IsDefined(typeof(DrawBufferMode), drawBuffersEnum)) {
			ArrayUtils.Add(ref drawBuffers, drawBuffersEnum);
		}

		maxTextureWidth = Math.Max(maxTextureWidth, texture.Width);
		maxTextureHeight = Math.Max(maxTextureHeight, texture.Height);

		Rendering.CheckFramebufferStatus();

		Rendering.CheckGLErrors($"At the end of '{nameof(Framebuffer)}.{nameof(AttachRenderTexture)}'.");
	}

	public void AttachRenderTextures(params RenderTexture[] textures)
		=> AttachRenderTextures((IEnumerable<RenderTexture>)textures);

	public void AttachRenderTextures(IEnumerable<RenderTexture> textures)
	{
		foreach (var texture in textures) {
			AttachRenderTexture(texture);
		}
	}

	public void AttachRenderbuffer(Renderbuffer renderbuffer, FramebufferAttachment? attachmentType = null)
	{
		Bind(this);

		var attachment = attachmentType ?? nextDefaultAttachment++;

		OpenGL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer, Id);

		Rendering.CheckFramebufferStatus();

		ArrayUtils.Add(ref renderbuffers, renderbuffer);

		var drawBuffersEnum = (DrawBufferMode)attachment;

		if (Enum.IsDefined(drawBuffersEnum)) {
			ArrayUtils.Add(ref drawBuffers, drawBuffersEnum);
		}
	}

	public void DetachRenderTexture(RenderTexture texture)
	{
		if (!textureToAttachment.TryGetValue(texture, out var attachment)) {
			return;
		}

		Bind(this);

		OpenGL.FramebufferTexture(FramebufferTarget.Framebuffer, attachment, 0, 0);

		renderTextures.Remove(texture);
		textureToAttachment.Remove(texture);

		var drawBuffersEnum = (DrawBufferMode)attachment;

		if (Enum.IsDefined(drawBuffersEnum)) {
			int index = Array.IndexOf(drawBuffers, drawBuffersEnum);

			if (index >= 0) {
				ArrayUtils.Remove(ref drawBuffers, index);
			}
		}

		nextDefaultAttachment--;

		Bind(null);
	}

	public void PrepareAttachments()
	{
		if (renderTextures != null) {
			for (int i = 0; i < renderTextures.Count; i++) {
				renderTextures[i].UpdateSize();
			}
		}
	}

	public Framebuffer WithRenderTexture(RenderTexture texture, FramebufferAttachment? attachmentType = null)
	{
		AttachRenderTexture(texture, attachmentType);

		return this;
	}

	public Framebuffer WithRenderTextures(params RenderTexture[] textures)
	{
		AttachRenderTextures(textures);

		return this;
	}

	public Framebuffer WithRenderbuffer(Renderbuffer renderbuffer, FramebufferAttachment? attachmentType = null)
	{
		AttachRenderbuffer(renderbuffer, attachmentType);

		return this;
	}

	public void Dispose()
	{
		OpenGL.DeleteFramebuffer(Id);

		renderTextures = null;
		renderbuffers = null;
		drawBuffers = null;

		GC.SuppressFinalize(this);
	}

	public static Framebuffer Create(string name, Action<Framebuffer> initializer = null)
	{
		var fb = new Framebuffer(name);

		initializer?.Invoke(fb);

		return fb;
	}

	public static void Bind(Framebuffer fb, FramebufferTarget target = FramebufferTarget.Framebuffer)
	{
		fb ??= DefaultFramebuffer;

		OpenGL.BindFramebuffer(target, fb?.Id ?? 0);

		if (target == FramebufferTarget.Framebuffer) {
			ActiveBuffer = fb;
		}
	}

	public static void BindWithDrawBuffers(Framebuffer fb, FramebufferTarget target = FramebufferTarget.Framebuffer)
	{
		Bind(fb, target);

		if (fb != null) {
			OpenGL.DrawBuffers((uint)fb.drawBuffers.Length, fb.drawBuffers);
		}
	}
}
