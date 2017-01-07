using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Script;

namespace Rambha.Document
{
    public class GSTitledCore : GSCore
    {
        private GSString p_title = new GSString("");

        public string Title
        {
            get
            {
                return p_title.Value;
            }
            set
            {
                p_title.Value = value;
            }
        }

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "title":
                    return p_title;
                default:
                    return base.GetPropertyValue(s);
            }
        }
    }
}
