using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;

namespace SlideMaker.Views
{
    public class TItemControl: TVItem
    {
        public SMControl Control { get; set; }

        public TItemControl(TreeObjectView v) : base(v) { }

        public override object GetContentData()
        {
            return Control;
        }

        public override bool IdenticalData(object data)
        {
            if (data is SMControl)
            {
                return Control == (data as SMControl);
            }
            else
            {
                return false;
            }
        }

        public override string GetName()
        {
            return Control != null ? string.Format("{0} : {1}", Control.GetType().Name, Control.Tag) : "?";
        }
    }
}
