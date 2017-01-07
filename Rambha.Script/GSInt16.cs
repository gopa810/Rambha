using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSInt16: GSCore
    {
        private Int16 Int16Value = 0;

        public GSInt16()
        {
        }

        public GSInt16(int i)
        {
            Int16Value = Convert.ToInt16(i);
        }

        public GSInt16(long l)
        {
            Int16Value = Convert.ToInt16(l);
        }

        public override string ToString()
        {
            return Int16Value.ToString();
        }

        public override string getStringValue()
        {
            return Int16Value.ToString();
        }

        public override long getIntegerValue()
        {
            return Int16Value;
        }

        public override bool getBooleanValue()
        {
            return Int16Value != 0;
        }

        public override double getDoubleValue()
        {
            return (double)Int16Value;
        }

        public override void writeScript(int level, System.IO.StreamWriter sw)
        {
            sw.Write(Int16Value.ToString());
        }
    }
}
