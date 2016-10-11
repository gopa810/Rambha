using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SlideMaker.Document
{
    public class MNPageRuler
    {
        private static int rulerIdUnique = 1;

        private int rulerId = 0;

        public int HorizontalVertical { get; set; }

        public bool IsHorizontal
        {
            get { return HorizontalVertical == 0; }
            set { HorizontalVertical = (value ? 0 : 1); }
        }

        public bool IsVertical
        {
            get { return HorizontalVertical == 1; }
            set { HorizontalVertical = (value ? 1 : 0); }
        }

        public int AbsoluteRelative { get; set; }

        public bool IsAbsolute
        {
            get { return AbsoluteRelative == 0; }
            set { AbsoluteRelative = (value ? 0 : 1); }
        }

        public bool IsRelative
        {
            get { return AbsoluteRelative == 1; }
            set { AbsoluteRelative = (value ? 1 : 0); }
        }

        public string Name { get; set; }

        // if relative, then value 1000 means page width
        public int Value { get; set; }

        public MNPageRuler()
        {
            rulerId = rulerIdUnique++;
            IsHorizontal = true;
            IsRelative = true;
            Value = 0;
            Name = "Default";
        }

        public MNPageRuler(MNPageRuler p)
        {
            rulerId = rulerIdUnique++;
            IsHorizontal = p.IsHorizontal;
            IsAbsolute = p.IsAbsolute;
            Value = p.Value;
            Name = p.Name;
        }

        public void Paint(MNPageContext context)
        {
            if (IsHorizontal)
            {
                int y = Value;
                if (IsRelative)
                {
                    y = Convert.ToInt32(y / 1000.0 * context.PageHeight);
                }
                context.g.DrawLine(Pens.LightGray, 0, y, context.PageWidth, y);
            }
            else
            {
                int x = Value;
                if (IsRelative)
                {
                    x = Convert.ToInt32(x / 1000.0 * context.PageWidth);
                }
                context.g.DrawLine(Pens.LightGray, x, 0, x, context.PageHeight);
            }
        }

        public void SetRelativeX(int x)
        {
            IsVertical = true;
            IsRelative = true;
            Value = x;
        }

        public void SetRelativeY(int y)
        {
            IsHorizontal = true;
            IsRelative = true;
            Value = y;
        }

        public int GetAbsoluteValue(int pageDimension)
        {
            return IsRelative ? Convert.ToInt32(Value/1000.0*pageDimension) : Value;
        }

        public void AddValue(int pageDim, float change)
        {
            if (IsRelative)
            {
                Value += Convert.ToInt32(change / pageDim * 1000f);
            }
            else
            {
                Value += (int)change;
            }
        }
    }
}
