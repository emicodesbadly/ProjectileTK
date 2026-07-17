using System;

namespace PTK
{
	internal static class Program
	{
		private static void Main()
		{
			using (Window window = new(640, 360, "PTK WIndow"))
			{
				window.Run();
			}
		}
	}
}
