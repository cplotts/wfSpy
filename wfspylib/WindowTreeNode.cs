using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace wfspy
{
	enum ImageIndices : int
	{
		Window = 0,
		WindowHidden = 1,
		ManagedWindow = 2,
		ManagedWindowHidden = 3
	};
	
	/// <summary>
	/// Summary description for WindowTreeNode.
	/// </summary>
	public class WindowTreeNode : TreeNode
	{
		private IntPtr hwnd;

		private static Regex classNameRegex = new Regex(@"WindowsForms10\..*", RegexOptions.Singleline);	
		
		public WindowTreeNode(IntPtr hwnd)
		{
			this.hwnd = hwnd;
			string className = this.WindowClassName;
			IntPtr hProcess = UnmanagedMethods.GetProcessHandleFromHwnd(hwnd);

			int size = Is64BitProcess(hProcess) ? 64 : 86;
			this.Text = String.Format("Window {0:X8} \"{1}\" {2} (x{3})", hwnd.ToInt32(), WindowText, className, size);
			
			if (IsDotNetWindow(className))
			{
				if (UnmanagedMethods.IsWindowVisible(hwnd))
					this.ImageIndex = (int)ImageIndices.ManagedWindow;
				else
					this.ImageIndex = (int)ImageIndices.ManagedWindowHidden;
			}
			else
			{
				if (UnmanagedMethods.IsWindowVisible(hwnd))
					this.ImageIndex = (int)ImageIndices.Window;
				else
					this.ImageIndex = (int)ImageIndices.WindowHidden;
			}
			
			this.SelectedImageIndex = this.ImageIndex;
		}

		static bool Is64BitProcess(IntPtr hProcess)
		{
			bool flag = false;

			if (Environment.Is64BitOperatingSystem)
			{
				// On 64-bit OS, if a process is not running under Wow64 mode, 
				// the process must be a 64-bit process.
				flag = !(UnmanagedMethods.IsWow64Process(hProcess, out flag) && flag);
			}

			return flag;
		}

		public bool IsManaged
		{
			get
			{
				return (this.ImageIndex == (int)ImageIndices.ManagedWindow) || 
					(this.ImageIndex == (int)ImageIndices.ManagedWindowHidden);
			}
		}

		public IntPtr Hwnd
		{
			get
			{
				return hwnd;
			}
		}

		public string WindowClassName
		{
			get
			{
				return UnmanagedMethods.GetClassName(hwnd);
			}
		}
	
		public string WindowText
		{
			get
			{
				return UnmanagedMethods.GetWindowText(hwnd);
			}
		}

		public static bool IsDotNetWindow(string className)
		{
			Match match = classNameRegex.Match(className);
			return (match.Success);
		}
	}
}
