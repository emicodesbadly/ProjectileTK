using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PTK
{
	public class Window : GameWindow
	{
		public Window(int width, int height, string title)
			: base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) {}

		protected override void OnLoad()
		{
			base.OnLoad();
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
		}
	}
}
