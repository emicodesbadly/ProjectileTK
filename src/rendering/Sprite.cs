using System;
using OpenTK.Graphics.OpenGL4;

namespace ProjectileTK.Rendering
{
    public class Sprite : IRenderer
    {
        public readonly string id;

        readonly float[] vertices = [
		//	 X       Y      UV(X) UV(Y)
			 0.25f,  0.25f, 1.0f, 1.0f,
			 0.25f, -0.25f, 1.0f, 0.0f,
			-0.25f, -0.25f, 0.0f, 0.0f,
			-0.25f,  0.25f, 0.0f, 1.0f
		];

        readonly uint[] indices = [
            0, 1, 2, // bottom triangle
            2, 3, 0  // top triangle
        ];

        public readonly string shader, texture;

        int VBO, VAO, EBO;
        int instVBO;

        public Sprite(string id, string shader = "sprite-default", string texture = "missing.png")
        {
            this.id = id;

            this.shader  = shader;
            this.texture = texture;

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

            // Also create & bind instance vertex buffer
            instVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, instVBO);

            // Set up the transform vertex attribute
            // It is a mat4, so it takes 4 locations
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(2, 1);

            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 4 * sizeof(float));
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribDivisor(3, 1);

            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 8 * sizeof(float));
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribDivisor(4, 1);

            GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 12 * sizeof(float));
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribDivisor(5, 1);

			// Create & bind the element buffer, & upload data to it
			EBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }

        public void Render()
        {
            float[] instanceData = [32];

            GL.BindBuffer(BufferTarget.ArrayBuffer, instVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, instanceData.Length * sizeof(float), instanceData, BufferUsageHint.StreamDraw);

            GL.BindVertexArray(VAO);

            RenderingServer.Instance.UseShader(shader);
            RenderingServer.Instance.UseTexture(texture);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, 2, DrawElementsType.UnsignedInt, 0, 2);
        }

        #region  Dispose

		// IDisposable implementation
		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				

				disposed = true;
			}
		}

		~Sprite()
		{
			if (!disposed)
			{
				//throw new Exception($"(SpriteObject: {name}) GPU Resource leak! Did you forget to call Dispose()?");
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