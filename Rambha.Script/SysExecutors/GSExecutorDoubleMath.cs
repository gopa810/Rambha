using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSExecutorDoubleMath: GSCore
    {
        public GSExecutor Parent = null;

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            GSCore result = null;
            if (token.Equals("add") || token.Equals("+"))
                result = execAdd(getNativeValues(args));
            else if (token.Equals("sub") || token.Equals("-"))
                result = execSub(getNativeValues(args));
            else if (token.Equals("mul") || token.Equals("*"))
                result = execMul(getNativeValues(args));
            else if (token.Equals("div") || token.Equals("/"))
                result = execDiv(getNativeValues(args));
            else if ((token.Equals("gt") || token.Equals(">")) && args.Count > 1)
                result = execGt(getNativeValues(args));
            else if ((token.Equals("ge") || token.Equals(">=")) && args.Count > 1)
                result = execGe(getNativeValues(args));
            else if ((token.Equals("eq") || token.Equals("==")) && args.Count > 1)
                result = execEq(getNativeValues(args));
            else if ((token.Equals("ne") || token.Equals("!=")) && args.Count > 1)
                result = execNe(getNativeValues(args));
            else if ((token.Equals("le") || token.Equals("<=")) && args.Count > 1)
                result = execLe(getNativeValues(args));
            else if ((token.Equals("lt") || token.Equals("<")) && args.Count > 1)
                result = execLt(getNativeValues(args));
            return base.ExecuteMessage(token, args);
        }

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "PI":
                    return new GSDouble(Math.PI);
                default:
                    return base.GetPropertyValue(s);
            }
        }

        public long[] getIntegerArray(GSCoreCollection C)
        {
            long[] result = new long[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = Parent.ExecuteElement(C[i]).getIntegerValue();
            }
            return result;
        }
        public double[] getDoubleArray(GSCoreCollection C)
        {
            double[] result = new double[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = Parent.ExecuteElement(C[i]).getDoubleValue();
            }
            return result;
        }
        public string[] getStringArray(GSCoreCollection C)
        {
            string[] result = new string[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = Parent.ExecuteElement(C[i]).getStringValue();
            }
            return result;
        }
        public bool[] getBooleanArray(GSCoreCollection C)
        {
            bool[] result = new bool[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = Parent.ExecuteElement(C[i]).getBooleanValue();
            }
            return result;
        }

        public GSCoreCollection getNativeValues(GSCoreCollection args)
        {
            GSCoreCollection coll = new GSCoreCollection();
            foreach (GSCore item in args)
            {
                coll.Add(Parent.ExecuteElement(item));
            }
            return coll;
        }

        private GSCore execGt(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getDoubleValue() > arg1[1].getDoubleValue());
            return bv;
        }

        private GSCore execGe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getDoubleValue() >= arg1[1].getDoubleValue());
            return bv;
        }

        private GSCore execEq(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getDoubleValue() == arg1[1].getDoubleValue());
            return bv;
        }

        private GSCore execNe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getDoubleValue() != arg1[1].getDoubleValue());
            return bv;
        }

        private GSCore execLe(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getDoubleValue() <= arg1[1].getDoubleValue());
            return bv;
        }

        private GSCore execLt(GSCoreCollection arg1)
        {
            GSBoolean bv = new GSBoolean();
            bv.BooleanValue = (arg1[0].getDoubleValue() < arg1[1].getDoubleValue());
            return bv;
        }

        private GSCore execDiv(GSCoreCollection args)
        {
            double[] arr = getDoubleArray(args);
            double sum = arr[0];
            for (int i = 1; i < arr.Length; i++)
                sum /= arr[i];
            return new GSDouble() { DoubleValue = sum };
        }

        private GSCore execMul(GSCoreCollection args)
        {
            double[] arr = getDoubleArray(args);
            double sum = 1.0;
            for (int i = 0; i < arr.Length; i++)
                sum *= arr[i];
            return new GSDouble() { DoubleValue = sum };
        }

        private GSCore execSub(GSCoreCollection args)
        {
            double[] arr = getDoubleArray(args);
            double sum = arr[0];
            for (int i = 1; i < arr.Length; i++)
                sum -= arr[i];
            return new GSDouble() { DoubleValue = sum };
        }

        private GSCore execAdd(GSCoreCollection args)
        {
            double[] arr = getDoubleArray(args);
            double sum = 0;
            for (int i = 0; i < arr.Length; i++)
                sum += arr[i];
            return new GSDouble() { DoubleValue = sum };
        }
    }
}
