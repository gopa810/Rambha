using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVTrackerBase
    {
        public int TrackId = 0;
        public string Title = "";
        public GVObjectTrackerType ClickType;
        public RectangleF drawRect = RectangleF.Empty;
        public bool StatusHighlighted = false;

        public GVTrackerBase()
        {
            OnReset();
        }

        public virtual void OnReset()
        {
            ClickType = GVObjectTrackerType.Click;
            StatusHighlighted = false;
        }

        public virtual void OnTouchBegin(GVGraphViewContext context)
        {
            StatusHighlighted = true;
            context.downObject = context.SelectedObject;
        }

        public virtual void OnTouchEnd(GVGraphViewContext context)
        {
        }

        public virtual void OnClick(GVGraphViewContext context)
        {
        }

        public virtual void OnTrack(GVGraphViewContext context)
        {
            if (context.TrackedObject != null && !context.TrackedObject.AcceptsTracker(this))
                context.TrackedObject = null;
        }

        public virtual void OnDraw(GVGraphViewContext context, RectangleF paintRect)
        {
            if (GetTrackerIcon() != null)
            {
                context.Graphics.DrawImage(GetTrackerIcon(), paintRect);
            }
        }

        public virtual Image GetTrackerIcon()
        {
            return null;
        }

        public virtual string GetHelpText()
        {
            return Title;
        }

        public virtual Brush GetBackgroundBrush()
        {
            if (StatusHighlighted)
            {
                switch (ClickType)
                {
                    case GVObjectTrackerType.Click:
                        return GVGraphics.GetBrush(Color.CadetBlue);
                    case GVObjectTrackerType.DragDrop:
                        return GVGraphics.GetBrush(Color.MediumSeaGreen);
                    case GVObjectTrackerType.Switch:
                        return GVGraphics.GetBrush(Color.MediumVioletRed);
                    default:
                        return GVGraphics.GetBrush(Color.White);
                }
            }
            else
            {
                switch (ClickType)
                {
                    case GVObjectTrackerType.Click:
                        return GVGraphics.GetBrush(Color.Teal);
                    case GVObjectTrackerType.DragDrop:
                        return GVGraphics.GetBrush(Color.SeaGreen);
                    case GVObjectTrackerType.Switch:
                        return GVGraphics.GetBrush(Color.Purple);
                    default:
                        return GVGraphics.GetBrush(Color.Gray);
                }
            }
        }
    }

    public enum GVObjectTrackerType
    {
        DragDrop,
        Click,
        Switch
    }
}
