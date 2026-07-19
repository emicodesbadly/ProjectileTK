using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectileTK.Rendering;

namespace ProjectileTK
{
	public class Sprite : GameObject, IRenderer
	{
		float[] vertices = {
		//	 X       Y      UV(X) UV(Y)
			 0.25f,  0.25f, 1.0f, 1.0f,
			 0.25f, -0.25f, 1.0f, 0.0f,
			-0.25f, -0.25f, 0.0f, 0.0f,
			-0.25f,  0.25f, 0.0f, 1.0f
		};

		uint[] indices = {
			0, 1, 2, // bottom trianlge
			2, 3, 0  // top triangle
		};

		int VBO, VAO, EBO;
	
		string shader, texture;

		public Sprite(string name, Vector2 position, float rotation, string shader = "shader", string texture = "missing.png")
			: base(name, position, rotation)
		{
			// Create & bind vertex buffer, & upload data to it
			VBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

			// Create & bind vertex array, & set vertex attributes
			VAO = GL.GenVertexArray();
			GL.BindVertexArray(VAO);

			// Vertex positions
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);

			// Vertex UVs
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
			GL.EnableVertexAttribArray(1);

			// Create & bind the element buffer, & upload data to it
			EBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

			// Add shader to rendering server if it doesn't exist already
			this.shader = shader;
			RenderingServer.Instance.TryAddShader(shader);

			// Add texture to rendering server if it doesn't exist already
			this.texture = texture;
			RenderingServer.Instance.TryAddTexture(texture);
		}

		public void SetTexture(string texture)
		{
			this.texture = texture;
			RenderingServer.Instance.TryAddTexture(texture);
		}

		public void Render()
		{
			// Bind vertex array
			GL.BindVertexArray(VAO);

			// Activate shader & texture
			RenderingServer.Instance.UseTexture(texture);
			RenderingServer.Instance.UseShader(shader);

			// Draw elements
			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
		}

		// IDisposable implementation
		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				// Delete vertex buffer
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.DeleteBuffer(VBO);

				// TODO: free VAO, EBO

				disposed = true;
			}
		}

		~Sprite()
		{
			if (!disposed)
			{
				throw new Exception($"(SpriteObject: {name}) GPU Resource leak! Did you forget to call Dispose()?");
			}
		}

		// MUST be called when the sprite is no longer needed!
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
