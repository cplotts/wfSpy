using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyCpuWindowsFormsApp
{
    public partial class AnyCpuTestForm : Form
    {
        private const int WM_NULL = 0x0000;

        public AnyCpuTestForm()
        {
            InitializeComponent();
            Debug.WriteLine($"{GetType().Name}..ctor leave");
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NULL:
                    Debug.WriteLine($"{GetType().Name}.WndProc: Msg={m.Msg}, WParam={m.WParam}, LParam={m.LParam}");
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
