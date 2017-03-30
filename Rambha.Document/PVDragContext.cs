using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Script;

namespace Rambha.Document
{
    /// <summary>
    /// Context for tracking the status of user interaction through the screen
    /// </summary>
    public class PVDragContext : GSCore
    {
        public enum Status
        {
            ClickDown,
            Dragging,
            LongClicked,
            RefusingDrop,
            None
        }

        public Status State = Status.None;
        public Point startPoint = Point.Empty;
        public Point lastPoint = Point.Empty;
        public SMControl startControl = null;
        public SMControl endControl = null;
        public SMControl trackedControl = null;
        public SMTokenItem draggedItem = null;
        public MNPageContext context = null;

        public SMRectangleArea getStartArea(MNPage p)
        {
            return (startControl == null) ? null : startControl.Area;
        }

        public long getStartAreaId()
        {
            if (startControl == null) return -1;
            return startControl.Id;
        }

        public SMRectangleArea getTrackedArea(MNPage p)
        {
            return (trackedControl == null) ? null : trackedControl.Area;
        }

        public long getTrackedAreaId()
        {
            if (trackedControl == null) return -1;
            return trackedControl.Id;
        }

        public bool StartClicked
        {
            get
            {
                if (startControl != null)
                {
                    return startControl.UIStatePressed;
                }
                return false;
            }
            set
            {
                if (startControl != null && startControl.Clickable)
                {
                    startControl.UIStatePressed = value;
                }
            }
        }

        public bool EndHover
        {
            get
            {
                if (endControl != null)
                {
                    return endControl.UIStateHover;
                }
                return false;
            }
            set
            {
                if (endControl != null && startControl.Cardinality != SMConnectionCardinality.None)
                {
                    startControl.UIStateHover = value;
                }
            }
        }


        public SMDragResponse DragType { get; set; }
    }

}
