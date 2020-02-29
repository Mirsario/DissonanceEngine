using System.Collections.Generic;
using Dissonance.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO.Graphics.Models
{
	partial class GltfManager
	{
		public abstract class JsonElementBase
		{
			/// <summary> Dictionary object with extension-specific objects. </summary>
			public JObject extensions;
			/// <summary> Application-specific data. </summary>
			public JToken extras;
		}

		public abstract class NamedJsonElementBase : JsonElementBase
		{
			/// <summary> The user-defined name of this object. This is not necessarily unique, e.g., an accessor and a buffer could have the same name, or two accessors could even have the same name. </summary>
			public string name;
		}

		/// <summary> The root object for a glTF asset. </summary>
		public class GltfJson : JsonElementBase
		{
			/// <summary> A typed view into a bufferView. A bufferView contains raw binary data. An accessor provides a typed view into a bufferView or a subset of a bufferView similar to how GL's VertexAttribPointer() defines an attribute in a buffer. </summary>
			public class Accessor : NamedJsonElementBase
			{
				/// <summary> Sparse storage of attributes that deviate from their initialization value. </summary>
				public class SparseStorage : JsonElementBase
				{
					/// <summary> Indices of those attributes that deviate from their initialization value. </summary>
					public class Indices : JsonElementBase
					{
						/// <summary> The index of the bufferView with sparse indices. Referenced bufferView can't have ARRAY_BUFFER or ELEMENT_ARRAY_BUFFER target. </summary>
						[JsonRequired]
						public uint bufferView;

						/// <summary> The indices data type. </summary>
						[JsonRequired]
						public ComponentType componentType;

						/// <summary> The offset relative to the start of the bufferView in bytes. Must be aligned. </summary>
						public uint byteOffset;
					}

					/// <summary> Array of size accessor.sparse.count times number of components storing the displaced accessor attributes pointed by accessor.sparse.indices. </summary>
					public class Values : JsonElementBase
					{
						/// <summary> The index of the bufferView with sparse values. Referenced bufferView can't have ARRAY_BUFFER or ELEMENT_ARRAY_BUFFER target. </summary>
						[JsonRequired]
						public uint bufferView;

						/// <summary> The offset relative to the start of the bufferView in bytes. Must be aligned. </summary>
						public uint byteOffset;
					}

					/// <summary> Number of entries stored in the sparse array. </summary>
					[JsonRequired]
					public uint count;

					/// <summary> Index array of size count that points to those accessor attributes that deviate from their initialization value. Indices must strictly increase. </summary>
					[JsonRequired]
					public Indices indices;

					/// <summary> Array of size count times number of components, storing the displaced accessor attributes pointed by indices. Substituted values must have the same componentType and number of components as the base accessor. </summary>
					[JsonRequired]
					public Values values;
				}

				/// <summary> The datatype of components in the attribute. </summary>
				[JsonRequired]
				public ComponentType componentType;

				/// <summary> The number of attributes referenced by this accessor. </summary>
				[JsonRequired]
				public uint count;

				/// <summary> Specifies if the attribute is a scalar, vector, or matrix. </summary>
				[JsonRequired]
				public string type;

				/// <summary> The index of the bufferView. When not defined, accessor must be initialized with zeros; sparse property or extensions could override zeros with actual values. </summary>
				public uint? bufferView;
				/// <summary> The offset relative to the start of the bufferView in bytes. </summary>
				public uint byteOffset;
				/// <summary> Specifies whether integer data values should be normalized. </summary>
				public bool normalized;
				/// <summary> Maximum value of each component in this attribute. </summary>
				public float[] max;
				/// <summary> Minimum value of each component in this attribute. </summary>
				public float[] min;
				/// <summary> Sparse storage of attributes that deviate from their initialization value. </summary>
				public SparseStorage sparse;
			}

			/// <summary> A keyframe animation. </summary>
			public class Animation : NamedJsonElementBase
			{
				/// <summary> Combines input and output accessors with an interpolation algorithm to define a keyframe graph (but not its target). </summary>
				public class AnimationSampler : JsonElementBase
				{
					/// <summary> The index of an accessor containing keyframe input values, e.g., time. </summary>
					[JsonRequired]
					public uint input;

					/// <summary> The index of an accessor, containing keyframe output values. </summary>
					[JsonRequired]
					public uint output;

					/// <summary> Interpolation algorithm. </summary>
					public InterpolationAlgorithm interpolation = InterpolationAlgorithm.Linear;
				}

				/// <summary> Targets an animation's sampler at a node's property. </summary>
				public class Channel : JsonElementBase
				{
					/// <summary> The index of the node and TRS property that an animation channel targets. </summary>
					public class Target : JsonElementBase
					{
						/// <summary>
						/// The name of the node's TRS property to modify, or the "weights" of the Morph Targets it instantiates.<br/>
						/// For the "translation" property, the values that are provided by the sampler are the translation along the x, y, and z axes.<br/>
						/// For the "rotation" property, the values are a quaternion in the order (x, y, z, w), where w is the scalar.<br/>
						/// For the "scale" property, the values are the scaling factors along the x, y, and z axes.
						/// </summary>
						[JsonRequired]
						public string path;

						/// <summary> The index of the node to target. </summary>
						public uint node;
					}

					/// <summary> The index of a sampler in this animation used to compute the value for the target. </summary>
					[JsonRequired]
					public uint sampler;

					/// <summary> The index of the node and TRS property to target. </summary>
					[JsonRequired]
					public Target target;
				}

				/// <summary> An array of channels, each of which targets an animation's sampler at a node's property. Different channels of the same animation can't have equal targets. </summary>
				[JsonRequired]
				public Channel[] channels;

				/// <summary> An array of samplers that combines input and output accessors with an interpolation algorithm to define a keyframe graph (but not its target). </summary>
				[JsonRequired]
				public AnimationSampler[] samplers;
			}

			/// <summary> Metadata about the glTF asset. </summary>
			public class Asset : JsonElementBase
			{
				/// <summary> The glTF version that this asset targets. </summary>
				[JsonRequired]
				public string version;

				/// <summary> A copyright message suitable for display to credit the content creator. </summary>
				public string copyright;
				/// <summary> Tool that generated this glTF model. Useful for debugging. </summary>
				public string generator;
				/// <summary> The minimum glTF version that this asset targets. </summary>
				public string minVersion;
			}

			/// <summary> A buffer points to binary geometry, animation, or skins. </summary>
			public class Buffer : NamedJsonElementBase
			{
				/// <summary> The total byte length of the buffer view. </summary>
				[JsonRequired]
				public uint byteLength;

				/// <summary> The uri of the buffer. Relative paths are relative to the .gltf file. Instead of referencing an external file, the uri can also be a data-uri. </summary>
				public string uri;
			}

			/// <summary> A view into a buffer generally representing a subset of the buffer. </summary>
			public class BufferView : NamedJsonElementBase
			{
				/// <summary> The index of the buffer. </summary>
				[JsonRequired]
				public uint buffer;

				/// <summary> The length of the bufferView in bytes. </summary>
				[JsonRequired]
				public uint byteLength;

				/// <summary> The offset into the buffer in bytes. </summary>
				public uint byteOffset;
				/// <summary> The stride, in bytes, between vertex attributes. When this is not defined, data is tightly packed. When two or more accessors use the same bufferView, this field must be defined.<br/> Min: 4.<br/>Max: 252. </summary>
				public byte byteStride;
				/// <summary> The target that the GPU buffer should be bound to. </summary>
				public BufferTarget target;
			}

			/// <summary> A camera's projection. A node can reference a camera to apply a transform to place the camera in the scene. </summary>
			public class Camera : NamedJsonElementBase
			{
				/// <summary> An orthographic camera containing properties to create an orthographic projection matrix. </summary>
				public class Ortographic : JsonElementBase
				{
					/// <summary> The floating-point horizontal magnification of the view. </summary>
					[JsonRequired]
					public float xmag;

					/// <summary> The floating-point vertical magnification of the view. </summary>
					[JsonRequired]
					public float ymag;

					/// <summary> The floating-point distance to the far clipping plane. zfar must be greater than znear. </summary>
					[JsonRequired]
					public float zfar;

					/// <summary> The floating-point distance to the near clipping plane. </summary>
					[JsonRequired]
					public float znear;
				}

				/// <summary> A perspective camera containing properties to create a perspective projection matrix. </summary>
				public class Perspective : JsonElementBase
				{
					/// <summary> The floating-point vertical field of view in radians. </summary>
					[JsonRequired]
					public float yfov;

					/// <summary> The floating-point distance to the near clipping plane. </summary>
					[JsonRequired]
					public float znear;

					/// <summary> The floating-point aspect ratio of the field of view. When this is undefined, the aspect ratio of the canvas is used. </summary>
					public float? aspectRatio;
					/// <summary> The floating-point distance to the far clipping plane. When defined, zfar must be greater than znear. If zfar is undefined, runtime must use infinite projection matrix. </summary>
					public float? zfar;
				}

				/// <summary> Specifies if the camera uses a perspective or orthographic projection. </summary>
				[JsonRequired]
				public CameraType type;

				/// <summary> An orthographic camera containing properties to create an orthographic projection matrix. </summary>
				public Ortographic orthographic;
				/// <summary> A perspective camera containing properties to create a perspective projection matrix. </summary>
				public Perspective perspective;
			}

			/// <summary> Image data used to create a texture. Image can be referenced by URI or bufferView index. mimeType is required in the latter case. </summary>
			public class Image : NamedJsonElementBase
			{
				/// <summary> The uri of the image. Relative paths are relative to the .gltf file. Instead of referencing an external file, the uri can also be a data-uri. The image format must be jpg or png. </summary>
				public string uri;
				/// <summary> The image's MIME type. Required if bufferView is defined. </summary>
				public string mimeType;
				/// <summary> The index of the bufferView that contains the image. Use this instead of the image's uri property. </summary>
				public uint? bufferView;
			}

			/// <summary> The material appearance of a primitive. </summary>
			public class Material : NamedJsonElementBase
			{
				/// <summary> Reference to a texture. </summary>
				public class NormalTextureInfo : TextureInfo
				{
					/// <summary> The scalar multiplier applied to each normal vector of the normal texture. </summary>
					public float scale = 1f;
				}

				/// <summary> Reference to a texture. </summary>
				public class OcclusionTextureInfo : TextureInfo
				{
					/// <summary> A scalar multiplier controlling the amount of occlusion applied. </summary>
					public float strength = 1f;
				}

				/// <summary> A set of parameter values that are used to define the metallic-roughness material model from Physically-Based Rendering (PBR) methodology. </summary>
				public class PbrMetallicRoughness : JsonElementBase
				{
					/// <summary> The material's base color factor. </summary>
					public float[] baseColorFactor = { 1f,1f,1f,1f };
					/// <summary> The base color texture. </summary>
					public JObject baseColorTexture;
					/// <summary> The metalness of the material. </summary>
					public float metallicFactor = 1f;
					/// <summary> The roughness of the material. </summary>
					public float roughnessFactor = 1f;
					/// <summary> The metallic-roughness texture. </summary>
					public JObject metallicRoughnessTexture;
				}

				/// <summary> A set of parameter values that are used to define the metallic-roughness material model from Physically-Based Rendering (PBR) methodology. When not specified, all the default values of pbrMetallicRoughness apply. </summary>
				public PbrMetallicRoughness pbrMetallicRoughness;
				/// <summary> The normal map texture. </summary>
				public NormalTextureInfo normalTexture;
				/// <summary> The occlusion map texture. </summary>
				public OcclusionTextureInfo occlusionTexture;
				/// <summary> The emissive color of the material. </summary>
				public float[] emissiveFactor = { 0f,0f,0f };
				/// <summary> The alpha rendering mode of the material. </summary>
				public string alphaMode = "OPAQUE";
				/// <summary> The alpha cutoff value of the material. </summary>
				public float alphaCutoff = 0.5f;
				/// <summary> Specifies whether the material is double sided. </summary>
				public bool doubleSided;
			}

			/// <summary> A set of primitives to be rendered. A node can contain one mesh. A node's transform places the mesh in the scene. </summary>
			public class Mesh : NamedJsonElementBase
			{
				/// <summary> Geometry to be rendered with the given material. </summary>
				public class Primitive : JsonElementBase
				{
					/// <summary> A dictionary object, where each key corresponds to mesh attribute semantic and each value is the index of the accessor containing attribute's data. </summary>
					[JsonRequired]
					public Dictionary<string,int> attributes;

					/// <summary> The index of the accessor that contains the indices. </summary>
					public uint? indices;
					/// <summary> The index of the material to apply to this primitive when rendering. </summary>
					public uint? material;
					/// <summary> The type of primitives to render. All valid values correspond to GL enums. </summary>
					public PrimitiveType mode;
				}

				/// <summary> An array of primitives, each defining geometry to be rendered with a material. </summary>
				[JsonRequired]
				public Primitive[] primitives;

				/// <summary> Array of weights to be applied to the Morph Targets. </summary>
				public float[] weights;
			}

			/// <summary>
			/// A node in the node hierarchy.<br/>
			/// When the node contains skin, all mesh.primitives must contain JOINTS_0 and WEIGHTS_0 attributes.<br/>
			/// A node can have either a matrix or any combination of translation/rotation/scale (TRS) properties.<br/>
			/// TRS properties are converted to matrices and postmultiplied in the T * R * S order to compose the transformation matrix;<br/>
			/// first the scale is applied to the vertices, then the rotation, and then the translation.<br/>
			/// If none are provided, the transform is the identity.<br/>
			/// When a node is targeted for animation (referenced by an animation.channel.target), only TRS properties may be present; matrix will not be present.
			/// </summary>
			public class Node : NamedJsonElementBase
			{
				/// <summary> The index of the camera referenced by this node. </summary>
				public uint? camera;
				/// <summary>  [1-*] 	The indices of this node's children. </summary>
				public uint[] children;
				/// <summary> The index of the skin referenced by this node. </summary>
				public uint? skin;
				/// <summary> A floating-point 4x4 transformation matrix stored in column-major order. </summary>
				public float[] matrix;
				/// <summary> The index of the mesh in this node. </summary>
				public int? mesh;
				/// <summary> The node's unit quaternion rotation in the order (x, y, z, w), where w is the scalar. </summary>
				public float[] rotation = { 0f,0f,0f,1f };
				/// <summary> The node's non-uniform scale, given as the scaling factors along the x, y, and z axes. </summary>
				public float[] scale = { 1f,1f,1f };
				/// <summary> The node's translation along the x, y, and z axes. </summary>
				public float[] translation = { 0f,0f,0f };
				/// <summary> The weights of the instantiated Morph Target. Number of elements must match number of Morph Targets of used mesh. </summary>
				public float[] weights;
			}

			/// <summary> Texture sampler properties for filtering and wrapping modes. </summary>
			public class Sampler : NamedJsonElementBase
			{
				/// <summary> Magnification filter. </summary>
				public TextureMagFilter magFilter;
				/// <summary> Minification filter. </summary>
				public TextureMinFilter minFilter;
				/// <summary> S wrapping mode. </summary>
				public TextureWrapMode wrapS;
				/// <summary> T wrapping mode. </summary>
				public TextureWrapMode wrapT;
			}

			/// <summary> The root nodes of a scene. </summary>
			public class Scene : NamedJsonElementBase
			{
				/// <summary> The indices of each root node. </summary>
				public uint[] nodes;
			}

			/// <summary> Joints and matrices defining a skin. </summary>
			public class Skin : NamedJsonElementBase
			{
				/// <summary> Indices of skeleton nodes, used as joints in this skin. </summary>
				[JsonRequired]
				public uint[] joints;

				/// <summary> The index of the accessor containing the floating-point 4x4 inverse-bind matrices. The default is that each matrix is a 4x4 identity matrix, which implies that inverse-bind matrices were pre-applied. </summary>
				public uint? inverseBindMatrices;
				/// <summary> The index of the node used as a skeleton root. </summary>
				public uint? skeleton;
			}

			/// <summary> A texture and its sampler. </summary>
			public class Texture : NamedJsonElementBase
			{
				/// <summary> The index of the sampler used by this texture. When undefined, a sampler with repeat wrapping and auto filtering should be used. </summary>
				public uint? sampler;
				/// <summary> The index of the image used by this texture. When undefined, it is expected that an extension or other mechanism will supply an alternate texture source, otherwise behavior is undefined. </summary>
				public uint? source;
			}

			/// <summary> Reference to a texture. </summary>
			public class TextureInfo : JsonElementBase
			{
				/// <summary> The index of the texture. </summary>
				[JsonRequired]
				public uint index;

				/// <summary> The set index of texture's TEXCOORD attribute used for texture coordinate mapping. </summary>
				public uint texCoord;
			}

			/// <summary> Metadata about the glTF asset. </summary>
			[JsonRequired]
			public Asset asset;

			/// <summary> Names of glTF extensions used somewhere in this asset. </summary>
			public string[] extensionsUsed;
			/// <summary> Names of glTF extensions required to properly load this asset. </summary>
			public string[] extensionsRequired;
			/// <summary> An array of accessors. </summary>
			public Accessor[] accessors;
			/// <summary> An array of keyframe animations. </summary>
			public Animation[] animations;
			/// <summary> An array of buffers. </summary>
			public Buffer[] buffers;
			/// <summary> An array of bufferViews. </summary>
			public BufferView[] bufferViews;
			/// <summary> An array of cameras. </summary>
			public Camera[] cameras;
			/// <summary> An array of images. </summary>
			public Image[] images;
			/// <summary> An array of materials. </summary>
			public Material[] materials;
			/// <summary> An array of meshes. </summary>
			public Mesh[] meshes;
			/// <summary> An array of nodes. </summary>
			public Node[] nodes;
			/// <summary> An array of samplers. </summary>
			public Sampler[] samplers;
			/// <summary> The index of the default scene. </summary>
			public uint? scene;
			/// <summary> An array of scenes. </summary>
			public Scene[] scenes;
			/// <summary> An array of skins. </summary>
			public Skin[] skins;
			/// <summary> An array of textures. </summary>
			public Texture[] textures;
		}
	}
}
