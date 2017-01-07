using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVGraphConnDataFlow: GVGraphConnection
    {
        public GVGraphConnDataFlow(GVGraph g): base(g)
        {
            ArrowSize = 10;
            LinePen = Pens.BlueViolet;
        }
    }
}
