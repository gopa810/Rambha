using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordBase
    {
        public SMFont Font = null;
        public int LineNo = 0;
        public int ColumnNo = 0;
        public int PageNo = 0;
        public RectangleF rect = RectangleF.Empty;
        public string tag = string.Empty;
        public MNEvaluationType Evaluation = MNEvaluationType.Lazy;
        private SMDragResponse draggable = SMDragResponse.Undef;
        public int lineOffset = 0;
        public string replacementTarget = null;
        public bool UIStateHover = false;

        public SMWordBase(SMFont f)
        {
            Font = f;
        }

        public virtual SMTokenItem GetDraggableItem()
        {
            return null;
        }

        public virtual bool IsSpace()
        {
            return false;
        }

        public virtual void Paint(MNPageContext context, SMStatusLayout layout, int X, int Y)
        {
        }

        public SMDragResponse Draggable
        {
            get { return draggable; }
            set { draggable = value; }
        }

        public bool IsDraggable
        {
            get
            {
                return Draggable == SMDragResponse.Drag || Draggable == SMDragResponse.Line;
            }
        }

        public virtual void HoverPoint(Point p)
        {
        }

        public virtual Brush GetCurrentTextBrush(SMStatusLayout layout)
        {
            return SMGraphics.GetBrush(layout.ForeColor);
        }

    }


    public class SMWordLine : List<SMWordBase>
    {
    }

}
