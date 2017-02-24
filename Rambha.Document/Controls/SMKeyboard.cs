using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMKeyboard: SMControl
    {
        public List<SMKeyboardKey> Keys = new List<SMKeyboardKey>();

        private StringFormat p_sf = new StringFormat();

        public SMKeyboard(MNPage p)
            : base(p)
        {
            Keys.Add(new SMKeyboardKey() { Key = "Q", X = 01, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "W", X = 10, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "E", X = 19, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "R", X = 28, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "T", X = 37, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "Y", X = 46, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "U", X = 55, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "I", X = 64, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "O", X = 73, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "P", X = 82, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "" + Convert.ToChar(9),
                Image = Properties.Resources.Backkey,
                X = 91, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "A", X = 06, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "S", X = 15, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "D", X = 24, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "F", X = 33, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "G", X = 42, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "H", X = 51, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "J", X = 60, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "K", X = 69, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "L", X = 78, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "" + Convert.ToChar(10), 
                Image = Properties.Resources.Enterkey,
                X = 87, Y = 10, Width = 12, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "Z", X = 10, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "X", X = 19, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "C", X = 28, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "V", X = 37, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "B", X = 46, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "N", X = 55, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "M", X = 64, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = " ", X = 73, Y = 19, Width = 26, Height = 8 });

            p_sf.Alignment = StringAlignment.Center;
            p_sf.LineAlignment = StringAlignment.Center;
        }

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = Page.GetArea(Id);
            Rectangle rect = area.GetBounds(context);
            Brush keyBack = Brushes.LightGray;
            Brush keyPress = Brushes.Blue;

            float a = Math.Min(rect.Width / 100f, rect.Height / 28f);
            Font font = SMGraphics.GetFontVariation(SystemFonts.DefaultFont, 4 * a);
            PointF origin = new PointF(rect.X + rect.Width / 2 - 50 * a, rect.Y + rect.Height / 2 - 14 * a);
            Brush textBrush = Brushes.Black;
            Brush textPressBrush = Brushes.White;

            foreach (SMKeyboardKey key in Keys)
            {
                key.PaintedRect = new RectangleF(key.X*a + origin.X, key.Y*a + origin.Y, key.Width*a, key.Height*a);
                Rectangle r = new Rectangle((int)key.PaintedRect.X, (int)key.PaintedRect.Y, (int)key.PaintedRect.Width, (int)key.PaintedRect.Height);
                context.g.FillRectangle(key.Pressed ? keyPress : keyBack, r);
                if (key.Image != null)
                {
                    float b = Math.Min(key.PaintedRect.Width / key.Image.Width, key.PaintedRect.Height / key.Image.Height);
                    b *= 0.7f;
                    Rectangle r2 = new Rectangle(Convert.ToInt32(key.PaintedRect.X + key.PaintedRect.Width / 2),
                        Convert.ToInt32(key.PaintedRect.Y + key.PaintedRect.Height / 2),
                        Convert.ToInt32(b*key.Image.Width), Convert.ToInt32(b * key.Image.Height));
                    r2.X -= r2.Width / 2;
                    r2.Y -= r2.Height / 2;
                    context.g.DrawImage(key.Image, r2);
                }
                else
                {
                    context.g.DrawString(key.Key, font, key.Pressed ? textPressBrush : textBrush, r, p_sf);
                }
            }

            base.Paint(context);
        }

        public override void OnTapBegin(PVDragContext dc)
        {
            foreach (SMKeyboardKey key in Keys)
            {
                key.Pressed = false;
                if (key.PaintedRect.Contains(dc.lastPoint))
                {
                    key.Pressed = true;
                }
            }

            base.OnTapBegin(dc);
        }

        public override void OnClick(PVDragContext dc)
        {
            foreach (SMKeyboardKey key in Keys)
            {
                if (key.PaintedRect.Contains(dc.lastPoint))
                {
                    if (key.Pressed)
                    {
                        // send message about KeyChar pressed

                    }
                }

                key.Pressed = false;
            }

            base.OnClick(dc);
        }

        public override Size GetDefaultSize()
        {
            return new Size(800,260);
        }

        public class SMKeyboardKey
        {
            public string Key = "";
            public RectangleF PaintedRect = RectangleF.Empty;
            public Brush TextBrush = null;
            public float X, Y, Width, Height;
            public bool Pressed = false;
            public Image Image = null;
        }
    }
}
