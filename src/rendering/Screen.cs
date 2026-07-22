using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectileTK.Utilities;

namespace ProjectileTK.Rendering
{
    public class Screen : IRenderer
    {
        float[] vertices = [
             1.0f,  1.0f, 1.0f, 1.0f,
             1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
            -1.0f,  1.0f, 0.0f, 1.0f,
        ];

        uint[] indices = [
            0, 1, 2,
            2, 3, 0
        ];

        private readonly int FBO;
		private readonly int renderTexture;


        private readonly int VBO, VAO, EBO;

        private readonly Window window;

        private float aspectRatio;
        public float AspectRatio => aspectRatio;

        public Screen(Window window, (int width, int height) resolution)
        {
            // Set window & screen aspect ratio
            this.window = window;
            aspectRatio = (float)resolution.width / (float)resolution.height;

            // Create & bind frame buffer
			FBO = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

			// Create the texture we'll render to & set its parameters
			renderTexture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, renderTexture);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, resolution.width, resolution.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

			// Attach the render texture
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, FBO, 0);

			// Check for frame buffer errors
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Utils.ThrowError(this, "Frame buffer is incomplete!");
			}

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

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            OnWindowResized((window.ClientSize.X, window.ClientSize.Y));
        }
        
        public void OnWindowResized((int width, int height) windowResolution)
        {
            float windowAspect = (float)windowResolution.width / (float)windowResolution.height;

            if (windowAspect > aspectRatio)
            {
                vertices[0]  =  (1 / windowAspect) * aspectRatio;
                vertices[1]  =  1.0f;

                vertices[4]  =  (1 / windowAspect) * aspectRatio;
                vertices[5]  = -1.0f;

                vertices[8]  = -(1 / windowAspect) * aspectRatio;
                vertices[9]  = -1.0f;

                vertices[12] = -(1 / windowAspect) * aspectRatio;
                vertices[13] =  1.0f;

            }
            else if (windowAspect < aspectRatio)
            {
                vertices[0]  =  1.0f; 
                vertices[1]  =  1 / aspectRatio * windowAspect;

                vertices[4]  =  1.0f;
                vertices[5]  = -1 / aspectRatio * windowAspect;

                vertices[8]  = -1.0f;
                vertices[9]  = -1 / aspectRatio * windowAspect;

                vertices[12] = -1.0f;
                vertices[13] =  1 / aspectRatio * windowAspect;
            }
            else
            {
                vertices[0]  =  1.0f; 
                vertices[1]  =  1.0f;

                vertices[4]  =  1.0f;
                vertices[5]  = -1.0f;

                vertices[8]  = -1.0f;
                vertices[9]  = -1.0f;

                vertices[12] = -1.0f;
                vertices[13] =  1.0f;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
        }

        public void BindFBO()
        {
            // Bind FBO
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

            GL.ClearColor(Color4.HotPink);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Render()
        {
            // Bind default frame buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.ClearColor(Color4.Green);
			GL.Clear(ClearBufferMask.ColorBufferBit);

            // Bind VAO
            GL.BindVertexArray(VAO);

            // Activate shader & texture
            RenderingServer.Instance.UseShader("screen");
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, renderTexture);

            // Draw screen
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
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
                GL.DeleteBuffers(2, [VBO, EBO]);

				// Unbind & delete vertex array
                GL.BindVertexArray(0);
                GL.DeleteVertexArray(VAO);

                // Dispose of render texture
				GL.DeleteTexture(renderTexture);

				// Dispose of frame buffer
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
				GL.DeleteFramebuffer(FBO);

				disposed = true;
			}
		}

		~Screen()
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
}