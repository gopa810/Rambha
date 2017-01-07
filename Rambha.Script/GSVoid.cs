using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSVoid : GSCore
    {
        /// <summary>
        /// static value of VOID
        /// </summary>
        private static GSVoid voidValue = null;

        /// <summary>
        /// Publicly available property for VOID contant
        /// </summary>
        public static GSCore Void
        {
            get
            {
                if (voidValue == null)
                    voidValue = new GSVoid();
                return voidValue;
            }
        }
    }
}
