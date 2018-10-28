using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMSelection: SMControl
    {
        private string p_prevtext = "";

        private bool bHorizontal = false;

        private int p_expectedSelection = -1;

        private int p_currSelection = -1;

        private List<SelText> texts = new List<SelText>();

        private class SelText
        {
            public string text = "";
            public Size size = Size.Empty;
            public Rectangle drawRect = Rectangle.Empty;

            public SelText()
            {
            }

            public SelText(string s)
            {
                text = s;
            }
        }

        public SMSelection(MNPage p)
            : base(p)
        {

        }

        public int ExpectedValue
        {
            get { return p_expectedSelection; }
            set { p_expectedSelection = value; }
        }

        public override void Paint(MNPageContext context)
        {
            //Debugger.Log(0, "", "-- paint selection control -- Horizontal:" + bHorizontal + "\n");
            SMStatusLayout layout = PrepareBrushesAndPens();

            PrepareContent(context);

            Rectangle r = Area.GetBounds(context);

            Font font = GetUsedFont();
            Brush backBrush = null;
            Brush foreBrush = null;
            int radius = 15;

            if (bHorizontal)
            {
                int width = 1;
                foreach (SelText st in texts)
                {
                    width += st.size.Width;
                }

                int index = 0;
                int currPos = r.Left;
                foreach (SelText st in texts)
                {
                    int thisWidth = r.Width * st.size.Width / width;
                    layout = (p_currSelection == index ? SMGraphics.clickableLayoutH : SMGraphics.clickableLayoutN);
                    backBrush = SMGraphics.GetBrush(layout.BackColor);
                    foreBrush = SMGraphics.GetBrush(layout.ForeColor);
                    
                    if (index == 0)
                    {
                        context.g.FillEllipse(backBrush, r.Left, r.Top, radius * 2, radius * 2);
                        context.g.FillEllipse(backBrush, r.Left, r.Bottom - radius * 2, radius * 2, radius * 2);
                        context.g.FillRectangle(backBrush, r.Left, r.Top + radius, radius, r.Height - 2 * radius);
                    }
                    else
                    {
                        context.g.FillRectangle(backBrush, currPos, r.Top, radius, r.Height);
                    }

                    if (index == texts.Count - 1)
                    {
                        context.g.FillEllipse(backBrush, r.Right - radius*2, r.Top, radius * 2, radius * 2);
                        context.g.FillEllipse(backBrush, r.Right - radius*2, r.Bottom - radius * 2, radius * 2, radius * 2);
                        context.g.FillRectangle(backBrush, currPos + thisWidth - radius, r.Top + radius, 
                            r.Right - (currPos + thisWidth - radius), r.Height - 2 * radius);
                    }
                    else
                    {
                        context.g.FillRectangle(backBrush, currPos + thisWidth - radius, r.Top, radius, r.Height);
                    }

                    context.g.FillRectangle(backBrush, currPos + radius, r.Top, thisWidth - 2 * radius, r.Height);

                    if (index > 0)
                        context.g.DrawLine(tempForePen, currPos, r.Top, currPos, r.Bottom); 

                    Rectangle rt = new Rectangle();
                    rt.X = currPos;
                    rt.Y = r.Top;
                    rt.Width = thisWidth;
                    rt.Height = r.Height;
                    context.g.DrawString(st.text, font, foreBrush, rt, SMGraphics.StrFormatCenter);
                    st.drawRect = rt;

                    currPos += thisWidth;
                    index++;
                }
            }
            else
            {
                int height = 1;
                foreach (SelText st in texts)
                {
                    height += st.size.Height;
                }

                int index = 0;
                int currPos = r.Top;
                //Debugger.Log(0, "", "--- selection control ---\n");
                foreach (SelText st in texts)
                {
                    int thisHeight = r.Height * st.size.Height / height;
                    layout = (p_currSelection == index ? SMGraphics.clickableLayoutH : SMGraphics.clickableLayoutN);
                    backBrush = SMGraphics.GetBrush(layout.BackColor);
                    foreBrush = SMGraphics.GetBrush(layout.ForeColor);

                    if (index == 0)
                    {
                        context.g.FillEllipse(backBrush, r.Left, r.Top, radius * 2, radius * 2);
                        context.g.FillEllipse(backBrush, r.Right - radius * 2, r.Top, radius * 2, radius * 2);
                        context.g.FillRectangle(backBrush, r.Left + radius, r.Top, r.Width - 2 * radius, radius);
                    }
                    else
                    {
                        context.g.FillRectangle(backBrush, r.Left, currPos, r.Width, radius);
                    }

                    if (index == texts.Count - 1)
                    {
                        context.g.FillEllipse(backBrush, r.Left, r.Bottom - radius * 2, radius * 2, radius * 2);
                        context.g.FillEllipse(backBrush, r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2);
                        context.g.FillRectangle(backBrush, r.Left + radius, currPos + thisHeight - radius, 
                            r.Width - 2 * radius, r.Bottom - (currPos + thisHeight - radius));
                    }
                    else
                    {
                        context.g.FillRectangle(backBrush, r.Left, currPos + thisHeight - radius, r.Width, radius);
                    }

                    context.g.FillRectangle(backBrush, r.Left, currPos + radius, r.Width, thisHeight - 2 * radius);

                    if (index > 0)
                        context.g.DrawLine(tempForePen, r.Left, currPos, r.Right, currPos); 

                    Rectangle rt = new Rectangle();
                    rt.X = r.Left;
                    rt.Y = currPos;
                    rt.Width = r.Width;
                    rt.Height = thisHeight;
                    context.g.DrawString(st.text, font, foreBrush, rt, SMGraphics.StrFormatCenter);
                    st.drawRect = rt;

                    currPos += thisHeight;
                    index++;
                }
            }

            context.g.DrawRoundedRectangle(tempForePen, r, radius);

            // in case this is wrongly checked, run clearing the state in 2 secs
            if (UIStateError == MNEvaluationResult.Incorrect && HasImmediateEvaluation)
            {
                if (Document.HasViewer)
                {
                    Document.Viewer.ScheduleCall(MNNotificationCenter.RectifyDelay, this, "clearCheck");
                }
            }

            base.Paint(context);
        }


        public override void ExportToHtml(MNExportContext ctx, int zorder, StringBuilder sbHtml, StringBuilder sbCss, StringBuilder sbJS)
        {
            PrepareContent();
            bool horz = bHorizontal;
            string blockFormat = Font.HtmlString() + Paragraph.Html() + ContentPaddingHtml();
            string dimensions = string.Format("height:{0}%;width:{1}%", horz ? 100 : 100 / texts.Count, horz ? 100 / texts.Count : 100);

            sbHtml.Append("<div ");
            sbHtml.AppendFormat(" id=\"c{0}\" ", this.Id);
            sbHtml.AppendFormat(" style ='display:flex;flex-direction:{1};position:absolute;z-index:{0};", zorder,
                horz ? "row" : "column");
            SMRectangleArea area = this.Area;
            sbHtml.Append(area.HtmlLTRB());
            sbHtml.Append("'>\n");
            int i = 0;
            string[] radiustextH = { "15px 0px 0px 15px", "0px", "0px 15px 15px 0px" };
            string[] radiustextV = { "15px 15px 0px 0px", "0px", "0px 0px 15px 15px" };
            foreach (SelText si in texts)
            {
                int ridx = (i == 0 ? 0 : i == (texts.Count - 1) ? 2 : 1);
                sbHtml.AppendFormat("<div id=\"sel{0}_{2}\" class=\"csSelectN\" style='cursor:pointer;border-radius:{1};{3};' onclick=\"toogleSelectCtrl({0},{2})\">\n", 
                    Id, bHorizontal ? radiustextH[ridx] : radiustextV[ridx], i, dimensions);

                sbHtml.AppendFormat("<div class=\"vertCenter\" style='text-align:center'><div>\n");
                sbHtml.AppendFormat("{0}", si.text);
                sbHtml.AppendFormat("</div></div>");

                sbHtml.AppendFormat("</div>\n");
                i++;
            }
            //sbHtml.Append("background:lightyellow;border:1px solid black;'>");
            //sbHtml.Append("<b>" + GetType().Name + "</b><br>" + this.Text);
            sbHtml.Append("</div>\n");

            ctx.AppendToControlList("type", "select", "segments", texts.Count.ToString(), "correct", p_expectedSelection.ToString(),
                "id", Id.ToString(), "current", "0");

        }


        private string[] p_strSep = { "\n\n" };

        private int p_prevSel = -1;

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                byte tag;
                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10:
                            ExpectedValue = br.ReadInt32();
                            break;
                        default:
                            return false;
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
            bw.WriteInt32(ExpectedValue);

            bw.WriteByte(0);
        }

        public override void OnTapBegin(PVDragContext dc)
        {
            p_prevSel = p_currSelection;
            int index = 0;
            foreach (SelText st in texts)
            {
                if (st.drawRect.Contains(dc.startPoint))
                {
                    p_currSelection = index;
                    break;
                }
                index++;
            }

            base.OnTapBegin(dc);
        }

        public override void OnTapCancel(PVDragContext dc)
        {
            p_currSelection = p_prevSel;
            base.OnTapCancel(dc);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            int index = 0;
            foreach (SelText st in texts)
            {
                if (st.drawRect.Contains(dc.startPoint))
                {
                    if (p_currSelection != index)
                    {
                        p_currSelection = p_prevSel;
                    }
                    break;
                }
                index++;
            }

            Evaluate();

            base.OnTapEnd(dc);
        }

        public override MNEvaluationResult Evaluate()
        {
            UIStateError = MNEvaluationResult.NotEvaluated;

            if (p_currSelection >= 0 && p_expectedSelection >= 0)
            {
                UIStateError = (p_currSelection == p_expectedSelection) ? MNEvaluationResult.Correct : MNEvaluationResult.Incorrect;
            }

            return UIStateError;
        }

        public void PrepareContent()
        {
            p_prevtext = Text;
            texts.Clear();
            string[] p = null;
            int index = 0;
            string s = Text.Replace("\r\n", "\n");
            //Debugger.Log(0, "", "PrepareSelectionContents: " + Text + "<< end cont<<\n");
            if (s.IndexOf("\n\n") >= 0)
            {
                bHorizontal = false;
                p = s.Split(p_strSep, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                bHorizontal = true;
                p = s.Split('|');
            }

            if (p != null)
            {
                foreach (string line in p)
                {
                    if (line.StartsWith("*"))
                    {
                        p_expectedSelection = index;
                        texts.Add(new SelText(line.Substring(1)));
                    }
                    else
                    {
                        texts.Add(new SelText(line));
                    }
                    index++;
                }
            }
        }

        public void PrepareContent(MNPageContext context)
        {
            if (p_prevtext.Equals(Text))
                return;

            p_prevtext = Text;
            texts.Clear();
            string[] p = null;
            int index = 0;
            string s = Text.Replace("\r\n", "\n");
            //Debugger.Log(0, "", "PrepareSelectionContents: " + Text + "<< end cont<<\n");
            if (s.IndexOf("\n\n") >= 0)
            {
                bHorizontal = false;
                p = s.Split(p_strSep, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                bHorizontal = true;
                p = s.Split('|');
            }

            if (p != null)
            {
                foreach (string line in p)
                {
                    if (line.StartsWith("*"))
                    {
                        p_expectedSelection = index;
                        texts.Add(new SelText(line.Substring(1)));
                    }
                    else
                    {
                        texts.Add(new SelText(line));
                    }
                    index++;
                }

                Font font = GetUsedFont();

                foreach (SelText st in texts)
                {
                    SizeF sf = context.g.MeasureString(st.text, font);
                    st.size.Width = (int)sf.Width;
                    st.size.Height = (int)sf.Height;
                }
            }
        }

        public override Script.GSCore ExecuteMessage(string token, Script.GSCoreCollection args)
        {
            if (token.Equals("clearCheck"))
            {
                ResetStatus();
            }

            return base.ExecuteMessage(token, args);
        }

        public override void ResetStatus()
        {
            p_currSelection = -1;
            base.ResetStatus();
        }
    }
}
