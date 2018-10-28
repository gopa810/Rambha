using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;
using Rambha.Script;

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
            Rectangle bounds = Area.GetBounds(context);

            Rectangle textBounds = bounds;

            if (Drawings.Length == 0)
            {
                context.g.DrawRectangle(Pens.Gray, textBounds);
                context.g.DrawLine(Pens.LightGray, textBounds.Left, textBounds.Bottom, textBounds.Right, textBounds.Top);
                context.g.DrawLine(Pens.LightGray, textBounds.Left, textBounds.Top, textBounds.Right, textBounds.Bottom);
            }
            else
            {
                DrawingContext dc = new DrawingContext();
                dc.Graphics = context.g;
                dc.Bounds = bounds;
                string[] lines = Drawings.Split('\r', '\n', ';');
                foreach (string line in lines)
                {
                    dc.lineparts = line.Split(' ');
                    dc.PaintShape();
                }
            }

            // draw selection marks
            base.Paint(context);
        }

        public override void ExportToHtml(MNExportContext ctx, int zorder, StringBuilder sbHtml, StringBuilder sbCss, StringBuilder sbJS)
        {
            sbHtml.Append("<div ");
            sbHtml.AppendFormat(" id=\"c{0}\" ", this.Id);
            sbHtml.AppendFormat(" style ='position:absolute;z-index:{0};", zorder);
            SMRectangleArea area = this.Area;
            sbHtml.Append(area.HtmlLTRB());
            sbHtml.Append("'>");

            DrawingContext dc = new DrawingContext();
            string[] lines = Drawings.Split('\r', '\n', ';');
            foreach (string line in lines)
            {
                string[] lp = line.Split(' ');
                if (lp.Length == 0)
                    return;
                if (lp[0] != "line") continue;
                if (lp[1] == lp[3])
                {
                    sbHtml.AppendFormat("<div style='position:relative;top:{0}%;height:{1}%;left:{2}%;width:2;background:black;'></div>", lp[2], lp[4], lp[1]);
                }
                else if (lp[2] == lp[4])
                {
                    sbHtml.AppendFormat("<div style='position:relative;left:{0}%;width:{1}%;top:{2}%;height:2;background:black;'></div>", lp[1], lp[3], lp[2]);
                }
            }

            sbHtml.Append("</div>\n");
        }

        protected override GSCore ExecuteMessageSet(GSCore a1, GSCore a2, GSCoreCollection args)
        {
            string s = a1.getStringValue();
            if (s.Equals("drawings"))
            {
                Drawings = a2.getStringValue();
                return a2;
            }

            return base.ExecuteMessageSet(a1, a2, args);
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

        public class DrawingContext
        {
            public Graphics Graphics = null;
            public Pen CurrentPen = Pens.Black;
            public Rectangle Bounds;
            public string[] lineparts = null;

            public float GetFloat(int index)
            {
                if (lineparts == null || lineparts.Length <= index)
                    return 0;
                float f;
                if (float.TryParse(lineparts[index], out f))
                {
                    return f;
                }
                return 0;
            }

            public float GetAbsX(int i)
            {
                return Bounds.Left + GetFloat(i) / 100f * Bounds.Width;
            }
            public float GetAbsY(int i)
            {
                return Bounds.Top + GetFloat(i) / 100f * Bounds.Height;
            }

            public void PaintShape()
            {
                DrawingContext dc = this;
                if (dc.lineparts.Length == 0)
                    return;
                switch (dc.lineparts[0])
                {
                    case "line":
                        dc.Graphics.DrawLine(dc.CurrentPen, dc.GetAbsX(1), dc.GetAbsY(2),
                            dc.GetAbsX(3), dc.GetAbsY(4));
                        break;
                }
            }

        }
    }
}
