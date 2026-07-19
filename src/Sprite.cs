using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectileTK.Rendering;

namespace ProjectileTK
{
	public class Sprite : GameObject, IRenderer
	{
		float[] vertices = [
		//	 X       Y      UV(X) UV(Y)
			 0.25f,  0.25f, 1.0f, 1.0f,
			 0.25f, -0.25f, 1.0f, 0.0f,
			-0.25f, -0.25f, 0.0f, 0.0f,
			-0.25f,  0.25f, 0.0f, 1.0f
		];

		uint[] indices = [
			0, 1, 2, // bottom trianlge
			2, 3, 0  // top triangle
		];

		int VBO, VAO, EBO;
	
		string shader, texture;

		private Sprite(string name, Vector2 position, float rotation, string shader = "sprite-default", string texture = "missing.png")
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

		public static Sprite CreateSprite(string name, Vector2 position, float rotation, string shader = "sprite-default", string texture = "missing.png")
		{
			Sprite sprite = new(name, position, rotation, shader, texture);
			sprite.Init();

			return sprite;
		}

		public void SetTexture(string texture)
		{
			this.texture = texture;
			RenderingServer.Instance.TryAddTexture(texture);
		}

		#region Rendering

		// Calculate transformation matrix
		public Matrix4 CalculateTransformationMatrix()
		{
			// We start with the identity matrix, which applies no transformation
			Matrix4 transformation = Matrix4.Identity;

			// First we scale our sprite
			transformation *= Matrix4.CreateScale(transform.scale.X, transform.scale.Y, 1.0f);

			// Then we rotate it
			transformation *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(transform.rotation));

			// Lastly we move it
			transformation *= Matrix4.CreateTranslation(transform.position.X, transform.position.Y, 0.0f);

			return transformation;
		}

		public void Render()
		{
			// Bind vertex array
			GL.BindVertexArray(VAO);

			// Activate shader & pass it our transformation matrix
			// Setting a shader uniform activates the shader, so we don't need to activate it separately
			RenderingServer.Instance.GetShader(shader).SetUniform(0, CalculateTransformationMatrix(), true);

			// Activate texture
			RenderingServer.Instance.UseTexture(texture);

			// Draw elements
			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
		}

		#endregion

		#region  Dispose

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

		#endregion
	}
}
