using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ProjectileTK
{
	internal static class Program
	{
		private static void Main()
		{
			NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
			{
				ClientSize = new Vector2i(640, 640),
				Title = "ProjectileTK Window"
			};

			using (Window window = new(GameWindowSettings.Default, nativeWindowSettings))
			{
				window.Run();
			}
		}
	}
}