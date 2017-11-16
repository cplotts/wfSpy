using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace wfspy
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button details;
		private System.Windows.Forms.Button close;
		private System.Windows.Forms.TreeView windowTree;
		private System.Windows.Forms.Button refresh;
		private System.Windows.Forms.ImageList theImageList;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.ComponentModel.IContainer components;
		private WindowTreeBuilder windowTreeBuilder;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			FillTree();
			if (IntPtr.Size == 8)
				Text += " x64";
			else
				Text += " x86";
		}
		
		private void FillTree()
		{
			this.windowTree.Nodes.Clear();

			windowTreeBuilder = new WindowTreeBuilder(windowTreeBuilder);
			windowTreeBuilder.BuildAllWindowsTree();

			if (this.IsHandleCreated)
				windowTreeBuilder.RemoveNode(this.Handle);

			windowTreeBuilder.FilterUnmanagedWindows();
			
			this.windowTree.Nodes.Add(windowTreeBuilder.RootNode);
			windowTreeBuilder.Expand();
		}

		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.label1 = new System.Windows.Forms.Label();
			this.windowTree = new System.Windows.Forms.TreeView();
			this.theImageList = new System.Windows.Forms.ImageList(this.components);
			this.details = new System.Windows.Forms.Button();
			this.close = new System.Windows.Forms.Button();
			this.refresh = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(288, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Current .&NET Windows Forms/Controls in the System:";
			// 
			// windowTree
			// 
			this.windowTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.windowTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.windowTree.HideSelection = false;
			this.windowTree.ImageList = this.theImageList;
			this.windowTree.Location = new System.Drawing.Point(8, 32);
			this.windowTree.Name = "windowTree";
			this.windowTree.Size = new System.Drawing.Size(344, 304);
			this.windowTree.TabIndex = 1;
			// 
			// theImageList
			// 
			this.theImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.theImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("theImageList.ImageStream")));
			this.theImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// details
			// 
			this.details.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.details.Location = new System.Drawing.Point(400, 64);
			this.details.Name = "details";
			this.details.TabIndex = 3;
			this.details.Text = "Details";
			this.details.Click += new System.EventHandler(this.details_Click);
			// 
			// close
			// 
			this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.close.Location = new System.Drawing.Point(400, 96);
			this.close.Name = "close";
			this.close.TabIndex = 4;
			this.close.Text = "Close";
			this.close.Click += new System.EventHandler(this.close_Click);
			// 
			// refresh
			// 
			this.refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.refresh.Location = new System.Drawing.Point(400, 32);
			this.refresh.Name = "refresh";
			this.refresh.TabIndex = 2;
			this.refresh.Text = "&Refresh";
			this.refresh.Click += new System.EventHandler(this.refresh_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(368, 184);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(144, 152);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Legend";
			// 
			// label5
			// 
			this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
			this.label5.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.label5.Location = new System.Drawing.Point(8, 102);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(128, 32);
			this.label5.TabIndex = 3;
			this.label5.Text = "Managed Window (Hidden)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
			this.label4.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.label4.Location = new System.Drawing.Point(8, 76);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 24);
			this.label4.TabIndex = 2;
			this.label4.Text = "Managed Window";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
			this.label3.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.label3.Location = new System.Drawing.Point(8, 50);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 24);
			this.label3.TabIndex = 1;
			this.label3.Text = "Window (Hidden)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.label2.Location = new System.Drawing.Point(8, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 24);
			this.label2.TabIndex = 0;
			this.label2.Text = "Window";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(424, 136);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
			// 
			// MainForm
			// 
			this.AcceptButton = this.details;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.close;
			this.ClientSize = new System.Drawing.Size(520, 346);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.refresh);
			this.Controls.Add(this.close);
			this.Controls.Add(this.details);
			this.Controls.Add(this.windowTree);
			this.Controls.Add(this.label1);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Windows Forms Spy";
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			MainForm form = new MainForm();
			FormSizeSaver saver = new FormSizeSaver(form);
			Application.Run(form);
		}

		private void close_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void refresh_Click(object sender, System.EventArgs e)
		{
			FillTree();
		}

		private void details_Click(object sender, System.EventArgs e)
		{
			WindowTreeNode node = (WindowTreeNode)windowTree.SelectedNode;
			WindowPropertiesForm propForm = new WindowPropertiesForm();
			propForm.TargetWindowHandle = node.Hwnd;
			
			FormSizeSaver saver = new FormSizeSaver(propForm);
			propForm.ShowDialog();
		}


		private bool m_HasCapture = false;
		private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!m_HasCapture)
			{
				SetCapture(this.Handle);
				m_HasCapture = true;
				Cursor.Current = Cursors.Cross;
			}
		}

		[DllImport("user32.dll")]
		static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		static extern IntPtr WindowFromPoint(Point pt);

		private void pictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_HasCapture)
			{
				ReleaseCapture();
				m_HasCapture= false;
			}
		}

		private void MainForm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.m_HasCapture)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}", Control.MousePosition.X, Control.MousePosition.Y));
				Point point1 = new Point(Control.MousePosition.X, Control.MousePosition.Y);
				IntPtr ptr1 = MainForm.WindowFromPoint(point1);
				if (((ptr1 != IntPtr.Zero) && (Control.FromChildHandle(ptr1) == null)) && MainForm.IsManagedHandle(ptr1))
				{
					this.SetFindhWnd(ptr1);
				}
				else
				{
					this.SetFindhWnd(IntPtr.Zero);
				}
			}
		}

		private TreeNode FindTreeNode(TreeNode treeNode, string text)
		{
			if (treeNode.Text.IndexOf(text) != -1)
				return treeNode;
			else if (treeNode.Nodes.Count == 0)
				return null;
			else
			{
				TreeNodeCollection childTreeNodeCollection = treeNode.Nodes;
				foreach (TreeNode childTreeNode in childTreeNodeCollection)
				{
					TreeNode foundTreeNode = FindTreeNode(childTreeNode, text);
					if (foundTreeNode != null)
						return foundTreeNode;
				}
				return null;
			}
		}

		private void MainForm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_HasCapture)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}", Control.MousePosition.X, Control.MousePosition.Y));			
				ReleaseCapture();
				m_HasCapture= false;

				if (this.m_FindhWnd != IntPtr.Zero)
				{
					using (WindowPropertiesForm form1 = new WindowPropertiesForm())
					{
						form1.TargetWindowHandle = this.m_FindhWnd;

						string treeNodeText = string.Format("Window {0:X8}", this.m_FindhWnd.ToInt32());
						TreeNode selectedTreeNode = FindTreeNode(this.windowTree.Nodes[0], treeNodeText);
						if (selectedTreeNode != null)
						{
							selectedTreeNode.EnsureVisible();
							this.windowTree.SelectedNode = selectedTreeNode;
						}

						this.RemoveFindRect();
						new FormSizeSaver(form1);

						form1.ShowDialog();
					}
				}
			}
		}
		private IntPtr m_FindhWnd;
		private void SetFindhWnd(IntPtr hWnd)
		{
			if (this.m_FindhWnd != hWnd)
			{
				this.RemoveFindRect();
				this.m_FindhWnd = hWnd;
				if (this.m_FindhWnd != IntPtr.Zero)
				{
					Rectangle rectangle1 = new Rectangle();
					if (MainForm.GetClientRect(this.m_FindhWnd, ref rectangle1))
					{
						using (Graphics graphics1 = Graphics.FromHwnd(this.m_FindhWnd))
						{
							graphics1.DrawRectangle(new Pen(Color.Black, 4f), rectangle1);
						}
					}
				}
			}
		}
		private static bool IsManagedHandle(IntPtr hWnd)
		{
			bool flag1 = false;
			StringBuilder builder1 = new StringBuilder(0x100);
			if (MainForm.GetClassName(hWnd, builder1, 0x100) != 0)
			{
				flag1 = builder1.ToString().StartsWith("WindowsForms");
			}
			return flag1;
		}
		private void RemoveFindRect()
		{
			if (this.m_FindhWnd != IntPtr.Zero)
			{
				MainForm.InvalidateRect(this.m_FindhWnd, IntPtr.Zero, true);
				MainForm.UpdateWindow(this.m_FindhWnd);
				this.m_FindhWnd = IntPtr.Zero;
			}
		}
		[DllImport("user32.dll")]
		private static extern bool GetClientRect(IntPtr hWnd, ref Rectangle lpRect);
		[DllImport("user32.dll")]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		[DllImport("user32.dll")]
		private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
		[DllImport("user32.dll")]
		private static extern bool UpdateWindow(IntPtr hWnd);
	}
}
