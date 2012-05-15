using System;
using System.Runtime.InteropServices;
using System.Text;

namespace wfspy
{
	public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
	
	public enum GetWindowCmd : int
	{
		GW_HWNDFIRST = 0,
		GW_HWNDLAST,
		GW_HWNDNEXT,
		GW_HWNDPREV,
		GW_OWNER,
		GW_CHILD    
	};
	
	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	};

	public class UnmanagedMethods
	{
		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
	
		[DllImport("user32.dll")]
		public static extern bool EnumThreadWindows(int threadId, WindowEnumProc func, IntPtr lParam);
		
		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr hwnd);
	
		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hwnd, StringBuilder buffer, int bufferLen);
	
		[DllImport("user32.dll")]
		public static extern int GetClassName(IntPtr hwnd, StringBuilder buffer, int bufferLen);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hwnd);
		
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr hwnd);
		
		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int processID);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32.dll")]
		public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindow(IntPtr hwnd, int cmd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hwnd, ref RECT rc);

		[DllImport("user32.dll")]
		public static extern IntPtr GetClientRect(IntPtr hwnd, ref RECT rc);

		public static string GetWindowText(IntPtr hwnd)
		{
			int bufLen = GetWindowTextLength(hwnd) + 1;

			StringBuilder buffer = new StringBuilder(bufLen);
			GetWindowText(hwnd, buffer, bufLen);
			
			return buffer.ToString();
		}
	
		public static string GetClassName(IntPtr hwnd)
		{
			StringBuilder buffer = new StringBuilder(256);
			GetClassName(hwnd, buffer, 256);
			
			return buffer.ToString();
		}
	}
}
