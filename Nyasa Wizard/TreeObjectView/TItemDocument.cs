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
                new TADocAddPage(View, "New Page", Document, true),
                new TADocAddPage(View, "New Template", Document, false),
            };
        }

        public override TVItem[] GetChildren()
        {
            if (p_children == null)
            {
                p_children = new TVItem[] 
                {
                    new TVItemPageArray(View) { Array = Document.Pages, Name = "Pages"},
                    new TVItemPageArray(View) { Array = Document.Templates, Name = "Templates"} 
                };
            }

            return p_children;
        }

        public override string GetName()
        {
            return (Document != null && Document.BookTitle != null && Document.BookTitle.Length > 0) ? Document.BookTitle : "Document";
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
