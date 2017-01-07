using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;

namespace SlideMaker.Views
{
    public class PageEditViewArguments: EventArgs
    {
        public MNDocument Document { get; set; }
        public MNPage Page { get; set; }
        public object Object { get; set; }
        public PageEditView PageView { get; set; }
    }
}
