using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;

namespace SlideMaker.Views
{
    public class NABase
    {
        public string Title { get; set; }
        public TreeObjectView View { get; set; }
        public MNDocument Document { get; set; }
        public MNPage Page { get; set; }
        public SMControl Control { get; set; }

        public NABase(string t)
        {
            Title = t;
        }

        public override string ToString()
        {
            return Title;
        }

        public virtual void Execute()
        {
        }
    }

}
