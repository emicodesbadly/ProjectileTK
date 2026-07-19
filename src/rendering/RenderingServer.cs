using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using ProjectileTK.Utilities;

namespace ProjectileTK.Rendering
{
	public sealed class RenderingServer : IDisposable
	{
		private RenderingServer()
		{
			shaders  = [];
			textures = [];
		}

		// Lazy singleton implementation (NOT THREAD-SAFE!!!)
		private static readonly Lazy<RenderingServer> instance = new(() => new RenderingServer());
		public static RenderingServer Instance => instance.Value;

		private Dictionary<string, Shader> shaders;
		private Dictionary<string, Texture> textures;

		#region Shaders

		// Add shader if it doesn't exist already, or throw a warning
		public void AddShader(string shader)
		{
			if (shaders.ContainsKey(shader))
			{
				Utils.ThrowWarning($"Shader already exists! (name: {shader})");

				return;
			}

			shaders.Add(shader, new Shader(shader));
		}

		// Try to add shader, return true on success, false on failure
		public bool TryAddShader(string shader)
		{
			if (shaders.ContainsKey(shader))
			{
				return false;
			}

			shaders.Add(shader, new Shader(shader));

			return true;
		}

		// Activate shader for rendering, or throw a warning if it doesn't exist
		public void UseShader(string shader)
		{
			if (shaders.TryGetValue(shader, out Shader s))
			{
				s.Use();
			}
			else
			{
				Utils.ThrowWarning($"Shader not found! (name: {shader}");
			}
		}

		// Activate shader if it exists, return true on success
		public bool TryUseShader(string shader)
		{
			bool success = shaders.TryGetValue(shader, out Shader s);
			if (success)
			{
				s.Use();
			}

			return success;
		}

		// Get shader by name, without checking if it exists
		public Shader GetShader(string shader)
		{
			return shaders[shader];
		}

		// Returns true if shader exists & sets s equal to the shader
		// Otherwise returns false and sets s to null
		public bool TryGetShader(string shader, out Shader s)
		{
			bool success = shaders.TryGetValue(shader, out s);
			return success;
		}

		#endregion

		#region Textures

		// Add texture if it doesn't exist already, or throw a warning
		public void AddTexture(string texture)
		{
			if (textures.ContainsKey(texture))
			{
				Utils.ThrowWarning($"Texture already exists! (name: {texture})");

				return;
			}

			textures.Add(texture, new Texture(texture));
		}

		// Try to add texture, return true on success, false on failure
		public bool TryAddTexture(string texture)
		{
			if (textures.ContainsKey(texture))
			{
				return false;
			}

			textures.Add(texture, new Texture(texture));

			return true;
		}

		// Activate texture for rendering, or throw a warning if it doesn't exist
		public void UseTexture(string texture, TextureUnit unit = TextureUnit.Texture0)
		{
			if (textures.TryGetValue(texture, out Texture t))
			{
				t.Use(unit);
			}
			else
			{
				Utils.ThrowWarning($"Texture not found! (name: {texture}");
			}
		}

		// Get texture by name, without checking if it exists
		public Texture GetTexture(string texture)
		{
			return textures[texture];
		}

		// Returns true if texture exists & sets t equal to the shader
		// Otherwise returns false and sets t to null
		public bool TryGetTexture(string texture, out Texture t)
		{
			bool success = textures.TryGetValue(texture, out t);
			return success;
		}

		#endregion

		#region Dispose

		// IDisposable implementation
		private bool disposed = false;

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				// Dispose of shaders
				foreach (KeyValuePair<string, Shader> shader in shaders)
				{
					shader.Value.Dispose();
				}

				shaders = null;

				// Dispose of textures
				int i = 0;
				int[] handles = [textures.Count];

				foreach (KeyValuePair<string, Texture> texture in textures)
				{
					handles[i] = texture.Value.handle;
					i++;
				}

				GL.DeleteTextures(handles.Length, handles);

				textures = null;

				disposed = true;
			}
		}

		~RenderingServer()
		{
			if (!disposed)
			{
				throw new Exception($"(Rendering Server) GPU Resource leak! Did you forget to call Dispose()?");
			}
		}

		// MUST be called when the sprite is no longer needed!
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
