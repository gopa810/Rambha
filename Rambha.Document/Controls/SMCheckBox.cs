using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Text;
using System.IO;

using Rambha.Serializer;


namespace Rambha.Document
{
    public class SMCheckBox: SMControl
    {
        [Browsable(false)]
        public bool Status { get { return UIStateChecked; } set { UIStateChecked = value; } }

        [Browsable(true)]
        public bool CheckBoxAtEnd { get; set; }

        private SMRichText richText = null;

        public SMCheckBox(MNPage p)
            : base(p)
        {
            Text = "Check Box";
            Evaluation = MNEvaluationType.Inherited;
            CheckBoxAtEnd = false;
            richText = new SMRichText(this);
        }

        public override void TextDidChange()
        {
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(256,32);
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
                        case 10: DefaultChecked = (Bool3)br.ReadByte();
                            ExpectedChecked = (Bool3)br.ReadByte();
                            break;
                        case 11:
                            CheckBoxAtEnd = br.ReadBool();
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
            bw.WriteByte((byte)DefaultChecked);
            bw.WriteByte((byte)ExpectedChecked);

            bw.WriteByte(11);
            bw.WriteBool(CheckBoxAtEnd);

            bw.WriteByte(0);
        }

        public override void Paint(MNPageContext context)
        {
            Rectangle bounds = Area.GetBounds(context);

            SMStatusLayout layout = PrepareBrushesAndPens();

            Rectangle textBounds = ContentPadding.ApplyPadding(bounds);

            if (Text.Length == 0)
            {
                bool b = UIStatePressed;
                UIStatePressed |= UIStateChecked;
                DrawStyledBackground(context, textBounds);
                DrawStyledBorder(context, layout, textBounds);
                UIStatePressed = b;
            }
            else
            {

                Font font = GetUsedFont();

                SizeF cbSize = context.g.MeasureString("M", font);
                int inpad = (int)(cbSize.Height / 8);
                int inpad2 = inpad / 2;
                int height = (int)(cbSize.Height * 3 / 4);


                SizeF sf = richText.MeasureString(context, Text, textBounds.Width - height - 2*inpad);
                Size textSize = new Size((int)sf.Width + 5, (int)sf.Height);

                Pen drawPen = (UIStateError == MNEvaluationResult.Incorrect ? Pens.Red : tempForePen);

                Rectangle rectCB = textBounds;
                if (CheckBoxAtEnd)
                {
                    rectCB = new Rectangle(textBounds.X + textSize.Width + inpad, rectCB.Top + inpad, height, height);
                    textBounds.Size = textSize;
                }
                else
                {
                    rectCB = new Rectangle(rectCB.Left + inpad, rectCB.Top + inpad, height, height);
                    textBounds.Size = textSize;
                    textBounds.X += height + 2 * inpad + ContentPadding.LeftRight;
                }

                if (Status)
                {
                    context.g.DrawFillRoundedRectangle(SMGraphics.GetPen(layout.ForeColor, 1), SMGraphics.GetBrush(layout.BackColor), rectCB, 5);
                }
                else
                {
                    context.g.DrawRoundedRectangle(SMGraphics.GetPen(layout.ForeColor, 1), rectCB, 5);
                }

                if (Clickable)
                    richText.DrawString(context, SMGraphics.clickableLayoutN, Text, textBounds);
                else
                    richText.DrawString(context, NormalState, Text, textBounds);
            }

            // in case this is wrongly checked, run clearing the state in 2 secs
            if (UIStateError == MNEvaluationResult.Incorrect && UIStateChecked)
            {
                if (Document.HasViewer)
                {
                    Document.Viewer.ScheduleCall(MNNotificationCenter.RectifyDelay, this, "clearCheck");
                }
            }


            // draw selection marks
            base.Paint(context, false);
        }


        public override void OnClick(PVDragContext dc)
        {
            Status = !Status;

            if (HasImmediateEvaluation)
            {
                Evaluate();
            }

            base.OnClick(dc);
        }

        public override MNEvaluationResult Evaluate()
        {
            UIStateError = MNEvaluationResult.NotEvaluated;

            if (HasImmediateEvaluation || HasLazyEvaluation)
            {
                UIStateError = GenericCheckedEvaluation();
            }
            return UIStateError;
        }

        public override void DisplayAnswers()
        {
            Status = (ExpectedChecked == Bool3.True || ExpectedChecked == Bool3.Both);
        }

        public override void LoadStatus(RSFileReader br)
        {
            base.LoadStatusCore(br);

            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 20:
                        Status = br.ReadBool();
                        break;
                }
            }
        }

        public override void SaveStatus(RSFileWriter bw)
        {
            base.SaveStatusCore(bw);

            bw.WriteByte(20);
            bw.WriteBool(Status);

            bw.WriteByte(0);
        }

        public override void ResetStatus()
        {
            base.ResetStatus();
        }
    }
}
