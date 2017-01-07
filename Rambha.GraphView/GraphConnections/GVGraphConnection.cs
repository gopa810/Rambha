using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    public class GVGraphConnection: IRSObjectOrigin
    {
        public long Id = -1;
        public GVGraph Graph { get; set; }
        public GVGraphObject Source { get; set; }
        public GVGraphObject Target { get; set; }
        public string Title { get; set; }
        public float ArrowSize = 0;
        public Pen LinePen = Pens.Black;
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public RectangleF TitleRectangle = RectangleF.Empty;

        public GVGraphConnection(GVGraph g)
        {
            this.Graph = g;
        }

        public virtual void Paint(GVGraphViewContext context)
        {
            RectangleF rectA = Source.PaintedRect;
            RectangleF rectB = Target.PaintedRect;
            RectSide rsa, rsb;
            PointF border1 = GVGraphics.GetConnectorPoint(rectA, GVGraphics.GetCenterPoint(rectB), out rsa);
            PointF border2 = GVGraphics.GetConnectorPoint(rectB, GVGraphics.GetCenterPoint(rectA), out rsb);
            context.Graphics.DrawLine(LinePen, border1, border2);
            if (ArrowSize > 0)
                GVGraphics.DrawArrowEnd(context.Graphics, border1, border2, LinePen.Color, ArrowSize);


            TitleRectangle = GetRectangleForSize(context.Graphics.MeasureString(Title, SystemFonts.MenuFont),
                GVGraphics.GetCenterPoint(border1, border2));
            context.Graphics.FillRectangle(SystemBrushes.Control, TitleRectangle.X, TitleRectangle.Y,
                TitleRectangle.Width, TitleRectangle.Height);
            context.Graphics.DrawString(Title, SystemFonts.MenuFont, Brushes.Black, TitleRectangle.Location);
        }

        public RectangleF GetTitleRectangle()
        {
            if (TitleRectangle.IsEmpty)
            {
                return GetRectangleForSize(new SizeF(96,18));
            }
            else
            {
                return TitleRectangle;
            }
        }

        private RectangleF GetRectangleForSize(SizeF sz)
        {
            if (Source == null || Target == null)
                return RectangleF.Empty;
            PointF p1 = GVGraphics.GetCenterPoint(Source.PaintedRect);
            PointF p2 = GVGraphics.GetCenterPoint(Target.PaintedRect);

            return new RectangleF((p1.X + p2.X - sz.Width) / 2, (p1.Y + p2.Y - sz.Height) / 2,
                sz.Width, sz.Height);
        }

        private RectangleF GetRectangleForSize(SizeF sz, PointF centerPoint)
        {
            if (Source == null || Target == null)
                return RectangleF.Empty;

            return new RectangleF(centerPoint.X - sz.Width / 2, centerPoint.Y - sz.Height / 2,
                sz.Width, sz.Height);
        }

        void IRSObjectOrigin.setReference(int tag, object obj)
        {
            switch (tag)
            {
                case 11:
                    Source = (GVGraphObject)obj;
                    break;
                case 12:
                    Target = (GVGraphObject)obj;
                    break;
            }
        }

        public virtual void Load(RSFileReader R)
        {
            byte tag;
            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        Id = R.ReadInt64();
                        break;
                    case 11:
                        R.AddReference(Graph, "GraphObject", R.ReadInt64(), 11, this);
                        break;
                    case 12:
                        R.AddReference(Graph, "GraphObject", R.ReadInt64(), 12, this);
                        break;
                    case 20:
                        Title = R.ReadString();
                        break;
                    case 25:
                        string key = R.ReadString();
                        string val = R.ReadString();
                        Attributes.Add(key, val);
                        break;
                }
            }
        }

        public virtual void Save(RSFileWriter W)
        {
            W.WriteByte(10);
            W.WriteInt64(Id);

            W.WriteByte(11);
            W.WriteInt64(Source != null ? Source.Id : -1);

            W.WriteByte(12);
            W.WriteInt64(Target != null ? Target.Id : -1);

            W.WriteByte(20);
            W.WriteString(Title);

            if (Attributes != null)
            {
                foreach (KeyValuePair<string, string> kvp in Attributes)
                {
                    W.WriteByte(25);
                    W.WriteString(kvp.Key);
                    W.WriteString(kvp.Value);
                }
            }

            W.WriteByte(0);
        }
    }

}
