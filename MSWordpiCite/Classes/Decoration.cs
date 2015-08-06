using System;
using System.Windows.Forms;
using System.Drawing;
using BrightIdeasSoftware;

namespace MSWordpiCite.Classes
{
    // ObjectListView decorations
    public delegate void EndDragDelegate();
    public class DragSource : BrightIdeasSoftware.AbstractDragSource
    {
        public event EndDragDelegate endDragEvent;

        public override object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item)
        {
            return "";
        }

        public override void EndDrag(object dragObject, DragDropEffects effect)
        {
            endDragEvent();
        }

        public override DragDropEffects GetAllowedEffects(object data)
        {
            return DragDropEffects.Copy;
        }
    }
    public class ItemDecoration : BrightIdeasSoftware.AbstractDecoration
    {
        public string Title;
        public string Year;
        public string Authors;
        public Font TitleFont = new Font("Trebuchet MS", 9, FontStyle.Bold);
        public Color TitleColor = Color.FromArgb(255, 45, 45, 45);
        public Font AuthorsFont = new Font("Trebuchet MS", 9);
        public Color AuthorsColor = Color.FromArgb(255, 96, 96, 96);
        public Size CellPadding = new Size(2, 3);
        public override void Draw(BrightIdeasSoftware.ObjectListView olv, Graphics g, Rectangle r)
        {
            Rectangle cellBounds = this.CellBounds;
            cellBounds.Inflate(-this.CellPadding.Width, -this.CellPadding.Height);
            Rectangle textBounds = cellBounds;
            int currentDPI = (int)g.DpiX;
            this.TitleFont = new Font(this.TitleFont.FontFamily, 9 * ((float) 96 / currentDPI), this.TitleFont.Style);
            this.AuthorsFont = new Font(this.AuthorsFont.FontFamily, 8 * ((float) 96 / currentDPI), this.AuthorsFont.Style);
            //g.DrawRectangle(Pens.Red, textBounds);

            // Draw the title
            using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap))
            {
                fmt.Trimming = StringTrimming.EllipsisCharacter;
                fmt.Alignment = StringAlignment.Near;
                fmt.LineAlignment = StringAlignment.Near;                
                using (SolidBrush b = new SolidBrush(this.TitleColor))
                {
                    g.DrawString(this.Title, this.TitleFont, b, textBounds, fmt);
                }
                // Draw the description
                SizeF size = g.MeasureString(this.Title, this.TitleFont, (int)textBounds.Width, fmt);
                textBounds.Y += (int)size.Height + 4;
                textBounds.Height -= (int)size.Height;
            }

            // Draw the description
            using (StringFormat fmt2 = new StringFormat(StringFormatFlags.NoWrap))
            {
                fmt2.Trimming = StringTrimming.EllipsisCharacter;
                using (SolidBrush b = new SolidBrush(this.AuthorsColor))
                {
                    g.DrawString(((this.Year.Length > 0) ? ("(" + this.Year + ") " ) : "") + this.Authors, this.AuthorsFont, b, textBounds, fmt2);
                }
            }
        }
    }
}
