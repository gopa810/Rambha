using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;

namespace SlideMaker.Views
{
    public class TItemPage: TVItem
    {
        public MNPage Page { get; set; }

        private TVItem[] p_children = null;

        public TItemPage(TreeObjectView v) : base(v) { }

        public override TVItem[] GetChildren()
        {
            if (p_children == null)
            {
                p_children = new TVItem[]
                {
                    new TItemControlArray(View) { Controls = Page.Objects, Name = "Controls" }
                };
            }

            return p_children;
        }

        public override NABase[] GetActions()
        {
            return base.GetActions();
        }

        public override bool IdenticalData(object data)
        {
            if (data is MNPage)
            {
                return Page == (data as MNPage);
            }
            else
                return false;
        }

        public override object GetContentData()
        {
            return Page;
        }

        public override string GetName()
        {
            return Page != null ? Page.Title : base.GetName();
        }
    }
}
