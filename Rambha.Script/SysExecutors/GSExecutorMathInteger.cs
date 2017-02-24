using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSExecutorMathInteger: GSCore
    {
        public GSExecutor Parent = null;

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            GSCore result = null;
            if (token.Equals("add") || token.Equals("+"))
                result = execAdd(Parent.getNativeValues(args));
            else if (token.Equals("sub") || token.Equals("-"))
                result = execSub(Parent.getNativeValues(args));
            else if (token.Equals("mul") || token.Equals("*"))
                result = execMul(Parent.getNativeValues(args));
            else if (token.Equals("div") || token.Equals("/"))
                result = execDiv(Parent.getNativeValues(args));
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
            switch(s)
            {
                case "PI":
                    return new GSDouble(Math.PI);
                default:
                    return base.GetPropertyValue(s);
            }
        }

        private GSCore execGt(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getIntegerValue() > arg1[1].getIntegerValue());
            return bv;
        }

        private GSCore execGe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getIntegerValue() >= arg1[1].getIntegerValue());
            return bv;
        }

        private GSCore execEq(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getIntegerValue() == arg1[1].getIntegerValue());
            return bv;
        }

        private GSCore execNe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getIntegerValue() != arg1[1].getIntegerValue());
            return bv;
        }

        private GSCore execLe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getIntegerValue() <= arg1[1].getIntegerValue());
            return bv;
        }

        private GSCore execLt(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getIntegerValue() < arg1[1].getIntegerValue());
            return bv;
        }


        private GSCore execDiv(GSCoreCollection args)
        {
            long[] arr = Parent.getIntegerArray(args);
            long sum = arr[0];
            for (int i = 1; i < arr.Length; i++)
                sum /= arr[i];
            return new GSInt64() { Int64Value = sum };
        }

        private GSCore execMul(GSCoreCollection args)
        {
            long[] arr = Parent.getIntegerArray(args);
            long sum = 1;
            for (int i = 0; i < arr.Length; i++)
                sum *= arr[i];
            return new GSInt64() { Int64Value = sum };
        }

        private GSCore execSub(GSCoreCollection args)
        {
            long[] arr = Parent.getIntegerArray(args);
            long sum = arr[0];
            for (int i = 1; i < arr.Length; i++)
                sum -= arr[i];
            return new GSInt64() { Int64Value = sum };
        }

        private GSCore execAdd(GSCoreCollection args)
        {
            long[] arr = Parent.getIntegerArray(args);
            long sum = 0;
            for (int i = 0; i < arr.Length; i++)
                sum += arr[i];
            return new GSInt64() { Int64Value = sum };
        }

    }
}
