using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;
using Rambha.Script;

namespace SlideMaker.Views
{
    public class TADocAddPage : NABase
    {
        bool IsPage = true;

        public TADocAddPage(TreeObjectView v, string t, MNDocument doc, bool isPage)
            : base(t)
        {
            View = v;
            Document = doc;
            IsPage = isPage;
        }

        public override void Execute()
        {
            GSCore newObject = null;
            if (IsPage)
                newObject = Document.CreateNewPage();
            else
                newObject = Document.CreateNewTemplate();
            View.Invalidate(newObject);
            base.Execute();
        }
    }

    public class TADocAddMenu : NABase
    {
        public TADocAddMenu(TreeObjectView v, string t, MNDocument doc)
            : base(t)
        {
            View = v;
            Document = doc;
        }

        public override void Execute()
        {
            MNMenu menu = new MNMenu();
            Document.Data.Menus.Add(menu);
            base.Execute();
        }
    }

    public class TADocAddText : NABase
    {
        bool IsScript = true;

        public TADocAddText(TreeObjectView v, string t)
            : base(t)
        {
            View = v;
            Document = null;
            IsScript = true;
        }


        public TADocAddText(TreeObjectView v, string t, MNDocument doc, bool is_script) : base(t)
        {
            View = v;
            Document = doc;
            IsScript = is_script;
        }

        public override void Execute()
        {
            if (Document != null)
            {
                Document.CreateNewText(IsScript);
            }
            else if (Page != null)
            {
                MNReferencedText rt = new MNReferencedText();
                rt.Name = "Untitled";
                Page.Scripts.Add(rt);
                MNNotificationCenter.BroadcastMessage(Page, "TextInserted", rt);
            }
            else if (Control != null)
            {
                MNReferencedText rt = new MNReferencedText();
                rt.Name = "Untitled";
                Control.Scripts.Add(rt);
                MNNotificationCenter.BroadcastMessage(Control, "TextInserted", rt);
            }
            base.Execute();
        }
    }
}
