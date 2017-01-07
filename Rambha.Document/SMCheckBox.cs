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
        [Browsable(true), Category("Content")]
        public bool DefaultStatus { get; set; }

        [Browsable(false)]
        public bool Status { get; set; }

        [Browsable(true), Category("Evaluation")]
        public bool ExpectedStatus { get; set; }

        public SMCheckBox(MNPage p)
            : base(p)
        {
            Text = "Check Box";
            Evaluation = MNEvaluationType.Inherited;
            ExpectedStatus = false;
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
                        case 10: DefaultStatus = br.ReadBool();
                            ExpectedStatus = br.ReadBool();
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
            bw.WriteBool(DefaultStatus);
            bw.WriteBool(ExpectedStatus);

            bw.WriteByte(0);
        }

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = Page.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            PrepareBrushesAndPens();

            Rectangle textBounds = Style.ApplyPadding(bounds);

            SizeF sf = context.g.MeasureString(Text, Style.Font);
            Size textSize = new Size((int)sf.Width + 5, (int)sf.Height);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Near;

            int inpad = textSize.Height / 8;
            int height = textSize.Height * 3 / 4;

            Pen drawPen = (UIStateError == MNEvaluationResult.Incorrect ? Pens.Red : tempForePen);

            context.g.DrawRectangle(drawPen, textBounds.X, textBounds.Y + inpad, height, height);
            if (Status)
            {
                context.g.DrawLine(drawPen, textBounds.X + height / 5, textBounds.Y + 2 * height / 5 + inpad,
                    textBounds.X + 2 * height / 5, textBounds.Y + 4 * height / 5 + inpad);
                context.g.DrawLine(drawPen, textBounds.X + 4 * height / 5, textBounds.Y + height / 5 + inpad,
                    textBounds.X + 2 * height / 5, textBounds.Y + 4 * height / 5 + inpad);
            }

            textBounds.X += textSize.Height;
            textBounds.Size = textSize;
            context.g.DrawString(Text, Style.Font, tempForeBrush, textBounds, format);

            // draw selection marks
            base.Paint(context);
        }


        public override void OnClick(PVDragContext dc)
        {
            Status = !Status;

            if (HasImmediateEvaluation) Evaluate();

            base.OnClick(dc);
        }

        public override MNEvaluationResult Evaluate()
        {
            if (HasImmediateEvaluation || HasLazyEvaluation)
            {
                UIStateError = (ExpectedStatus == Status) ? MNEvaluationResult.Correct : MNEvaluationResult.Incorrect;
            }
            return UIStateError;
        }
    }
}
