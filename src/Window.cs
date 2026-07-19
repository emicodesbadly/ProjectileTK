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
		public static Color4 clearColor = new(0.0f, 0.0f, 1.0f, 1.0f);

		Sprite sprite;

		public Window(int width, int height, string title)
			: base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) {}

		protected override void OnLoad()
		{
			base.OnLoad();

			GL.ClearColor(clearColor);

			sprite = new("sprite", Vector2.Zero, 0f);
		}

		protected override void OnUnload()
		{
			base.OnUnload();
		}

		protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
		{
			base.OnFramebufferResize(e);

			GL.Viewport(0, 0, e.Width, e.Height);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit);

			sprite.Render();

			SwapBuffers();
		}
	}
}