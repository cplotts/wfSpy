using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace wfspy
{
	/// <summary>
	/// Summary description for WindowsPropertyView.
	/// </summary>
	public class WindowPropertiesView : System.Windows.Forms.UserControl, IHookInstall 
	{
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		private IntPtr parentWindow;
		private IntPtr spyWindow;

		public WindowPropertiesView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid1.Size = new System.Drawing.Size(280, 384);
			this.propertyGrid1.TabIndex = 0;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// timer1
			// 
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// WindowPropertiesView
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.propertyGrid1});
			this.Name = "WindowPropertiesView";
			this.Size = new System.Drawing.Size(280, 384);
			this.ResumeLayout(false);

		}
		#endregion

		protected override CreateParams CreateParams
		{
			get
			{
				System.Windows.Forms.CreateParams cp = base.CreateParams;
				cp.Parent = parentWindow;
				RECT rc = new RECT();
				UnmanagedMethods.GetClientRect(parentWindow, ref rc);
				
				cp.X = rc.left;
				cp.Y = rc.top;
				cp.Width = rc.right - rc.left;
				cp.Height = rc.bottom - rc.top;
		
				return cp;
			}
		}
		#region IHookInstall Members

		public void OnInstallHook(byte[] data)
		{
			parentWindow = (IntPtr)BitConverter.ToInt32(data, 0);
			spyWindow = (IntPtr)BitConverter.ToInt32(data, 4);
			timer1.Start();
		}

		#endregion

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			try
			{
				timer1.Stop();

				CreateControl();
				
				Control ctl = Control.FromHandle(spyWindow);
				
				if (ctl != null)
					propertyGrid1.SelectedObject = new WindowProperties(ctl);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "wfav");
			}
		}
	}
}
