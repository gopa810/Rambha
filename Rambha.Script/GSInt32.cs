using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSInt32: GSCore
    {
        private Int32 Int32Value = 0;

        public GSInt32()
        {
        }

        public GSInt32(int i)
        {
            Int32Value = i;
        }

        public GSInt32(long l)
        {
            Int32Value = Convert.ToInt32(l);
        }

        public override string ToString()
        {
            return Int32Value.ToString();
        }

        public override string getStringValue()
        {
            return Int32Value.ToString();
        }

        public override long getIntegerValue()
        {
            return Int32Value;
        }

        public override bool getBooleanValue()
        {
            return Int32Value != 0;
        }

        public override double getDoubleValue()
        {
            return (double)Int32Value;
        }

        public override void writeScript(int level, System.IO.StreamWriter sw)
        {
            sw.Write(Int32Value.ToString());
        }
    }
}
