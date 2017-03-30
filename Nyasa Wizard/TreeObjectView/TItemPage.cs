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
                    new TItemControlArray(View) { Controls = Page.Objects, Name = "Controls" },
                    new TVItemTextArray(View) { Array = Page.Scripts, Name = "Page Scripts" },
                };
            }

            return p_children;
        }

        public override NABase[] GetActions()
        {
            return new NABase[]
            {
                new TVAction(View, "New Script", Page, "addScript"),
                new TVAction(View, "Insert Pages", Page, "insertPage"),
                new TVAction(View, "Delete Page", Page, "deletePage"),
                new TVAction(View, "Add Page to Shared", Page, "addToShared")

            };
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

    /// <summary>
    /// Item presenting the MNReferencedText
    /// </summary>
    public class TItemText : TVItem
    {
        public MNReferencedText Text { get; set; }

        private TVItem[] p_children = null;

        public TItemText(TreeObjectView v) : base(v) { }

        public override TVItem[] GetChildren()
        {
            return p_children;
        }

        public override NABase[] GetActions()
        {
            return base.GetActions();
        }

        public override bool IdenticalData(object data)
        {
            if (data is MNReferencedText)
            {
                return Text == (data as MNReferencedText);
            }
            else
                return false;
        }

        public override object GetContentData()
        {
            return Text;
        }

        public override string GetName()
        {
            return Text != null ? Text.Name : base.GetName();
        }
    }


    public class TItemMenu : TVItem
    {
        public MNMenu Menu { get; set; }

        private TVItem[] p_children = null;

        public TItemMenu(TreeObjectView v) : base(v) { }

        public override TVItem[] GetChildren()
        {
            return p_children;
        }

        public override NABase[] GetActions()
        {
            return base.GetActions();
        }

        public override bool IdenticalData(object data)
        {
            if (data is MNMenu)
            {
                return Menu == (data as MNMenu);
            }
            else
                return false;
        }

        public override object GetContentData()
        {
            return Menu;
        }

        public override string GetName()
        {
            return Menu != null ? Menu.APIName : base.GetName();
        }
    }


}
