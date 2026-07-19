using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using ProjectileTK.Utilities;

namespace ProjectileTK.Rendering
{
	public sealed class RenderingServer
	{
		private RenderingServer()
		{
			shaders  = new Dictionary<string, Shader>();
			textures = new Dictionary<string, Texture>();
		}

		// Lazy singleton implementation (NOT THREAD-SAFE!!!)
		private static readonly Lazy<RenderingServer> instance = new Lazy<RenderingServer>(() => new RenderingServer());
		public static RenderingServer Instance => instance.Value;

		private Dictionary<string, Shader> shaders;
		private Dictionary<string, Texture> textures;

		public void AddShader(string shader)
		{
			if (shaders.ContainsKey(shader))
			{
				Utils.ThrowWarning($"Shader already exists! (name: {shader})");

				return;
			}

			shaders.Add(shader, new Shader(shader));
		}

		public bool TryAddShader(string shader)
		{
			if (shaders.ContainsKey(shader))
			{
				return false;
			}

			shaders.Add(shader, new Shader(shader));

			return true;
		}

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

		public void AddAndUseShader(string shader)
		{
			if (!shaders.ContainsKey(shader))
			{
				shaders.Add(shader, new Shader(shader));
			}

			shaders[shader].Use();
		}

		public void AddTexture(string texture)
		{
			if (textures.ContainsKey(texture))
			{
				Utils.ThrowWarning($"Texture already exists! (name: {texture})");

				return;
			}

			textures.Add(texture, new Texture(texture));
		}

		public bool TryAddTexture(string texture)
		{
			if (textures.ContainsKey(texture))
			{
				return false;
			}

			textures.Add(texture, new Texture(texture));

			return true;
		}

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

		public void AddAndUseTexture(string texture, TextureUnit unit = TextureUnit.Texture0)
		{
			if (!textures.ContainsKey(texture))
			{
				textures.Add(texture, new Texture(texture));
			}

			textures[texture].Use(unit);
		}
	}
}
