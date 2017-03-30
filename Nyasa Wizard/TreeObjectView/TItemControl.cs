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

        public TVItem[] p_children = null;

        public TItemControl(TreeObjectView v) : base(v) { }

        public override object GetContentData()
        {
            return Control;
        }

        public override NABase[] GetActions()
        {
            return new NABase[]
            {
                new TVAction(View, "New Script") { Control = this.Control, Script = "addScript" }
            };
        }

        public override TVItem[] GetChildren()
        {
            if (p_children == null)
            {
                p_children = new TVItem[] 
                {
                    new TVItemTextArray(View) { Array = Control.Scripts, Name = "Scripts (control)"}
                };
            }

            return p_children;
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
            return Control != null ? string.Format("{0} : {1}", Control.GetType().Name, (Control.Text.Length < 20 ? Control.Text : Control.Text.Substring(0,20))) : "?";
        }
    }
}
