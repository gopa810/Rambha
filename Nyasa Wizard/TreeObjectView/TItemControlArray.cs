using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Document;

namespace SlideMaker.Views
{
    public class TItemControlArray: TVItemArray
    {
        public List<SMControl> Controls = null;

        public TItemControlArray(TreeObjectView v) : base(v) { }

        public override int GetArraySize()
        {
            return Controls != null ? Controls.Count : 0;
        }

        public override object GetArrayObject(int idx)
        {
            return Controls != null ? Controls[idx] : null;
        }

        public override TVItem CreateItemObject(object a)
        {
            if (a is SMControl)
            {
                return new TItemControl(View) { Control = (a as SMControl) };
            }

            return null;
        }

        public override bool IdenticalData(object data)
        {
            if (data is List<SMControl>)
            {
                return Controls == (data as List<SMControl>);
            }
            return false;
        }

        public override object GetContentData()
        {
            return Controls;
        }
    }
}
