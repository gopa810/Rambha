using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVGraphConnMember: GVGraphConnection
    {
        public GVGraphConnMember(GVGraph g)
            : base(g)
        {
            ArrowSize = -1;
            LinePen = Pens.Cyan;
        }
    }
}
