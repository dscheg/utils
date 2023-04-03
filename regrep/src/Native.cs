using System;
using System.Runtime.InteropServices;

namespace regrep
{
	internal static class Native
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD cursorPosition);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int GetFileType(IntPtr handle);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out ConsoleModes lpMode);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, ConsoleModes dwMode);

		[Flags]
		internal enum ConsoleModes : uint
		{
			ENABLE_PROCESSED_INPUT = 0x0001,
			ENABLE_LINE_INPUT = 0x0002,
			ENABLE_ECHO_INPUT = 0x0004,
			ENABLE_WINDOW_INPUT = 0x0008,
			ENABLE_MOUSE_INPUT = 0x0010,
			ENABLE_INSERT_MODE = 0x0020,
			ENABLE_QUICK_EDIT_MODE = 0x0040,
			ENABLE_EXTENDED_FLAGS = 0x0080,
			ENABLE_AUTO_POSITION = 0x0100,
			ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,

			ENABLE_PROCESSED_OUTPUT = 0x0001,
			ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
			ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
			DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
			ENABLE_LVB_GRID_WORLDWIDE = 0x0010,

			INVALID = ~0u
		}

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

		internal static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

		internal const int STD_INPUT_HANDLE = -10;
		internal const int STD_OUTPUT_HANDLE = -11;
		internal const int STD_ERROR_HANDLE = -12;

		internal const int FILE_TYPE_PIPE = 3;
	}
}