using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace regrep
{
	internal static class Native
	{
		public static Color ConsoleColorToColorAttribute(ConsoleColor color, bool isBackground)
		{
			if((color & ~ConsoleColor.White) != ConsoleColor.Black)
				throw new ArgumentException();
			var result = (Color)color;
			if(isBackground)
				result = (Color)((int)result << 4);
			return result;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, short attributes);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD cursorPosition);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

		[DllImport("kernel32.dll")]
		internal static extern int GetFileType(SafeFileHandle handle);

		internal struct COORD
		{
			internal short X;
			internal short Y;
		}

		internal struct SMALL_RECT
		{
			internal short Left;
			internal short Top;
			internal short Right;
			internal short Bottom;
		}

		internal struct CONSOLE_SCREEN_BUFFER_INFO
		{
			internal COORD dwSize;
			internal COORD dwCursorPosition;
			internal short wAttributes;
			internal SMALL_RECT srWindow;
			internal COORD dwMaximumWindowSize;
		}

		[Flags]
		[Serializable]
		internal enum Color : short
		{
			Black = 0,
			ForegroundBlue = 1,
			ForegroundGreen = 2,
			ForegroundRed = 4,
			ForegroundYellow = ForegroundRed | ForegroundGreen, // 0x0006
			ForegroundIntensity = 8,
			BackgroundBlue = 16, // 0x0010
			BackgroundGreen = 32, // 0x0020
			BackgroundRed = 64, // 0x0040
			BackgroundYellow = BackgroundRed | BackgroundGreen, // 0x0060
			BackgroundIntensity = 128, // 0x0080
			ForegroundMask = ForegroundIntensity | ForegroundYellow | ForegroundBlue, // 0x000F
			BackgroundMask = BackgroundIntensity | BackgroundYellow | BackgroundBlue, // 0x00F0
			ColorMask = BackgroundMask | ForegroundMask, // 0x00FF
		}

		internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

		internal const int STD_INPUT_HANDLE = -10;
		internal const int STD_OUTPUT_HANDLE = -11;
		internal const int STD_ERROR_HANDLE = -12;

		internal const int FILE_TYPE_PIPE = 3;
	}
}