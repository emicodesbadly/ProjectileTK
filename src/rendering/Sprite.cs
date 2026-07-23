using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectileTK.Utilities;

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

        // Sprites with low priority are rendered behind sprites with high priority
        public readonly byte priority;

        int VBO, VAO, EBO;
        int instVBO;

        public Sprite(string id, byte priority, string shader = "sprite-default", string texture = "missing")
        {
            this.id = id;

            this.shader  = shader;
            this.texture = texture;

            this.priority = priority;

            // Create & bind vertex buffer, & upload data to it
			VBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

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

            // Column 0
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(2, 1);

            // Column 1
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 4 * sizeof(float));
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribDivisor(3, 1);

            // Column 2
            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 8 * sizeof(float));
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribDivisor(4, 1);

            // Column 3
            GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 12 * sizeof(float));
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribDivisor(5, 1);

			// Create & bind the element buffer, & upload data to it
			EBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Unbind buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Render()
        {
            // If this sprite has not been instantiated, skip
            if (!SpriteInstance.allInstances.TryGetValue(id, out List<SpriteInstance> instances) || instances.Count <= 0)
            {
                return;
            }

            // Gather instance data
            float[] instanceData = new float[16 * instances.Count];
            for (int i = 0; i < instances.Count; i++)
            {
                // Get instance transformation matrix
                Matrix4 transform = instances[i].CalculateTransformationMatrix();

                // Column 0
                instanceData[16 * i + 0] = transform.Column0.X;
                instanceData[16 * i + 1] = transform.Column0.Y;
                instanceData[16 * i + 2] = transform.Column0.Z;
                instanceData[16 * i + 3] = transform.Column0.W;

                // Column 1
                instanceData[16 * i + 4] = transform.Column1.X;
                instanceData[16 * i + 5] = transform.Column1.Y;
                instanceData[16 * i + 6] = transform.Column1.Z;
                instanceData[16 * i + 7] = transform.Column1.W;

                // Column 2
                instanceData[16 * i +  8] = transform.Column2.X;
                instanceData[16 * i +  9] = transform.Column2.Y;
                instanceData[16 * i + 10] = transform.Column2.Z;
                instanceData[16 * i + 11] = transform.Column2.W;

                // Column 3
                instanceData[16 * i + 12] = transform.Column3.X;
                instanceData[16 * i + 13] = transform.Column3.Y;
                instanceData[16 * i + 14] = transform.Column3.Z;
                instanceData[16 * i + 15] = transform.Column3.W;
            }

            // Bind instance data buffer & upload instance data to it
            GL.BindBuffer(BufferTarget.ArrayBuffer, instVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, instanceData.Length * sizeof(float), instanceData, BufferUsageHint.StreamDraw);

            // Bind VAO
            GL.BindVertexArray(VAO);

            // Activate shader & texture
            RenderingServer.Instance.UseShader(shader);
            RenderingServer.Instance.UseTexture(texture);

            // Draw sprite instances
            GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0, instances.Count);

            // Unbind buffers & arrays
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        #region  Dispose

		// IDisposable implementation
		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
                // Unbind & delete VBO, instance VBO & EBO
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DeleteBuffers(3, [VBO, instVBO, EBO]);

				// Unbind & delete vertex array
                GL.BindVertexArray(0);
                GL.DeleteVertexArray(VAO);

				disposed = true;
			}
		}

		~Sprite()
		{
			if (!disposed)
			{
				Utils.ThrowError(this, "GPU Resource leak! Did you forget to call Dispose()?");
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

    public class SpritePriorityComparer : Comparer<Sprite>
    {
        public override int Compare(Sprite x, Sprite y)
        {
            if (x == null && y != null)
            {
                return -1;
            }
            else if (x != null && y == null)
            {
                return 1;
            }
            else if (x == null && y == null)
            {
                return 0;
            }

            return x.priority.CompareTo(y.priority);
        }
    }
}
