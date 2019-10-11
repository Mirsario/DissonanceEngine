﻿using GameEngine;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderingPipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class CustomRenderingPipeline : DeferredRendering
	{
		public PostProcessPass fogPass;

		public override void Setup(List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{
			base.Setup(framebuffers,renderPasses);

			int guiIndex = renderPasses.FindIndex(p => p.name=="GUI");
			var mainBuffer = framebuffers.Find(p => p.Name=="mainBuffer");

			fogPass = RenderPass.Create<PostProcessPass>("Fog",p => {
				p.Shader = Resources.Find<Shader>("Fog");

				p.PassedTextures = new RenderTexture[] {
					mainBuffer.renderTextures.First(rt => rt.name=="depthBuffer"),
					mainBuffer.renderTextures.First(rt => rt.name=="normalBuffer"),
					mainBuffer.renderTextures.First(rt => rt.name=="positionBuffer")
				};

				//p.renderbuffers = new Renderbuffer[] {
				//	mainBuffer.renderbuffers.First(rb => rb.Name=="depthBuffer")
				//};

				//p.Framebuffer = mainBuffer;
			});

			renderPasses.Insert(guiIndex,fogPass);
		}
		public override void PreRender()
		{
			base.PreRender();

			fogPass.enabled = !Input.GetKey(Keys.T);
		}
	}
}
