using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMTextView: SMTextViewBase
    {
        private Size p_textSize = Size.Empty;

        public SMTextView(MNPage p)
            : base(p)
        {
            Text = "Text View";
            RunningLine = SMRunningLine.Natural;
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
                            break;
                    }
                }
            }

            return true;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(0);
        }

        public void ContentToTags()
        {

        }

        public override void ResetStatus()
        {
            foreach (SMWordBase wb in drawWords)
            {
                if (wb is SMWordToken)
                {
                    SMWordToken wt = (SMWordToken)wb;
                    wt.droppedItem = null;
                    wt.editedText = "";
                }
            }
            base.ResetStatus();
        }
    }
}
