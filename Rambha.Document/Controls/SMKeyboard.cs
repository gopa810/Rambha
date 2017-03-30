using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;
using Rambha.Script;

namespace Rambha.Document
{
    public class SMKeyboard: SMControl
    {
        public List<SMKeyboardKey> Keys = new List<SMKeyboardKey>();

        private StringFormat p_sf = new StringFormat();

        public string TargetControl { get; set; }

        public SMKeyboard(MNPage p)
            : base(p)
        {
            Keys.Add(new SMKeyboardKey() { Key = "q", X = 01, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "w", X = 10, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "e", X = 19, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "r", X = 28, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "t", X = 37, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "y", X = 46, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "u", X = 55, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "i", X = 64, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "o", X = 73, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "p", X = 82, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "#back",
                Image = Properties.Resources.Backkey,
                X = 91, Y = 1, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "a", X = 06, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "s", X = 15, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "d", X = 24, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "f", X = 33, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "g", X = 42, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "h", X = 51, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "j", X = 60, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "k", X = 69, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "l", X = 78, Y = 10, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "#enter", 
                Image = Properties.Resources.Enterkey,
                X = 87, Y = 10, Width = 12, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "z", X = 10, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "x", X = 19, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "c", X = 28, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "v", X = 37, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "b", X = 46, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "n", X = 55, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = "m", X = 64, Y = 19, Width = 8, Height = 8 });
            Keys.Add(new SMKeyboardKey() { Key = " ", X = 73, Y = 19, Width = 26, Height = 8 });

            p_sf.Alignment = StringAlignment.Center;
            p_sf.LineAlignment = StringAlignment.Center;
        }

        public override void Paint(MNPageContext context)
        {
            Rectangle rect = Area.GetBounds(context);
            Brush keyBack = Brushes.LightGray;
            Brush keyPress = Brushes.Blue;

            float ax = rect.Width / 100f;
            float ay = rect.Height / 28f;
            Font font = SMGraphics.GetFontVariation(SystemFonts.DefaultFont, 4 * Math.Min(ax, ay));
            PointF origin = new PointF(rect.X + rect.Width / 2 - 50 * ax, rect.Y + rect.Height / 2 - 14 * ay);
            Brush textBrush = Brushes.Black;
            Brush textPressBrush = Brushes.White;

            foreach (SMKeyboardKey key in Keys)
            {
                key.PaintedRect = new RectangleF(key.X*ax + origin.X, key.Y*ay + origin.Y, key.Width*ax, key.Height*ay);
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
                        SMControl targetControl = Page.FindObjectWithAPIName(TargetControl);
                        if (targetControl != null)
                        {
                            if (key.Key.Equals("#back"))
                                targetControl.ExecuteMessage("acceptBack");
                            else if (key.Key.Equals("#enter"))
                                targetControl.ExecuteMessage("acceptEnter");
                            else
                                targetControl.ExecuteMessage("acceptString", new GSString(key.Key));
                        }
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

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                byte tag;
                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10:
                            TargetControl = br.ReadString();
                            break;
                        default:
                            return false;
                    }
                }
                return true;
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteString(TargetControl);

            bw.WriteByte(0);
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
