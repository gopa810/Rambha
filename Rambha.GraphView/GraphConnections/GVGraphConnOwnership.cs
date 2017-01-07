using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVGraphConnOwnership: GVGraphConnection
    {
        public GVGraphConnOwnership(GVGraph g): base(g)
        {
            LinePen = Pens.Brown;
            ArrowSize = 0;
        }
    }
}
