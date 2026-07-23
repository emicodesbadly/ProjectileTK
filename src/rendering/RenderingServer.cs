using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using ProjectileTK.Utilities;

namespace ProjectileTK.Rendering
{
	public sealed class RenderingServer : IDisposable
	{
		// Lazy singleton implementation (NOT THREAD-SAFE!!!)
		private static readonly Lazy<RenderingServer> instance = new(() => new RenderingServer());
		public static RenderingServer Instance => instance.Value;

		// GPU Resources
		private Screen screen;
		public Screen Screen => screen;

		private Dictionary<string, Shader> shaders;
		private Dictionary<string, Texture> textures;
		private Dictionary<string, Sprite> sprites;
		private SortedSet<Sprite> spriteRenderingOrder;

		private RenderingServer()
		{
			shaders  = [];
			textures = [];
			sprites  = [];

			spriteRenderingOrder = new(new SpritePriorityComparer());
		}

		public void CreateScreen(Window window, (int width, int height) targetResolution)
		{
			screen = new Screen(window, targetResolution);
		}

		#region Shaders

		public bool NewShader(string name)
		{
			if (shaders.ContainsKey(name))
			{
				Utils.ThrowWarning(this, $"A shader with name \"{name}\" already exists!");

				return false;
			}

			shaders.Add(name, new Shader(name));

			return true;
		}

		public bool UseAndGetShader(string name, out Shader shader)
		{
			if (!shaders.TryGetValue(name, out shader))
			{
				Utils.ThrowWarning(this, $"No shader with name \"{name}\" exists!");

				return false;
			}

			return true;
		}

		public bool UseShader(string name)
		{
			if (!shaders.TryGetValue(name, out Shader shader))
			{
				Utils.ThrowWarning(this, $"No shader with name \"{name}\" exists!");

				return false;
			}

			shader.Use();

			return true;
		}

		#endregion

		#region Textures
		
		public bool NewTexture(string name, string fileExtension = ".png")
		{
			if (textures.ContainsKey(name))
			{
				Utils.ThrowWarning(this, $"A texture with name \"{name}\" already exists!");

				return false;
			}

			textures.Add(name, new Texture(name, fileExtension));

			return true;
		}

		public bool GetTexture(string name, out Texture texture)
		{
			if (!textures.TryGetValue(name, out texture))
			{
				Utils.ThrowWarning(this, $"No texture with name \"{name}\" exists!");

				return false;
			}

			return true;
		}

		public bool UseTexture(string name, TextureUnit unit = TextureUnit.Texture0)
		{
			if (!textures.TryGetValue(name, out Texture texture))
			{
				Utils.ThrowWarning(this, $"No texture with name \"{name}\" exists!");

				return false;
			}

			texture.Use(unit);

			return true;
		}

		#endregion

		#region Sprites

		public bool NewSprite(string id, byte priority, string shader, string texture)
		{
			if (string.IsNullOrEmpty(id) || sprites.ContainsKey(id))
			{
				Utils.ThrowWarning(this, $"A sprite with id \"{id}\" already exists!");

				return false;
			}

			Sprite newSprite = new Sprite(id, priority, shader, texture);
			sprites.Add(id, newSprite);
			spriteRenderingOrder.Add(newSprite);

			return true;
		}

		public bool GetSprite(string id, out Sprite sprite)
		{
			if (!sprites.TryGetValue(id, out sprite))
			{
				Utils.ThrowWarning(this, $"No sprite with id \"{id}\" exists!");

				return false;
			}

			return true;
		}

		public void RenderAllSprites()
		{
			// Render all sprites, from low to high priority
			foreach (Sprite sprite in spriteRenderingOrder)
			{
				sprite.Render();
			}
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
				int[] handles = new int[textures.Count];

				foreach (KeyValuePair<string, Texture> texture in textures)
				{
					handles[i] = texture.Value.handle;
					i++;
				}

				GL.DeleteTextures(handles.Length, handles);

				textures = null;

				// Dispose of sprites
				foreach (KeyValuePair<string, Sprite> sprite in sprites)
				{
					sprite.Value.Dispose();
				}

				sprites = null;
				spriteRenderingOrder = null;

				// Dispose of the screen
				screen.Dispose();

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
