using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVTrackerConnectionBase: GVTrackerBase
    {
        public GVTrackerConnectionBase()
        {
            Title = "Connection";
        }

        public override void OnDraw(GVGraphViewContext context, RectangleF paintRect)
        {
            base.OnDraw(context, paintRect);

            context.Graphics.DrawImage(GetTrackerIcon(), paintRect);
            if (StatusHighlighted && !context.SelectedObject.PaintedRect.Contains(context.mouseMovePoint))
            {
                if (context.TrackedObject != null && context.TrackedObject.AcceptsTracker(this))
                {
                    GVGraphics.DrawRectangleLine(context.Graphics, Pens.Black, context.SelectedObject.PaintedRect,
                        context.TrackedObject.PaintedRect);
                }
                else
                {
                    GVGraphics.DrawRectangleLine(context.Graphics, Pens.Black,
                        context.SelectedObject.PaintedRect, context.mouseMovePoint);
                }
            }
        }

        public override Image GetTrackerIcon()
        {
            return Properties.Resources.TrackerPageNextIcon;
        }

        public override void OnTouchEnd(GVGraphViewContext context)
        {
            if (context.TrackedObject != null && context.TrackedObject.AcceptsTracker(this))
            {
                GVGraphConnection gc = GetLinkType(context.Graph);
                gc.Source = context.SelectedObject;
                gc.Target = context.TrackedObject;

                if (context.Delegate == null || context.Delegate.WillInsertConnection(context, gc))
                {
                    context.Graph.Connections.Add(gc);
                    context.Delegate.DidInsertConnection(gc);
                }
            }

        }

        public virtual GVGraphConnection GetLinkType(GVGraph g)
        {
            return new GVGraphConnection(g);
        }
    }
}
