using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVGraphViewContext
    {
        private GVGraphObject p_selectedObject = null;
        private float p_scale = 1.0f;
        private int p_scale_index = 5;
        private static float[] p_scales = new float[] { 1/30f, 1/25f, 1/20f, 1/15f, 1/10f, 1/5f, 
            1/3f, 1/2f, 2/3f, 1f, 6/5f,
            5/4f, 4/3f, 3/2f, 35/20f, 2f, 3f };

        public Graphics Graphics { get; set; }

        public GVGraph Graph { get; set; }

        public Color BackColor { get; set; }

        public PointF mouseDownPoint = PointF.Empty;
        public PointF mouseMovePoint = PointF.Empty;
        public PointF mouseUpPoint = PointF.Empty;

        public PointF logDownPoint = PointF.Empty;
        public PointF logMovePoint = PointF.Empty;
        public PointF logUpPoint = PointF.Empty;

        public GVGraphViewMouseState mouseState = GVGraphViewMouseState.None;

        public GVGraphObject downObject = null;
        public PointF downObjectHotPointOffset = PointF.Empty;

        public GVGraphObject TrackedObject = null;

        public GVGraphObject upObject = null;

        public PointF Offset = PointF.Empty;

        public float Scale { get { return p_scale; } }

        public GVGraphObject SelectedObject
        {
            get { return p_selectedObject; }
            set
            {
                p_selectedObject = value;
            }
        }

        public GVGraphObject MovedObject { get; set; }

        public PointF MovedObjectTempOffset = PointF.Empty;

        public PointF DiagramTempOffset = PointF.Empty;

        public GVTrackerBase ActiveTracker = null;

        public IGraphDelegate Delegate = null;

        public void ResetPoints()
        {
            mouseDownPoint = PointF.Empty;
            mouseMovePoint = PointF.Empty;
            mouseUpPoint = PointF.Empty;

            logDownPoint = PointF.Empty;
            logMovePoint = PointF.Empty;
            logUpPoint = PointF.Empty;

            mouseState = GVGraphViewMouseState.None;

            downObject = null;
            downObjectHotPointOffset = PointF.Empty;
            TrackedObject = null;

            upObject = null;
            ActiveTracker = null;
        }

        public void ResetObjects()
        {
            MovedObjectTempOffset = PointF.Empty;
            DiagramTempOffset = PointF.Empty;
        }

        public PointF ConvertClientToLogical(float clientX, float clientY)
        {
            return new PointF((clientX - Offset.X) / p_scale, (clientY - Offset.Y) / p_scale);
        }

        public PointF ConvertLogicalToClient(float logX, float logY)
        {
            return new PointF(logX * p_scale + Offset.X, logY * p_scale + Offset.Y);
        }

        public void SetScale(float f)
        {
            float dif = 1000;
            int index = 0;
            for (int i = 0; i < p_scales.Length; i++)
            {
                if (Math.Abs(p_scales[i] - f) < dif)
                {
                    dif = Math.Abs(p_scales[i] - f);
                    index = i;
                }
            }

            p_scale = p_scales[index];
            p_scale_index = index;
        }

        public void IncreaseScale()
        {
            p_scale_index++;
            if (p_scale_index >= p_scales.Length)
                p_scale_index = p_scales.Length - 1;
            p_scale = p_scales[p_scale_index];
        }

        public void DescreaseScale()
        {
            p_scale_index--;
            if (p_scale_index < 0)
                p_scale_index = 0;
            p_scale = p_scales[p_scale_index];

        }

    }

    public enum GVGraphViewMouseState
    {
        Down, None, Moved
    }


}
