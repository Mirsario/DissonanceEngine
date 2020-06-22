using System;
using System.Text.RegularExpressions;
using Dissonance.Engine.Core;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	partial class Rendering
	{
		private static readonly Regex RegexGLVersion = new Regex(@".*?([\d.]+).*",RegexOptions.Compiled);
		
		public static Version GetOpenGLVersion()
		{
			string versionStr = GL.GetString(StringName.Version);
			
			var match = RegexGLVersion.Match(versionStr);

			if(!match.Success) {
				throw new Exception("Unable to catch OpenGL version with Regex.");
			}

			return new Version(match.Groups[1].Value);
		}
		
		[Obsolete("This call of CheckGLErrors was meant to be temporary.")]
		public static bool CheckGLErrorsTemp(string context = null,bool throwException = true) => CheckGLErrors(context,throwException);
		public static bool CheckGLErrors(string context = null,bool throwException = true)
		{
			GraphicsError error = GL.GetError();

			switch(error) {
				case GraphicsError.NoError:
					return false;
				default:
					string message = $"Error: '{error}'. Context: '{context ?? "Not provided"}'.";

					if(throwException) {
						throw new GraphicsException(message);
					} else {
						Debug.Log(message,stackframeOffset:2);
					}

					return true;
			}
		}

		internal static void CheckFramebufferStatus()
		{
			switch(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) {
				case FramebufferStatus.FramebufferComplete:
					return;

				case FramebufferStatus.FramebufferIncompleteAttachment:
					throw new Exception("An attachment could not be bound to frame buffer object.");

				case FramebufferStatus.FramebufferIncompleteMissingAttachment:
					throw new Exception("Attachments are missing! At least one image (texture) must be bound to the frame buffer object!");

				case FramebufferStatus.FramebufferIncompleteDimensionsExt:
					throw new Exception("The dimensions of the buffers attached to the currently used frame buffer object do not match!");

				case FramebufferStatus.FramebufferIncompleteFormatsExt:
					throw new Exception("The formats of the currently used frame buffer object are not supported or do not fit together!");

				case FramebufferStatus.FramebufferIncompleteDrawBuffer:
					throw new Exception("A Draw buffer is incomplete or undefinied. All draw buffers must specify attachment points that have images attached.");

				case FramebufferStatus.FramebufferIncompleteReadBuffer:
					throw new Exception("A Read buffer is incomplete or undefinied. All read buffers must specify attachment points that have images attached.");

				case FramebufferStatus.FramebufferIncompleteMultisample:
					throw new Exception("All images must have the same number of multisample samples.");

				case FramebufferStatus.FramebufferIncompleteLayerTargets :
					throw new Exception("If a layered image is attached to one attachment, then all attachments must be layered attachments. The attached layers do not have to have the same number of layers, nor do the layers have to come from the same kind of texture.");

				case FramebufferStatus.FramebufferUnsupported:
					throw new Exception("Attempt to use an unsupported format combinaton!");

				default:
					throw new Exception("Unknown error while attempting to create frame buffer object!");
			}
		}

		internal static void SetStencilMask(uint stencilMask)
		{
			if(stencilMask!=currentStencilMask) {
				GL.StencilMask(currentStencilMask = stencilMask);
			}
		}
		internal static void SetBlendFunc(BlendingFactor blendFactorSrc,BlendingFactor blendFactorDst)
		{
			if(blendFactorSrc!=currentBlendFactorSrc || blendFactorDst!=currentBlendFactorDst) {
				currentBlendFactorSrc = blendFactorSrc;
				currentBlendFactorDst = blendFactorDst;

				GL.BlendFunc(blendFactorSrc,blendFactorDst);
			}
		}
	}
}