using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSInt64: GSCore
    {
        public Int64 Int64Value = 0L;

        public GSInt64()
        {
        }

        public GSInt64(int i)
        {
            Int64Value = i;
        }

        public GSInt64(long l)
        {
            Int64Value = l;
        }

        public override string ToString()
        {
            return Int64Value.ToString();
        }

        public override string getStringValue()
        {
            return Int64Value.ToString();
        }

        public override long getIntegerValue()
        {
            return Int64Value;
        }

        public override bool getBooleanValue()
        {
            return Int64Value != 0;
        }

        public override double getDoubleValue()
        {
            return (double)Int64Value;
        }

        public override void writeScript(int level, System.IO.StreamWriter sw)
        {
            sw.Write(Int64Value.ToString());
        }
    }
}
