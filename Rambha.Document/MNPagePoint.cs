using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace Rambha.Document
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

        /// <summary>
        /// returns logical coordinates of point, 
        /// it takes into account whether point is linked to page or image
        /// </summary>
        /// <returns></returns>
        public Point GetLogicalCenter(MNPageContext context)
        {
            SMRectangleArea area = Page.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);
            return new Point(bounds.Left + bounds.Width/2, bounds.Top + bounds.Height/2);
        }

        public override void Paint(MNPageContext context)
        {
            Point lc = GetLogicalCenter(context);
            int radius = Radius;

            SMRectangleArea area = Page.GetArea(Id);
            if (area.Selected && context.TrackedObjects.Count > 0)
            {
                lc.Offset(context.TrackedDrawOffset);
            }

            if (Radius > 0)
                context.g.FillEllipse(Brushes.OrangeRed, lc.X - radius, lc.Y - radius, radius * 2, radius * 2);

        }
    }


}
