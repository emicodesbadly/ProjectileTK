using System;

namespace ProjectileTK.Utilities
{
	public static class Utils
	{
		public static void ThrowWarning(object sender, string message)
		{
			Console.Write($"({sender}) ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public static void ThrowWarning(string sender, string message)
		{
			Console.Write($"({sender}) ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}
}
