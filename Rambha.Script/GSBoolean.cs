using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSBoolean: GSCore
    {
        public bool BooleanValue = false;

        public GSBoolean()
        {
        }

        public GSBoolean(bool b)
        {
            BooleanValue = b;
        }

        public override string getStringValue()
        {
            return BooleanValue ? "1" : "0";
        }

        public override long getIntegerValue()
        {
            return BooleanValue ? 1 : 0;
        }

        public override bool getBooleanValue()
        {
            return BooleanValue;
        }

        public override double getDoubleValue()
        {
            return BooleanValue ? 1.0 : 0.0;
        }

        public override void writeScript(int level, System.IO.StreamWriter sw)
        {
            sw.Write(getIntegerValue().ToString());
        }

    }
}
