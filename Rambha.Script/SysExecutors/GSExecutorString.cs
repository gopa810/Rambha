using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSExecutorString: GSCore
    {
        public GSExecutor Parent = null;

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            GSCore result = null;
            if (token.Equals("add") || token.Equals("+"))
                result = execAdd(Parent.getNativeValues(args));
            else if ((token.Equals("gt") || token.Equals(">")) && args.Count > 1)
                result = execGt(Parent.getNativeValues(args));
            else if ((token.Equals("ge") || token.Equals(">=")) && args.Count > 1)
                result = execGe(Parent.getNativeValues(args));
            else if ((token.Equals("eq") || token.Equals("==")) && args.Count > 1)
                result = execEq(Parent.getNativeValues(args));
            else if ((token.Equals("ne") || token.Equals("!=")) && args.Count > 1)
                result = execNe(Parent.getNativeValues(args));
            else if ((token.Equals("le") || token.Equals("<=")) && args.Count > 1)
                result = execLe(Parent.getNativeValues(args));
            else if ((token.Equals("lt") || token.Equals("<")) && args.Count > 1)
                result = execLt(Parent.getNativeValues(args));
            return base.ExecuteMessage(token, args);
        }

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "Empty":
                    return new GSString();
                default:
                    return base.GetPropertyValue(s);
            }
        }


        private GSCore execGt(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getStringValue().CompareTo(arg1[1].getStringValue()) > 0);
            return bv;
        }

        private GSCore execGe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getStringValue().CompareTo(arg1[1].getStringValue()) >= 0);
            return bv;
        }

        private GSCore execEq(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getStringValue().CompareTo(arg1[1].getStringValue()) == 0);
            return bv;
        }

        private GSCore execNe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getStringValue().CompareTo(arg1[1].getStringValue()) != 0);
            return bv;
        }

        private GSCore execLe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getStringValue().CompareTo(arg1[1].getStringValue()) <= 0);
            return bv;
        }

        private GSCore execLt(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getStringValue().CompareTo(arg1[1].getStringValue()) < 0);
            return bv;
        }

        private GSCore execAdd(GSCoreCollection args)
        {
            string[] arr = Parent.getStringArray(args);
            StringBuilder sb = new StringBuilder();
            foreach (string s in arr)
            {
                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append(s);
            }
            return new GSString() { Value = sb.ToString() };
        }
    }
}
