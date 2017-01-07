using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    public class GVGraphAction: GVGraphObject
    {
        private static List<GVTrackerBase> ps_trackers = null;

        public GVDeclarationProcedure Action { get; set; }

        private float p_titleheight = 0;
        private float p_titlewidth = 0;
        private float p_row1width = 0;
        private float p_row2width = 0;
        private float p_rowheight = 0;

        static GVGraphAction()
        {
            ps_trackers = new List<GVTrackerBase>();
            ps_trackers.Add(new GVTrackerMoving());
            ps_trackers.Add(new GVTrackerControlOut());
        }

        public GVGraphAction(GVGraph g): base(g)
        {
            Autosize = true;
            Action = null;
        }

        public override List<GVTrackerBase> getTrackers()
        {
            return ps_trackers;
        }

        public override string GetTitle()
        {
            return Action != null ? Action.Name : "<action>";
        }

        public override void Load(RSFileReader R)
        {
            base.Load(R);

            byte tag;
            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 20:
                        Action = new GVDeclarationProcedure();
                        Action.Load(R);
                        break;
                }
            }
        }

        public override void Save(RSFileWriter W)
        {
            base.Save(W);

            if (Action != null)
            {
                W.WriteByte(20);
                Action.Save(W);
            }

            W.WriteByte(0);
        }

        public override SizeF GetBoundsSize(GVGraphViewContext context)
        {
            if (Action == null)
            {
                return context.Graphics.MeasureString("<action>", SystemFonts.MenuFont);
            }
            else
            {
                float r1 = 16, r2 = 16;
                float rh = 0;
                SizeF cellSize = SizeF.Empty;
                SizeF titleSize = context.Graphics.MeasureString(Action.Name, GVGraphics.ActionTitleFont);
                int count = Action.ParametersCount;
                for (int a = 0; a < count; a++)
                {
                    GVDeclarationDataEntry ap= Action.GetParameter(a);
                    cellSize = context.Graphics.MeasureString(ap.Name, GVGraphics.ActionPropertyFont);
                    r1 = Math.Max(r1, cellSize.Width + 12);
                    rh = Math.Max(rh, cellSize.Height);
                    cellSize = context.Graphics.MeasureString(ap.Value, GVGraphics.ActionPropertyFont);
                    r2 = Math.Max(r2, cellSize.Width + 12);
                    rh = Math.Max(rh, cellSize.Height);
                }

                if (r1 + r2 > titleSize.Width + 8)
                    p_titlewidth = r1 + r2;
                else
                {
                    p_titlewidth = titleSize.Width + 8;
                    float d = (p_titlewidth - r1 - r2) / 2;
                    r1 += d;
                    r2 += d;
                }
                p_titleheight = titleSize.Height + 12;
                p_row1width = r1;
                p_row2width = r2;
                p_rowheight = rh + 8;

                return new SizeF(p_titlewidth, p_titleheight + p_rowheight*count + 4);
            }
        }

        public override void PaintContent(GVGraphViewContext context, RectangleF rect, Pen penBorder)
        {
            context.Graphics.DrawRectangle(penBorder, rect.X, rect.Y, rect.Width, rect.Height);
            if (Action != null)
            {
                context.Graphics.DrawString(Action.Name, GVGraphics.ActionTitleFont, Brushes.Black, rect.X + 4, rect.Y + 4);
                for (int i = 0; i < Action.ParametersCount; i++)
                {
                    GVDeclarationDataEntry ap = Action.GetParameter(i);
                    context.Graphics.DrawLine(Pens.LightGray, rect.X + 4, rect.Y + p_titleheight + i * p_rowheight,
                        rect.X + p_titlewidth - 4, rect.Y + p_titleheight + i * p_rowheight);
                    context.Graphics.DrawString(ap.Name, GVGraphics.ActionPropertyFont, Brushes.Black, rect.X + 8, rect.Y + p_titleheight + i * p_rowheight + 4);
                    context.Graphics.DrawString(ap.Value, GVGraphics.ActionPropertyFont, Brushes.DarkGreen, rect.X + p_row1width + 4, rect.Y + p_titleheight + i * p_rowheight + 4);
                }
                context.Graphics.DrawLine(Pens.LightGray, rect.X + 4, rect.Y + p_titleheight + Action.ParametersCount * p_rowheight, 
                    rect.X + p_titlewidth - 4, rect.Y + p_titleheight + Action.ParametersCount * p_rowheight);
                context.Graphics.DrawLine(Pens.LightGray, rect.X + 4, rect.Y + p_titleheight, rect.X + 4,
                    rect.Y + p_titleheight + Action.ParametersCount * p_rowheight);
                context.Graphics.DrawLine(Pens.LightGray, rect.X + p_row1width, rect.Y + p_titleheight,
                    rect.X + p_row1width, rect.Y + p_titleheight + Action.ParametersCount * p_rowheight);
                context.Graphics.DrawLine(Pens.LightGray, rect.X + p_titlewidth - 4, rect.Y + p_titleheight,
                    rect.X + p_titlewidth - 4, rect.Y + p_titleheight + Action.ParametersCount * p_rowheight);
            }
            else
            {
                context.Graphics.DrawString("<action>", GVGraphics.ActionTitleFont, Brushes.Black, rect.X + 4, rect.Y + 4);
            }
            //base.PaintContent(context, rect, penBorder);
        }

        public int IndexOfPropertyFromPoint(PointF point)
        {
            if (Action != null)
            {
                if (point.Y < p_titleheight) return -1;
                if (Action.ParametersCount * p_rowheight + p_titleheight < point.Y) return -1;
                return Convert.ToInt32((point.Y - p_titleheight) / p_rowheight);
            }
            else
                return -1;
        }

        public RectangleF RectangleOfPropertyValue(int index)
        {
            return new RectangleF(PaintedRect.X + p_row1width, PaintedRect.Y + p_titleheight + index * p_rowheight,
                p_row2width, p_rowheight);
        }

        public override bool AcceptsTracker(GVTrackerBase tracker)
        {
            if (tracker is GVTrackerControlOut) return true;
            if (tracker is GVTrackerDataOut) return true;
            return false;
        }

        public override GVDeclarationFlowOut getControlOutNaming()
        {
            if (Action != null)
                return Action.OutNaming;
            return null;
        }

        public override GVDeclarationDataEntry[] getDataProperties()
        {
            return Action != null ? Action.getDataProperties() : null;
        }
    }
}
