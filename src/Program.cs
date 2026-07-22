using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace ProjectileTK
{
	internal static class Program
	{
		private static void Main()
		{
			GameWindowSettings gameWindowSettings = new GameWindowSettings()
			{
				UpdateFrequency = 60d,
			};

			NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
			{
				Title = "ProjectileTK Window",
				ClientSize = new Vector2i(640, 640),
				//AspectRatio = (1, 1),
			};

            using Window window = new(gameWindowSettings, nativeWindowSettings);
            window.Run();
        }
	}
}
