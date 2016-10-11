using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace SlideMaker.Document
{
    [Serializable()]
    public class MNPageImage: SMControl
    {
        public MNReferencedImage Image { get; set; }

        // actual size in pixels
        public Size Size { get; set; }

        public MNPageImage(MNPage p)
            : base(p)
        {
        }

        public override List<SMControl> Load(XmlElement e)
        {
            List<SMControl> list = base.Load(e);
            foreach (XmlElement e1 in e.ChildNodes)
            {
                if (e1.Name.Equals("size")) Size = LoadSize(e1);
            }
            Image = Document.FindImage(e.GetAttribute("imageName"));
            return list;
        }

        public override XmlElement Save(XmlDocument doc)
        {
            XmlElement e = base.Save(doc);
            e.AppendChild(SaveSize(doc, Size, "size"));
            e.SetAttribute("imageName", Image.Title);
            return e;
        }

        public Rectangle LogicalRectangle(MNPageContext context)
        {
            return GetBounds(context);
        }

        public override void Paint(MNPageContext context)
        {
            Graphics g = context.g;
            MNPageImage pi = this;
            Rectangle rect = pi.GetBounds(context);
            Point center = new Point(rect.Left + rect.Width/2, rect.Top + rect.Height/2);

            if (pi.Selected != SMControlSelection.None && context.TrackedObjects.Count > 0)
            {
                rect.Offset(context.TrackedDrawOffset);
                center.Offset(context.TrackedDrawOffset);
            }
            g.DrawImage(pi.Image.ImageData, rect);

        }
    }
}
