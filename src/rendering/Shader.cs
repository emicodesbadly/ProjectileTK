using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectileTK.Utilities;

namespace ProjectileTK.Rendering
{
	public class Shader : IDisposable
	{
		public readonly int handle;
        readonly bool valid = true;

		public readonly string name;

		public Shader(string name)
		{
			this.name = name;

			int vert, frag;

			// Read shader source code
			string vertSource = File.ReadAllText("resources/shaders/" + name + ".vert");
			string fragSource = File.ReadAllText("resources/shaders/" + name + ".frag");

			// Create vertex shader
			vert = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vert, vertSource);

			// Create fragment shader
			frag = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(frag, fragSource);

            // Deposit compilation success status here

            // Compile vertex shader & check for errors
            GL.CompileShader(vert);

            GL.GetShader(vert, ShaderParameter.CompileStatus, out int success);
			if (success == 0)
			{
				valid = false;

				string infoLog = GL.GetShaderInfoLog(vert);
				Console.WriteLine(infoLog);
			}

			// Compile fragment shader & check for errors
			GL.CompileShader(frag);

			GL.GetShader(frag, ShaderParameter.CompileStatus, out success);
			if (success == 0)
			{
				valid = false;

				string infoLog = GL.GetShaderInfoLog(frag);
				Console.WriteLine(infoLog);
			}

			// Create GPU program & attach our shaders
			handle = GL.CreateProgram();

			GL.AttachShader(handle, vert);
			GL.AttachShader(handle, frag);

			// Link program & check for errors
			GL.LinkProgram(handle);

			GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out success);
			if (success == 0)
			{
				string infoLog = GL.GetProgramInfoLog(handle);
				Console.WriteLine(infoLog);
			}

			// Detach & delete our shaders, since we no longer need them
			GL.DetachShader(handle, vert);
			GL.DetachShader(handle, frag);
			GL.DeleteShader(vert);
			GL.DeleteShader(frag);
		}

		// Call to activate this shader for rendering
		public void Use()
		{
			if (valid)
			{
				GL.UseProgram(handle);
			}
			else
			{
				Utils.ThrowWarning(this, $"Shader exists, but is invalid! ({name})");
			}
		}

		// Set shader uniform of type Matrix4
		// OpenTK uses row-major, while GLSL uses column-major, so usually we must transpose the matrix to correct for this
		public void SetMatrix4(int location, Matrix4 matrix, bool transpose = true)
		{
			GL.UseProgram(handle);
			GL.UniformMatrix4(location, transpose, ref matrix);
		}

		#region Dispose

		// IDisposable implementation
		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				GL.DeleteProgram(handle);

				disposed = true;
			}
		}

		~Shader()
		{
			if (!disposed)
			{
				Utils.ThrowError(this, "GPU Resource leak! Did you forget to call Dispose()?");
			}
		}

		// MUST be called when the shader is no longer needed!
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
