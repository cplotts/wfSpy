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
			this.Text = String.Format("Window {0:X8} \"{1}\" {2}", hwnd.ToInt32(), WindowText, className);
			
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
