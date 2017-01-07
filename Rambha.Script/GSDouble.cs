using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSDouble: GSCore
    {
        public double DoubleValue = 0.0;

        public GSDouble() { }

        public GSDouble(double d)
        {
            DoubleValue = d;
        }

        public override string ToString()
        {
            return DoubleValue.ToString();
        }

        public override long getIntegerValue()
        {
            return Convert.ToInt64(DoubleValue);
        }

        public override bool getBooleanValue()
        {
            return DoubleValue != 0.0;
        }

        public override double getDoubleValue()
        {
            return DoubleValue;
        }

        public override void writeScript(int level, System.IO.StreamWriter sw)
        {
            sw.Write(DoubleValue.ToString());
        }

    }
}
