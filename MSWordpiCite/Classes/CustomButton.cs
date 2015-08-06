using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

namespace MSWordpiCite.Classes
{
    public class CustomButton : Button
    {
        private bool ShowBorder { get; set; }

        public CustomButton() : base()
        {
            // Prevent the button from drawing its own border
            FlatAppearance.BorderSize = 0;

            // Set up a blue border and back colors for the button
            FlatAppearance.BorderColor = Color.FromArgb(115, 170, 115);
            FlatAppearance.CheckedBackColor = Color.FromArgb(188, 216, 188);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(188, 216, 188);
            FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 225, 205);
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            // Set the size for the button to be the same as a ToolStripButton
            Size = new System.Drawing.Size(24, 24);
        }

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            // Show the border when you hover over the button
            ShowBorder = true;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Hide the border when you leave the button
            ShowBorder = false;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            int radius = 2;
            base.OnPaint(e);

            // The DesignMode check here causes the border to always draw in the Designer
            // This makes it easier to place your button
            if (DesignMode || ShowBorder)
            {
                Pen pen = new Pen(FlatAppearance.BorderColor, 1);
                GraphicsPath gp = new GraphicsPath();
                gp.AddLine(radius, 0, Size.Width - 1 - (radius * 2), 0); // Line
                gp.AddArc(Size.Width - 1 - (radius * 2), 0, radius * 2, radius * 2, 270, 90); // Corner
                gp.AddLine(Size.Width - 1, radius, Size.Width - 1, Size.Height - 1 - (radius * 2)); // Line
                gp.AddArc(Size.Width - 1 - (radius * 2), Size.Height - 1 - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
                gp.AddLine(radius, Size.Height - 1, Size.Width - 1 - (radius * 2), Size.Height - 1); // Line
                gp.AddArc(0, Size.Height - 1 - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
                gp.AddLine(0, Size.Height - 1 - (radius * 2), 0, radius); // Line
                gp.AddArc(0, 0, radius * 2, radius * 2, 180, 90); // Corner
                gp.CloseFigure();
                e.Graphics.DrawPath(pen, gp);
                gp.Dispose();                
            }
        }

        // Prevent Text from being set on the button (since it will be an icon)
        [Browsable(false)]
        public override string Text { get { return ""; } set { base.Text = ""; } }

        [Browsable(false)]
        public override ContentAlignment TextAlign { get { return base.TextAlign; } set { base.TextAlign = value; } }
    }

}
