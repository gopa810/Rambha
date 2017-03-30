using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;
using Rambha.Script;

namespace SlideMaker.Views
{
    public class TVAction : NABase
    {
        public string Script { get; set; }
        public TVAction(TreeObjectView v, string t)
            : base(t)
        {
            Script = "";
            View = v;
            Document = null;
        }

        public TVAction(TreeObjectView v, string t, MNPage p)
            : base(t)
        {
            Script = "";
            View = v;
            Page = p;
            Document = null;
        }

        public TVAction(TreeObjectView v, string t, MNPage p, string script)
            : base(t)
        {
            Script = "";
            View = v;
            Page = p;
            Script = script;
            Document = null;
        }

        public TVAction(TreeObjectView v, string t, MNDocument d)
            : base(t)
        {
            Script = "";
            View = v;
            Page = null;
            Document = d;
        }

        public TVAction(TreeObjectView v, string t, MNDocument d, string script)
            : base(t)
        {
            Script = "";
            View = v;
            Page = null;
            Script = script;
            Document = d;
        }


        public override void Execute()
        {
            if (Script.Equals("insertPage"))
            {
                if (Page != null)
                {
                    DialogEnterPageNames dlg = new DialogEnterPageNames();
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string[] names = dlg.Names;
                        if (names != null && names.Length > 0)
                        {
                            List<MNPage> pages = Page.Document.Data.Pages;
                            int idx = pages.IndexOf(Page);
                            if (idx >= 0 && idx < pages.Count)
                            {
                                foreach (string pageName in names)
                                {
                                    idx++;
                                    MNPage p = new MNPage(Page.Document);
                                    p.Title = pageName;
                                    p.Id = Page.Document.Data.GetNextId();
                                    pages.Insert(idx, p);
                                }
                            }

                            MNNotificationCenter.BroadcastMessage(Page, "PageInserted");
                        }
                    }
                }
            }
            else if (Script.Equals("addPage"))
            {
                if (Document != null)
                    Document.CreateNewPage();
                else if (Page != null)
                    Page.Document.CreateNewPage();
                MNNotificationCenter.BroadcastMessage(Page, "PageInserted");
            }
            else if (Script.Equals("deletePage"))
            {
                if (Page != null)
                {
                    if (MessageBox.Show("Are you sure to delete this page?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Page.Document.Data.Pages.Remove(Page);
                        MNNotificationCenter.BroadcastMessage(Page, "PageInserted");
                    }
                }
            }
            else if (Script.Equals("addToShared"))
            {
                if (Page != null)
                {
                    MNSharedObjects.AddTemplate(Page);
                    MNSharedObjects.Save();
                }
            }
            else if (Script.Equals("addTemplate"))
            {
                if (Document != null)
                    Document.CreateNewTemplate();
                else if (Page != null)
                    Page.Document.CreateNewTemplate();
                MNNotificationCenter.BroadcastMessage(Page, "PageInserted");
            }
            else if (Script.Equals("addMenu"))
            {
                MNMenu menu = new MNMenu();
                if (Document != null)
                    Document.Data.Menus.Add(menu);
                else if (Page != null)
                    Page.Document.Data.Menus.Add(menu);
            }
            else if (Script.Equals("addScript"))
            {
                if (Document != null)
                {
                    Document.CreateNewText(true);
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
            }
            else if (Script.Equals("addText"))
            {
                if (Document != null)
                {
                    Document.CreateNewText(false);
                }
            }

            base.Execute();
        }
    }

}
