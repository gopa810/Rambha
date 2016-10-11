using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace SlideMaker.Document
{
    [Serializable()]
    public class MNPagePoint : SMControl
    {
        public int Radius { get; set; }

        public bool EditParentProperties { get; set; }

        public MNPagePoint(MNPage p): base(p)
        {
            EditParentProperties = false;
        }

        public override XmlElement Save(XmlDocument doc)
        {
            XmlElement e = base.Save(doc);
            e.SetAttribute("radius", Radius.ToString());
            e.SetAttribute("EditParentProperties", EditParentProperties.ToString());
            return e;
        }

        public override List<SMControl> Load(XmlElement e)
        {
            List < SMControl > list =  base.Load(e);

            Radius = int.Parse(e.GetAttribute("radius"));
            EditParentProperties = bool.Parse(e.GetAttribute("EditParentProperties"));

            return list;
        }

        /// <summary>
        /// returns logical coordinates of point, 
        /// it takes into account whether point is linked to page or image
        /// </summary>
        /// <returns></returns>
        public Point GetLogicalCenter(MNPageContext context)
        {
            Rectangle bounds = GetBounds(context);
            return new Point(bounds.Left + bounds.Width/2, bounds.Top + bounds.Height/2);
        }

        public override void Paint(MNPageContext context)
        {
            Point lc = GetLogicalCenter(context);
            int radius = Radius;

            if (Selected != SMControlSelection.None && context.TrackedObjects.Count > 0)
            {
                lc.Offset(context.TrackedDrawOffset);
            }

            if (Radius > 0)
                context.g.FillEllipse(Brushes.OrangeRed, lc.X - radius, lc.Y - radius, radius * 2, radius * 2);

        }
    }


}
