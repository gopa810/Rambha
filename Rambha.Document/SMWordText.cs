using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordText : SMWordTextBase
    {
        public string text = string.Empty;
        public SMWordText(SMFont f)
            : base(f)
        {
        }

        public SMWordText(SMFont f, SMTokenItem item)
            : base(f)
        {
            text = item.Text ?? text;
            tag = item.Tag;
            TextBrush = null;
        }

        public override void Paint(MNPageContext context, SMStatusLayout layout, int X, int Y)
        {
            //context.g.FillRectangle(Brushes.LightGreen, rect);
            context.g.DrawString(this.text, this.WinFont, GetCurrentTextBrush(layout), rect.X + X, rect.Y + Y + this.lineOffset * rect.Height / 100);
        }

        public override bool IsSpace()
        {
            return text.Equals(" ");
        }

        public override SMTokenItem GetDraggableItem()
        {
            SMTokenItem item = new SMTokenItem();
            item.Tag = SimpleText(tag);
            item.Text = SimpleText(text);
            item.ContentSize = Size.Empty;
            return item;
        }

        public string SimpleText(string s)
        {
            while (s.Length > 0 && Char.IsPunctuation(s[s.Length - 1]))
            {
                s = s.Substring(0,s.Length - 1);
            }

            while (s.Length > 0 && Char.IsPunctuation(s[0]))
            {
                s = s.Substring(1);
            }

            return s;
        }

    }
}
