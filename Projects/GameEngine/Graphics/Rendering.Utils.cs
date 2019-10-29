using System;
using System.Text.RegularExpressions;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GLBlendingFactor = OpenTK.Graphics.OpenGL.BlendingFactor;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine.Graphics
{
	public static partial class Rendering
	{
		private static readonly Regex RegexGLVersion = new Regex(@".*?([\d.]+).*",RegexOptions.Compiled);
		
		public static Version GetOpenGLVersion()
		{
			string versionStr = GL.GetString(StringName.Version);
			
			var match = RegexGLVersion.Match(versionStr);
			if(!match.Success) {
				throw new Exception("Unable to read OpenGL version. This is sad...");
			}

			return new Version(match.Groups[1].Value);
		}
		
		[Obsolete("This call of CheckGLErrors was meant to be temporary.")]
		public static bool CheckGLErrorsTemp(bool throwException = true,object prefix = null) => CheckGLErrors(throwException,prefix);
		public static bool CheckGLErrors(bool throwException = true,object prefix = null)
		{
			if(prefix==null) {
				prefix = "";
			}

			ErrorCode error = GL.GetError();
			switch(error) {
				case ErrorCode.NoError: return false;
				default:
					if(throwException) {
						throw new GraphicsException(prefix.ToString()+error);
					}else{
						Debug.Log(prefix.ToString()+error,stackframeOffset:2);
					}
					return true;
			}
		}

		internal static void CheckFramebufferStatus()
		{
			switch(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) {
				case FramebufferErrorCode.FramebufferComplete:
					return;
				case FramebufferErrorCode.FramebufferIncompleteAttachment:
					throw new Exception("An attachment could not be bound to frame buffer object!");
				case FramebufferErrorCode.FramebufferIncompleteMissingAttachment:
					throw new Exception("Attachments are missing! At least one image (texture) must be bound to the frame buffer object!");
				case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
					throw new Exception("The dimensions of the buffers attached to the currently used frame buffer object do not match!");
				case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
					throw new Exception("The formats of the currently used frame buffer object are not supported or do not fit together!");
				case FramebufferErrorCode.FramebufferIncompleteDrawBuffer:
					throw new Exception("A Draw buffer is incomplete or undefinied. All draw buffers must specify attachment points that have images attached.");
				case FramebufferErrorCode.FramebufferIncompleteReadBuffer:
					throw new Exception("A Read buffer is incomplete or undefinied. All read buffers must specify attachment points that have images attached.");
				case FramebufferErrorCode.FramebufferIncompleteMultisample:
					throw new Exception("All images must have the same number of multisample samples.");
				case FramebufferErrorCode.FramebufferIncompleteLayerTargets :
					throw new Exception("If a layered image is attached to one attachment,then all attachments must be layered attachments. The attached layers do not have to have the same number of layers,nor do the layers have to come from the same kind of texture.");
				case FramebufferErrorCode.FramebufferUnsupported:
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
				GL.BlendFunc((GLBlendingFactor)blendFactorSrc,(GLBlendingFactor)blendFactorDst);
			}
		}
	}
}