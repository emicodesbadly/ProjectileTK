using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using ProjectileTK.Rendering;

namespace ProjectileTK
{
	public class Window : GameWindow
	{
		public static Color4 clearColor = new(0.0f, 0.0f, 1.0f, 1.0f);	// Window background color

		Sprite sprite1, sprite2;

		public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
			: base(gameWindowSettings, nativeWindowSettings)
		{

		}

		// Runs immediately after Run() is called
		protected override void OnLoad()
		{
			base.OnLoad();

			// Console.WriteLine($"({Title}) Loading window...");

			GL.ClearColor(clearColor);

			sprite1 = new("sprite 1", ( 0.5f, 0.5f), 45f);
			sprite1.transform.scale = (1.0f, 1.5f);

			sprite2 = new("sprite 2", (-0.5f, 0.0f),  0f);
			sprite2.transform.scale = (1.0f, 1.0f);
		}

		// Runs when the window is about to close
		protected override void OnUnload()
		{
			base.OnUnload();

			RenderingServer.Instance.Dispose();
		}

		// Runs when the window is resized
		protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
		{
			base.OnFramebufferResize(e);

			GL.Viewport(0, 0, e.Width, e.Height);
		}

		// Runs every frame, BEFORE rendering
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
		}

		// Runs when the window is ready to render, AFTER OnUpdateFrame()
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit);

			sprite1.Render();
			sprite2.Render();

			SwapBuffers();
		}
	}
}