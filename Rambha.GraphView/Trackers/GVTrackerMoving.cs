using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVTrackerMoving: GVTrackerBase
    {
        public GVTrackerMoving()
        {
            Title = "Move Object";
        }

        public override void OnTouchEnd(GVGraphViewContext p_context)
        {
            p_context.MovedObject.X += p_context.MovedObjectTempOffset.X / p_context.Scale;
            p_context.MovedObject.Y += p_context.MovedObjectTempOffset.Y / p_context.Scale;
            base.OnTouchEnd(p_context);
        }

        public override void OnTouchBegin(GVGraphViewContext context)
        {
            base.OnTouchBegin(context);
            context.MovedObject = context.SelectedObject;
        }

        public override void OnTrack(GVGraphViewContext p_context)
        {
            base.OnTrack(p_context);

            // move only selected object
            p_context.MovedObjectTempOffset.X = p_context.mouseMovePoint.X - p_context.mouseDownPoint.X;
            p_context.MovedObjectTempOffset.Y = p_context.mouseMovePoint.Y - p_context.mouseDownPoint.Y;
        }

        public override void OnDraw(GVGraphViewContext context, RectangleF paintRect)
        {
            base.OnDraw(context, paintRect);

            context.Graphics.DrawImage(Properties.Resources.TrackerMoveIcon, paintRect);
        }
    }

}
