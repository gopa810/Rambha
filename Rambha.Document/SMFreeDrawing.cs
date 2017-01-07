using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rambha.Document
{
    public class SMFreeDrawing: SMControl
    {
        public float PenWidth = 10.0f;
        public Color PenColor = Color.Black;

        private float oldWidth = 0;
        private Color oldColor = Color.Transparent;

        private TempPoints tempPoints = new TempPoints();
        private List<DrawPoints> drawPoints = new List<DrawPoints>();

        public SMFreeDrawing(MNPage p)
            : base(p)
        {
            Text = "Free Paint";
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
            Pen currentPen;

            /*context.g.DrawRectangle(Pens.Gray, textBounds.Right - 16, textBounds.Bottom - 48, 16, 16);
            context.g.DrawRectangle(Pens.Gray, textBounds.Right - 16, textBounds.Bottom - 32, 16, 16);
            context.g.DrawRectangle(Pens.Gray, textBounds.Right - 16, textBounds.Bottom - 16, 16, 16);*/
            context.g.DrawRectangle(Pens.Black, textBounds);

            if (!context.drawSelectionMarks)
            {
                foreach (DrawPoints dp in drawPoints)
                {
                    currentPen = SMGraphics.GetPen(dp.penColor, dp.penWidth);
                    context.g.DrawLines(currentPen, dp.pts);
                }

                currentPen = SMGraphics.GetPen(tempPoints.penColor, tempPoints.penWidth);
                context.g.DrawLines(currentPen, tempPoints.pts.ToArray<Point>());
            }

            // draw selection marks
            base.Paint(context);
        }

        public override void OnTapBegin(PVDragContext dc)
        {
            tempPoints.penColor = PenColor;
            tempPoints.penWidth = PenWidth;
            tempPoints.pts.Clear();
            base.OnTapBegin(dc);
        }

        public override void OnTapMove(PVDragContext dc)
        {
            tempPoints.pts.Add(dc.lastPoint);
            base.OnTapMove(dc);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            drawPoints.Add(tempPoints.GetPoints());
            tempPoints.pts.Clear();
            base.OnTapEnd(dc);
        }

        private class TempPoints
        {
            public float penWidth = 1.0f;
            public Color penColor = Color.Black;
            public List<Point> pts = new List<Point>();

            public DrawPoints GetPoints()
            {
                DrawPoints dp = new DrawPoints();
                dp.penWidth = penWidth;
                dp.penColor = penColor;
                dp.pts = pts.ToArray<Point>();
                return dp;
            }
        }

        public class DrawPoints
        {
            public float penWidth = 1.0f;
            public Color penColor = Color.Black;
            public Point[] pts = null;
        }
    }
}
