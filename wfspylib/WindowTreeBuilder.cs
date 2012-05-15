using System;
using System.Collections;

namespace wfspy
{
	/// <summary>
	/// Summary description for WindowTreeBuilder.
	/// </summary>
	public class WindowTreeBuilder
	{
		Hashtable hwndNodeMap = new Hashtable();		
		WindowTreeNode rootNode = new WindowTreeNode(UnmanagedMethods.GetDesktopWindow());

		public WindowTreeBuilder()
		{
		}

		public WindowTreeNode RootNode
		{
			get
			{
				return rootNode;
			}
		}
		
		public void RemoveNode(IntPtr hwnd)
		{
			WindowTreeNode node =  (WindowTreeNode)hwndNodeMap[hwnd];
			
			if (node != null)
			{
				hwndNodeMap.Remove(hwnd);
				node.Remove();
			}
		}

		public bool HasManagedChild(WindowTreeNode parentNode)
		{
			bool ret = false;
			
			foreach(WindowTreeNode node in parentNode.Nodes)
			{
				if (node.IsManaged || HasManagedChild(node))
				{
					ret = true;
					break;
				}
			}

			return ret;
		}

		public void FilterUnmanagedWindows(WindowTreeNode parentNode)
		{
			for(int i = 0; i < parentNode.Nodes.Count; i++)
			{
				WindowTreeNode node = (WindowTreeNode)parentNode.Nodes[i];
				
				if (!node.IsManaged && !HasManagedChild(node))
				{
					parentNode.Nodes.RemoveAt(i);
					i--;
				}
				else
				{
					FilterUnmanagedWindows(node);
				}
			}
		}

		public void FilterUnmanagedWindows()
		{
			FilterUnmanagedWindows(rootNode);
		}

		public WindowTreeNode AddWindow(IntPtr hwnd)
		{
			WindowTreeNode node;

			if (!hwndNodeMap.ContainsKey(hwnd))
			{
				node = new WindowTreeNode(hwnd);
				IntPtr hwndParent = UnmanagedMethods.GetParent(hwnd);
				
				if (hwndParent == IntPtr.Zero)
				{
					rootNode.Nodes.Add(node);
				}
				else
				{
					WindowTreeNode parentNode = AddWindow(hwndParent);
					parentNode.Nodes.Add(node);
				}

				hwndNodeMap[hwnd] = node;
			}
			else
			{
				node = (WindowTreeNode)hwndNodeMap[hwnd];
			}
			
			return node;
		}
		
		private int OnEnumWindow(IntPtr hwnd, IntPtr lParam)
		{
			AddWindow(hwnd);
			return 1;
		}

		private int OnEnumThreadWindow(IntPtr hwnd, IntPtr lParam)
		{
			AddWindow(hwnd);
			UnmanagedMethods.EnumChildWindows(hwnd, new WindowEnumProc(this.OnEnumWindow), IntPtr.Zero);
			return 1;
		}

		public void BuildAllWindowsTree()
		{
			UnmanagedMethods.EnumChildWindows(rootNode.Hwnd, new WindowEnumProc(this.OnEnumWindow), IntPtr.Zero);
		}

		public void BuildThreadWindowsTree(int threadId)
		{
			UnmanagedMethods.EnumThreadWindows(threadId, new WindowEnumProc(this.OnEnumThreadWindow), IntPtr.Zero);
		}

	}
}
