using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace wfspy
{
	/// <summary>
	/// Summary description for WindowPropertiesForm.
	/// </summary>
	public class WindowPropertiesForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel propertyViewPanel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;

		
		private IntPtr targetWindowHandle;

		public WindowPropertiesForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}
		
		//The target window whose properties need to be accessed
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr TargetWindowHandle
		{
			get
			{
				return targetWindowHandle;
			}
			set
			{
				targetWindowHandle = value;
			}
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.okButton = new System.Windows.Forms.Button();
			this.propertyViewPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.okButton.Location = new System.Drawing.Point(104, 416);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 3;
			this.okButton.Text = "Close";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// propertyViewPanel
			// 
			this.propertyViewPanel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.propertyViewPanel.Location = new System.Drawing.Point(6, 24);
			this.propertyViewPanel.Name = "propertyViewPanel";
			this.propertyViewPanel.Size = new System.Drawing.Size(280, 384);
			this.propertyViewPanel.TabIndex = 2;
			this.propertyViewPanel.Resize += new System.EventHandler(this.propertyViewPanel_Resize);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "&Properties :-";
			// 
			// WindowPropertiesForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 442);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label1,
																		  this.okButton,
																		  this.propertyViewPanel});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WindowPropertiesForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Window: {0:X8}";
			this.ResumeLayout(false);

		}
		#endregion


	
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
		
			try
			{
				this.Text = String.Format(this.Text, targetWindowHandle.ToInt32());
				int processId;
				int threadId = UnmanagedMethods.GetWindowThreadProcessId(targetWindowHandle, out processId);
				
				Type type = typeof(WindowPropertiesView);
				
				int panelHandle = propertyViewPanel.Handle.ToInt32();
				int targetHandle = targetWindowHandle.ToInt32();

				byte[] b1 = BitConverter.GetBytes(panelHandle);
				byte[] b2 = BitConverter.GetBytes(targetHandle);

				byte[] data = new byte[b1.Length + b2.Length];
				Array.Copy(b1,data, b1.Length);
				Array.Copy(b2, 0, data, b1.Length, b2.Length);

				HookHelper.InstallIdleHandler(processId, threadId, type.Assembly.Location, type.FullName, data);
				UnmanagedMethods.SendMessage(targetWindowHandle, 0, IntPtr.Zero, IntPtr.Zero);			
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "wfspy");
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void propertyViewPanel_Resize(object sender, System.EventArgs e)
		{
			IntPtr hwnd = UnmanagedMethods.GetWindow(propertyViewPanel.Handle, (int)GetWindowCmd.GW_CHILD);
			UnmanagedMethods.MoveWindow(hwnd, 0, 0, propertyViewPanel.Width, propertyViewPanel.Height, true);
		}
	}
}
