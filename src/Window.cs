using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using ProjectileTK.Rendering;

namespace ProjectileTK
{
	public class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
		: GameWindow(gameWindowSettings, nativeWindowSettings)
	{
        // Runs immediately after Run() is called
        protected override void OnLoad()
		{
			base.OnLoad();

			// Console.WriteLine($"({Title}) Loading window...");

			GL.Disable(EnableCap.DepthTest);

			// CREATE SHADERS HERE
			RenderingServer.Instance.NewShader("screen");
			RenderingServer.Instance.NewShader("sprite-default");

			// CREATE TEXTURES HERE
			RenderingServer.Instance.NewTexture("missing", ".png");
			RenderingServer.Instance.NewTexture("missing-red", ".png");
			RenderingServer.Instance.NewTexture("missing-green", ".png");
			RenderingServer.Instance.NewTexture("missing-blue", ".png");

			// CREATE SPRITES HERE
			RenderingServer.Instance.NewSprite("missing", 255, "sprite-default", "missing");
			RenderingServer.Instance.NewSprite("missing-red", 254, "sprite-default", "missing-red");
			RenderingServer.Instance.NewSprite("missing-green", 253, "sprite-default", "missing-green");
			RenderingServer.Instance.NewSprite("missing-blue", 252, "sprite-default", "missing-blue");

			// Create screen
			RenderingServer.Instance.CreateScreen(this, (1920, 1080));

			// Debug Squares
			int amount = 16;

			int q = -1;
			for (int i = 0; i < amount; i++)
			{
				if (i % 4 == 0) q = i;

				string sp = "missing";

				if		((i - q) % 4 == 0) sp = "missing";
				else if ((i - q) % 4 == 1) sp = "missing-red";
				else if ((i - q) % 4 == 2) sp = "missing-green";
				else if ((i - q) % 4 == 3) sp = "missing-blue";

				SpriteInstance instance = SpriteInstance.CreateSpriteInstance(
					sp,
					0.75f * new Vector2((float)MathHelper.Sin(MathHelper.TwoPi * (float)i / amount), (float)MathHelper.Cos(MathHelper.TwoPi * (float)i / amount)),
					-360f * (float)i / amount);

				instance.transform.scale = (1f - i / (float)(amount + 1), 1f - i / (float)(amount + 1));
			}

			//SpriteInstance.CreateSpriteInstance("missing", (1, 1), 45f);
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

			RenderingServer.Instance.Screen.OnWindowResized((e.Width, e.Height));
		}

		// Runs every frame, BEFORE rendering
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			if (IsKeyPressed(Keys.T))
			{
				Console.WriteLine(e.Time.ToString("#.#####"));
			}

			if (IsKeyPressed(Keys.F))
			{
				if (WindowState == WindowState.Fullscreen)
				{
					WindowState = WindowState.Normal;
				}
				else
				{
					WindowState = WindowState.Fullscreen;
				}
			}
		}

		// Runs when the window is ready to render, AFTER OnUpdateFrame()
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			RenderingServer.Instance.Screen.BindFBO();

			// Render all sprite instances
			RenderingServer.Instance.RenderAllSprites();

			// Render the screen
			RenderingServer.Instance.Screen.Render();

			SwapBuffers();
		}
	}
}
