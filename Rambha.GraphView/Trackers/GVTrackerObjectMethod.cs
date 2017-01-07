using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.GraphView
{
    public class GVTrackerObjectMethod : GVTrackerConnectionBase
    {
        public GVTrackerObjectMethod()
        {
            Title = "Action";
        }

        public override GVGraphConnection GetLinkType(GVGraph g)
        {
            return new GVGraphConnOwnership(g);
        }

        public override System.Drawing.Image GetTrackerIcon()
        {
            return Properties.Resources.IconObjectMethod;
        }
    }
}
