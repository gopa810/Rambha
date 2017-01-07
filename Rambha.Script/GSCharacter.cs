using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSCharacter: GSCore
    {
        private Char CharValue = '\0';

        public GSCharacter()
        {
        }

        public GSCharacter(Char c)
        {
            CharValue = c;
        }

        public GSCharacter(int i)
        {
            CharValue = Convert.ToChar(i);
        }

        public GSCharacter(long l)
        {
            CharValue = Convert.ToChar(l);
        }

        public override string ToString()
        {
            return CharValue.ToString();
        }

        public override string getStringValue()
        {
            return CharValue.ToString();
        }

        public override long getIntegerValue()
        {
            return CharValue;
        }

        public override bool getBooleanValue()
        {
            return CharValue != 0;
        }

        public override double getDoubleValue()
        {
            return (double)CharValue;
        }

        public override void writeScript(int level, System.IO.StreamWriter sw)
        {
            sw.Write(CharValue.ToString());
        }
    }
}
