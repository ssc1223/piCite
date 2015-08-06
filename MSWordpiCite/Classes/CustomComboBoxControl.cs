using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MSWordpiCite.Classes
{
    public class CustomComboBoxControl : ComboBox
    {
        public const int WM_PAINT = 0xF;
        [DllImport("user32")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        public CustomComboBoxControl(): base()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.IntegralHeight = false;
        }

        protected override void WndProc(ref Message m)
        {
            IntPtr hDC = IntPtr.Zero;
            Graphics gdc = null;
            switch (m.Msg)
            {
                case WM_PAINT:
                    base.WndProc(ref m);
                    hDC = GetWindowDC(this.Handle);
                    gdc = Graphics.FromHdc(hDC);
                    PaintFlatControlBorder(this, gdc);
                    ReleaseDC(m.HWnd, hDC);
                    gdc.Dispose();

                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void PaintFlatControlBorder(Control ctrl, Graphics g)
        {
            Rectangle outer = new Rectangle(0, 0, this.Width, this.Height);
            Rectangle inner = new Rectangle(1, 1, this.Width - 2, this.Height - 2);

            ControlPaint.DrawBorder(g, outer, Color.Black, ButtonBorderStyle.Solid);
            ControlPaint.DrawBorder(g, inner, this.BackColor, ButtonBorderStyle.Solid);
        }
    }
}
