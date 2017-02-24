using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSExecutorLog: GSCore
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
            bool result = true;

            if (args.Count > 0)
                result = !args[0].getBooleanValue();
            return new GSBoolean() { BooleanValue = result };
        }

        private GSCore execOr(GSCoreCollection args)
        {
            bool result = false;
            foreach (GSCore item in args)
            {
                if (item.getBooleanValue() == true)
                {
                    result = true;
                    break;
                }
            }
            return new GSBoolean() { BooleanValue = result };
        }

        private GSCore execAnd(GSCoreCollection args)
        {
            bool result = true;
            foreach (GSCore item in args)
            {
                if (item.getBooleanValue() == false)
                {
                    result = false;
                    break;
                }
            }
            return new GSBoolean() { BooleanValue = result };
        }
    }
}
