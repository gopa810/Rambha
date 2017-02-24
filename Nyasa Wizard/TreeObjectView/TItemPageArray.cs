using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;

namespace SlideMaker.Views
{
    public class TVItemPageArray: TVItemArray
    {
        public List<MNPage> Array = null;

        public TVItemPageArray(TreeObjectView v) : base(v) { }

        public override int GetArraySize()
        {
            return Array != null ? Array.Count : 0;
        }

        public override object GetArrayObject(int idx)
        {
            return Array[idx];
        }

        public override TVItem CreateItemObject(object a)
        {
            if (a != null && a is MNPage)
            {
                return new TItemPage(View) { Page = (a as MNPage) };
            }
            return null;
        }

        public override object GetContentData()
        {
            return Array;
        }

        public override bool IdenticalData(object data)
        {
            if (data is List<MNPage>)
            {
                return Array == data;
            }
            return false;
        }
    }


    public class TVItemTextArray : TVItemArray
    {
        public List<MNReferencedText> Array = null;

        public TVItemTextArray(TreeObjectView v) : base(v) { }

        public override int GetArraySize()
        {
            return Array != null ? Array.Count : 0;
        }

        public override object GetArrayObject(int idx)
        {
            return Array[idx];
        }

        public override TVItem CreateItemObject(object a)
        {
            if (a != null && a is MNReferencedText)
            {
                return new TItemText(View) { Text = (a as MNReferencedText) };
            }
            return null;
        }

        public override object GetContentData()
        {
            return Array;
        }

        public override bool IdenticalData(object data)
        {
            if (data is List<MNReferencedText>)
            {
                return Array == data;
            }
            return false;
        }
    }

    public class TVItemMenuArray : TVItemArray
    {
        public List<MNMenu> Array = null;

        public TVItemMenuArray(TreeObjectView v) : base(v) { }

        public override int GetArraySize()
        {
            return Array != null ? Array.Count : 0;
        }

        public override object GetArrayObject(int idx)
        {
            return Array[idx];
        }

        public override TVItem CreateItemObject(object a)
        {
            if (a != null && a is MNMenu)
            {
                return new TItemMenu(View) { Menu = (a as MNMenu) };
            }
            return null;
        }

        public override object GetContentData()
        {
            return Array;
        }

        public override bool IdenticalData(object data)
        {
            if (data is List<MNMenu>)
            {
                return Array == data;
            }
            return false;
        }
    }

}
