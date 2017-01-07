using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVGraphConnControlFlow: GVGraphConnection
    {
        public GVGraphConnControlFlow(GVGraph g): base(g)
        {
            LinePen = Pens.Black;
            ArrowSize = 15;
        }
    }
}
