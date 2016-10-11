using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlideMaker.Document;

namespace SlideMaker
{
    public class PageEditViewArguments: EventArgs
    {
        public MNDocument Document { get; set; }
        public MNPage Page { get; set; }
        public SMControl Object { get; set; }
        public PageEditView PageView { get; set; }
    }
}
