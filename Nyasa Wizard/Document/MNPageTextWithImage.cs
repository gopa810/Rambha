using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace SlideMaker.Document
{
    [Serializable()]
    public class MNPageTextWithImage : SMControl
    {
        public MNReferencedMantra Mantra { get; set; }
        public int ImageCode { get; set; }
        public Size ImageSize { get; set; }
        public Size TextSize { get; set; }

        public override List<SMControl> Load(XmlElement e)
        {
            List < SMControl > list = base.Load(e);
            foreach (XmlElement e1 in e.ChildNodes)
            {
                switch (e1.Name)
                {
                    case "mantra":
                        MNReferencedMantra m = new MNReferencedMantra();
                        m.Load(e1);
                        Mantra = m;
                        break;
                    case "ImageCode": ImageCode = LoadInteger(e1); break;
                    case "ImageSize": ImageSize = LoadSize(e1); break;
                    case "TextSize": TextSize = LoadSize(e1); break;
                    case "Text": Text = LoadString(e1); break;
                    default: break;
                }
            }
            return list;
        }

        public override XmlElement Save(XmlDocument doc)
        {
            XmlElement e = base.Save(doc);
            e.AppendChild(SaveString(doc, Text, "Text"));
            e.AppendChild(SaveInteger(doc, ImageCode, "ImageCode"));
            e.AppendChild(SaveSize(doc, TextSize, "TextSize"));
            e.AppendChild(SaveSize(doc, ImageSize, "ImageSize"));
            if (Mantra != null)
                e.AppendChild(Mantra.Save(doc));
            return e;
        }

        public MNPageTextWithImage(MNPage p): base(p)
        {
            ImageSize = Size.Empty;
            TextSize = Size.Empty;
            Text = "";
        }

        public void SetMantra(MNReferencedMantra m)
        {
            Mantra = m;
            TextSize = Size.Empty;
            Text = m.Number + ". " + m.MantraText;
        }

        public Rectangle GetLogicalRectangle(MNPageContext context)
        {
            return GetBounds(context);
        }

        public override void Paint(MNPageContext context)
        {
            if (ImageSize.Width == 0)
            {
                ImageSize = Page.DefaultTextImageSize;
            }

            SizeF mainTextSize = context.g.MeasureString(Text, Page.DefaultLabelFont);

            if (TextSize.Width == 0)
            {
                SizeF subTextSize = context.g.MeasureString(Mantra.TouchedPartText, Page.DefaultSubLabelFont);
                TextSize = new Size((int)Math.Max(mainTextSize.Width,subTextSize.Width), (int)mainTextSize.Height + (int)subTextSize.Height);
            }

            Rectangle rc = GetLogicalRectangle(context);

            if (Selected != SMControlSelection.None && context.TrackedObjects.Count > 0)
            {
                rc.Offset(context.TrackedDrawOffset);
            }
            
            Image handImage = context.GetHandImage(ImageCode);

            context.g.DrawImage(handImage, rc.X, rc.Y, ImageSize.Width, ImageSize.Height);

            context.g.FillRectangle(Brushes.White, rc.X + ImageSize.Width, rc.Y, TextSize.Width, TextSize.Height);
            context.g.DrawString(Text, Page.DefaultLabelFont, Brushes.Black, rc.X + ImageSize.Width, rc.Y);
            context.g.DrawString(Mantra.TouchedPartText, Page.DefaultSubLabelFont, Brushes.DarkGray, rc.X + ImageSize.Width, rc.Y + mainTextSize.Height);

        }
    }
}
