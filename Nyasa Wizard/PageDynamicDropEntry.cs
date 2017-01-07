using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlideMaker
{
    public class PageDynamicDropEntry
    {
        public string Text { get; set; }

        public string Tag { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
