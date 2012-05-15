using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using System.ComponentModel;

namespace wfspy
{
	/// <summary>
	/// Summary description for FormSizeSaver.
	/// </summary>
	public class FormSizeSaver
	{
		public FormSizeSaver(Form form)
		{
			form.Load += new EventHandler(this.OnFormLoad);
			form.Closing += new CancelEventHandler(this.OnFormUnload);
		}
	
		void OnFormLoad(object sender, EventArgs e)
		{
			Form form = (Form)sender;
			RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\CodeProject\\wfspy\\" + form.GetType().FullName);
			
			Rectangle bounds = form.Bounds;

			int x = (int)key.GetValue("X", bounds.X);
			int y = (int)key.GetValue("Y", bounds.Y);
			int width = (int)key.GetValue("Width", bounds.Width);
			int height = (int)key.GetValue("Height", bounds.Height);
			
			bounds = new Rectangle(x, y, width, height);
			form.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		void OnFormUnload(object sender, CancelEventArgs e)
		{
			Form form = (Form)sender;
			Rectangle bounds = form.Bounds;
			
			RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\CodeProject\\wfspy\\" + form.GetType().FullName);
			
			key.SetValue("X", bounds.X);
			key.SetValue("Y", bounds.Y);
			key.SetValue("Width", bounds.Width);
			key.SetValue("Height", bounds.Height);

			form.Load -= new EventHandler(this.OnFormLoad);
			form.Closing -= new CancelEventHandler(this.OnFormUnload);
		}
	}
}
