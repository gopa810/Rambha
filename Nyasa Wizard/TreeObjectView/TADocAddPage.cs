using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;
using Rambha.Script;

namespace SlideMaker.Views
{
    public class TADocAddPage: NABase
    {
        bool IsPage = true;

        public TADocAddPage(TreeObjectView v, string t, MNDocument doc, bool isPage) : base(t)
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
}
