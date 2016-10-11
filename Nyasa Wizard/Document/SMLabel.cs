using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.ComponentModel;
using System.Drawing.Design;

namespace SlideMaker.Document
{
    public class SMLabel: SMControl
    {
        [Browsable(true)]
        public int FontSize { get; set; }

        [Browsable(true)]
        public Size TextSize { get; set; }

        public SMLabel(MNPage p): base(p)
        {
            Text = "Label";
        }

        public override SMControl CreateCopy()
        {
            SMLabel F = base.CreateCopy() as SMLabel;
            F.FontSize = this.FontSize;
            F.TextSize = this.TextSize;
            return F;
        }

        public override List<SMControl> Load(XmlElement e)
        {
            List < SMControl >  list = base.Load(e);
            foreach (XmlElement e1 in e.ChildNodes)
            {
                if (e1.Name == "Text")
                {
                    Text = e1.InnerText;
                }
                else if (e1.Name == "TextSize")
                {
                    TextSize = LoadSize(e1);
                }
                else if (e1.Name == "FontSize")
                {
                    FontSize = LoadInteger(e1);
                }
            }
            return list;
        }

        public override XmlElement Save(XmlDocument doc)
        {
            XmlElement e = base.Save(doc);

            XmlElement e1 = doc.CreateElement("Text");
            e1.InnerText = Text;
            e.AppendChild(e1);

            e.AppendChild(SaveSize(doc, TextSize, "TextSize"));

            e.AppendChild(SaveInteger(doc, FontSize, "FontSize"));

            return e;
        }

        public Font FontOfThisText
        {
            get
            {
                if (FontSize < 10)
                    return Page.DefaultLabelFont;
                return MNDocument.FontOfSize(FontSize);
            }
        }

        public override void Paint(MNPageContext context)
        {
            SizeF sf = context.g.MeasureString(Text, FontOfThisText);
            TextSize = new Size((int)sf.Width, (int)sf.Height);
            Rectangle bounds = GetBounds(context);

            context.g.DrawString(Text, FontOfThisText, Brushes.Black, bounds);

            // draw selection marks
            base.Paint(context);
        }
    }
}
