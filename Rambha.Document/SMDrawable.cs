using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMDrawable: SMControl
    {
        [Browsable(true), Category("Content")]
        public string Drawings { get; set; }

        public SMDrawable(MNPage p)
            : base(p)
        {
            Text = "Drawable";
            Drawings = "";
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(256,196);
        }

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            Rectangle textBounds = bounds;

            if (Drawings.Length == 0)
            {
                context.g.DrawRectangle(Pens.Gray, textBounds);
                context.g.DrawLine(Pens.LightGray, textBounds.Left, textBounds.Bottom, textBounds.Right, textBounds.Top);
                context.g.DrawLine(Pens.LightGray, textBounds.Left, textBounds.Top, textBounds.Right, textBounds.Bottom);
            }

            // draw selection marks
            base.Paint(context);
        }

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                byte tag;
                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10: Drawings = br.ReadString(); break;
                        default: return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteString(Drawings);

            bw.WriteByte(0);
        }
    }
}
