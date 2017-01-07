using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlideMaker.Views
{
    /// <summary>
    /// This is base class for collection of items. When showing single object in TreeObjectView, then
    /// we need mapping TYPE => subclass of TVItem
    /// This TVItemArray class serves as helper class for mapping List(TYPE) => List(subclass of TVItem)
    /// </summary>
    public abstract class TVItemArray: TVItem
    {
        public TVItemArray(TreeObjectView v) : base(v) { }

        public override TVItem[] GetChildren()
        {
            List<TVItem> p_items = Children;
            List<TVItem> ta = new List<TVItem>();
            if (GetArraySize() > 0)
            {
                for (int j = 0; j < GetArraySize(); j++)
                {
                    int idx = FindPage(p_items, GetArrayObject(j));
                    if (idx >= 0)
                    {
                        ta.Add(p_items[idx]);
                        p_items[idx].RefreshChildren();
                    }
                    else
                    {
                        ta.Add(CreateItemObject(GetArrayObject(j)));
                    }
                }
            }
            p_items.Clear();
            p_items.AddRange(ta);
            return ta.ToArray<TVItem>();
        }

        public abstract int GetArraySize();

        public abstract object GetArrayObject(int idx);

        public abstract TVItem CreateItemObject(object a);

        public int FindPage(List<TVItem> tip, object p)
        {
            for (int i = 0; i < tip.Count; i++)
            {
                if (tip[i].IdenticalData(p))
                    return i;
            }
            return -1;
        }
    }
}
