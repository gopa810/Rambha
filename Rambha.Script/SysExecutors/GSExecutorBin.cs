using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSExecutorBin: GSCore
    {
        public GSExecutor Parent = null;

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            GSCore result = null;

            if (token.Equals("and") || token.Equals("&"))
                result = execAnd(Parent.getNativeValues(args));
            else if (token.Equals("or") || token.Equals("|"))
                result = execOr(Parent.getNativeValues(args));
            else if (token.Equals("not") || token.Equals("!"))
                result = execNot(args);

            return base.ExecuteMessage(token, args);
        }


        private GSCore execNot(GSCoreCollection args)
        {
            int result = 0;

            if (args.Count > 0)
                result = ~((int)args[0].getIntegerValue());
            return new GSInt32(result);
        }

        private GSCore execOr(GSCoreCollection args)
        {
            int result = 0;
            foreach (GSCore item in args)
            {
                int a = (int)item.getIntegerValue();
                result |= a;
            }
            return new GSInt32(result);
        }

        private GSCore execAnd(GSCoreCollection args)
        {
            bool b = false;
            int result = 0;
            foreach (GSCore item in args)
            {
                int a = (int)item.getIntegerValue();
                if (b)
                    result &= a;
                else
                {
                    result = a;
                    b = true;
                }
            }
            return new GSInt32(result);
        }
    }
}
