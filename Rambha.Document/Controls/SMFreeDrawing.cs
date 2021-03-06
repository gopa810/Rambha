﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMFreeDrawing: SMControl
    {
        public float PenWidth = 6.0f;
        public Color PenColor = Color.Black;
        public double minDistance = 20;

        private Color oldColor = Color.Transparent;

        private TempPoints tempPoints = new TempPoints();
        private List<DrawPoints> drawPoints = new List<DrawPoints>();

        [Browsable(true), Category("Content")]
        public MNReferencedImage BackgroundImage
        {
            get
            {
                if (p_image == null && image_id > 0)
                    p_image = Document.FindImage(image_id);
                return p_image;
            }
            set
            {
                p_image = value;
            }
        }
        public long BackgroundImageId
        {
            get { return p_image == null ? image_id : p_image.Id; }
            set { image_id = value; }
        }
        private MNReferencedImage p_image = null;
        private long image_id = -1;


        private Rectangle BackgroundImageRect = Rectangle.Empty; 


        public SMFreeDrawing(MNPage p)
            : base(p)
        {
            Text = "Free Paint";
            BackgroundImage = null;
            Draggable = SMDragResponse.None;
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(256,196);
        }

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                byte b;
                while ((b = br.ReadByte()) != 0)
                {
                    switch (b)
                    {
                        case 10:
                            BackgroundImageId = br.ReadInt64();
                            break;
                    }
                }
            }

            return true;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt64(BackgroundImageId);

            bw.WriteByte(0);
        }

        public override void ExportToHtml(MNExportContext ctx, int zorder, StringBuilder sbHtml, StringBuilder sbCss, StringBuilder sbJS)
        {
            ctx.AppendToResizeList("type", "canvaswh", "id", "can", "w", Area.RelativeArea.Width.ToString(), "h", Area.RelativeArea.Height.ToString());

            sbHtml.AppendFormat("<div style='position:absolute;z-index:{2};display:flex;flex-direction:row;left:{0}%;top:{1}%;'>\n", 25 * Area.RelativeArea.Left / 256, 25 * Area.RelativeArea.Top / 192, zorder);
            sbHtml.AppendFormat("<div style='display:flex;flex-direction:column'>\n");
            sbHtml.AppendFormat("    <div style=\"width:40px;height:30px;background:green;\" id=\"green\" onclick=\"freeDrawingColor(this)\"></div>\n");
            sbHtml.AppendFormat("    <div style=\"width:40px;height:30px;background:blue;\" id=\"blue\" onclick=\"freeDrawingColor(this)\"></div>\n");
            sbHtml.AppendFormat("    <div style=\"width:40px;height:30px;background:red;\" id=\"red\" onclick=\"freeDrawingColor(this)\"></div>\n");
            sbHtml.AppendFormat("    <div style=\"width:40px;height:30px;background:yellow;\" id=\"yellow\" onclick=\"freeDrawingColor(this)\"></div>\n");
            sbHtml.AppendFormat("    <div style=\"width:40px;height:30px;background:orange;\" id=\"orange\" onclick=\"freeDrawingColor(this)\"></div>\n");
            sbHtml.AppendFormat("    <div style=\"width:40px;height:30px;background:black;\" id=\"black\" onclick=\"freeDrawingColor(this)\"></div>\n");
            sbHtml.AppendFormat("    <div style=\"width:36px;height:30px;background:white;border:2px solid;\" id=\"white\" onclick=\"freeDrawingColor(this)\"></div>\n");
            sbHtml.AppendFormat("	<br>\n");
            sbHtml.AppendFormat("    <input type=\"button\" value=\"clear\" id=\"clr\" size=\"23\" onclick=\"freeDrawingErase()\" style=\"\">\n");
            sbHtml.AppendFormat("</div><div>\n");
            sbHtml.AppendFormat("    <canvas id=\"can\" width={1} height={2} style=\"z-index:{0};", zorder, Area.RelativeArea.Width, Area.RelativeArea.Height);
            if (BackgroundImage != null)
                sbHtml.AppendFormat("background-size:contain;background-repeat:no-repeat;background-position:center;background-image:url('{0}');", ctx.GetFileNameFromImage(BackgroundImage));
            sbHtml.AppendFormat("border:2px solid;\"></canvas></div></div>\n");
        }

        public override void Paint(MNPageContext context)
        {
            Rectangle bounds = Area.GetBounds(context);

            Rectangle textBounds = bounds;
            Pen currentPen;

            // background image is drawn centered
            if (BackgroundImage != null)
            {
                BackgroundImageRect = new Rectangle(textBounds.Location, SMGraphics.GetMaximumSize(textBounds, BackgroundImage.ImageData.Size));
                BackgroundImageRect.Offset((textBounds.Width - BackgroundImageRect.Width)/2,
                    (textBounds.Height - BackgroundImageRect.Height)/2);
                context.g.DrawImage(BackgroundImage.ImageData, BackgroundImageRect);
            }

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

                if (tempPoints.pts.Count > 1)
                {
                    currentPen = SMGraphics.GetPen(tempPoints.penColor, tempPoints.penWidth);
                    context.g.DrawLines(currentPen, tempPoints.pts.ToArray<Point>());
                }
            }

            // draw selection marks
            base.Paint(context);
        }

        public override void OnTapBegin(PVDragContext dc)
        {
            tempPoints.penColor = PenColor;
            tempPoints.penWidth = PenWidth;
            tempPoints.pts.Clear();
            Debugger.Log(0, "", "Temp Start\n");
            base.OnTapBegin(dc);
        }

        public override void OnTapMove(PVDragContext dc)
        {
            if (tempPoints.pts.Count > 0)
            {
                Point lp = tempPoints.pts[tempPoints.pts.Count - 1];
                if (Math.Sqrt((lp.X - dc.lastPoint.X)*(lp.X - dc.lastPoint.X) + (lp.Y - dc.lastPoint.Y)*(lp.Y - dc.lastPoint.Y)) >= minDistance)
                {
                    tempPoints.pts.Add(dc.lastPoint);
                    Debugger.Log(0, "", "Temp Move B " + tempPoints.pts.Count + "\n");
                }
            }
            else
            {
                tempPoints.pts.Add(dc.lastPoint);
                Debugger.Log(0, "", "Temp Move A " + tempPoints.pts.Count + "\n");
            }
            base.OnTapMove(dc);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            Debugger.Log(0, "", "Temp End " + tempPoints.pts.Count + "\n");
            drawPoints.Add(tempPoints.GetPoints());
            tempPoints.pts.Clear();
            base.OnTapEnd(dc);
        }

        public override void LoadStatus(RSFileReader br)
        {
            base.LoadStatusCore(br);
            byte b;
            drawPoints.Clear();
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 20:
                        DrawPoints dp = new DrawPoints();
                        dp.penWidth = br.ReadFloat();
                        dp.penColor = br.ReadColor();
                        int i = br.ReadInt32();
                        if (i == 0)
                            dp.pts = null;
                        else
                        {
                            dp.pts = new Point[i];
                            for (int j = 0; j < i; j++)
                            {
                                dp.pts[j] = new Point(br.ReadInt32(), br.ReadInt32());
                            }
                        }
                        break;
                }
            }
        }

        public override void SaveStatus(RSFileWriter bw)
        {
            base.SaveStatusCore(bw);

            foreach (DrawPoints dp in drawPoints)
            {
                bw.WriteByte(20);
                bw.WriteFloat(dp.penWidth);
                bw.WriteColor(dp.penColor);
                bw.WriteInt32(dp.pts != null ? dp.pts.Length : 0);
                for (int i = 0; i < dp.pts.Length; i++)
                {
                    bw.WriteInt32(dp.pts[i].X);
                    bw.WriteInt32(dp.pts[i].Y);
                }
            }

            bw.WriteByte(0);
        }

        public override void ResetStatus()
        {
            drawPoints.Clear();
            base.ResetStatus();
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
