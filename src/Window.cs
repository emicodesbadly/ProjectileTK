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

		Sprite sprite;

		public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
			: base(gameWindowSettings, nativeWindowSettings)
		{

		}

		// Runs immediately after Run() is called
		protected override void OnLoad()
		{
			base.OnLoad();

			GL.ClearColor(clearColor);

			sprite = new("sprite", Vector2.Zero, 0f);
		}

		// Runs when the window is about to close
		protected override void OnUnload()
		{
			base.OnUnload();
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

			sprite.Render();

			SwapBuffers();
		}
	}
}