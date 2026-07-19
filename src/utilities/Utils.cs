using System;

namespace ProjectileTK.Utilities
{
	public static class Utils
	{
		public static void ThrowWarning(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}
}
