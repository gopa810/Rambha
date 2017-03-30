using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;

namespace SlideMaker.Views
{
    public class TItemDocument: TVItem
    {
        public MNDocument Document { get; set; }

        public TVItem[] p_children = null;

        public TItemDocument(TreeObjectView v) : base(v) { }


        public override NABase[] GetActions()
        {
            return new NABase[]
            {
                new TVAction(View, "New Page", Document, "addPage"),
                new TVAction(View, "New Template", Document, "addTemplate"),
                new TVAction(View, "New Script", Document, "addScript"),
                new TVAction(View, "New Text", Document, "addText"),
                new TVAction(View, "New Menu", Document, "addMenu"),
            };
        }

        public override TVItem[] GetChildren()
        {
            if (p_children == null)
            {
                p_children = new TVItem[] 
                {
                    new TVItemPageArray(View) { Array = Document.Data.Pages, Name = "Pages"},
                    new TVItemPageArray(View) { Array = Document.Data.Templates, Name = "Templates"},
                    new TVItemTextArray(View) { Array = Document.Data.Scripts, Name = "Executable Scripts"},
                    new TVItemTextArray(View) { Array = Document.DefaultLanguage.Texts, Name = "Texts"},
                    new TVItemMenuArray(View) { Array = Document.Data.Menus, Name = "Menus"}
                };
            }

            return p_children;
        }

        public override string GetName()
        {
            return (Document != null && Document.Book.BookTitle != null && Document.Book.BookTitle.Length > 0) ? Document.Book.BookTitle : "Document";
        }

        public override object GetContentData()
        {
            return Document;
        }

        public override bool IdenticalData(object data)
        {
            if (data is MNDocument)
            {
                return Document == (data as MNDocument);
            }

            return false;
        }
    }
}
