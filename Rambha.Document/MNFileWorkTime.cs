using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Document
{
    public class MNFileWorkTime
    {
        public string GetLastTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void SetTotalWorkTime(long val)
        {
            totalWorkTime = val;
            lastTotalWorkPoint = DateTime.Now;
        }

        public long GetTotalWorkTime()
        {
            DateTime dt = DateTime.Now;
            TimeSpan ts = dt - lastTotalWorkPoint;
            totalWorkTime += Convert.ToInt64(ts.TotalSeconds);
            lastTotalWorkPoint = dt;
            return totalWorkTime;
        }

        public long PeekSeconds()
        {
            return totalWorkTime;
        }

        private long totalWorkTime = 0;

        private DateTime lastTotalWorkPoint = DateTime.Now;
    }
}
