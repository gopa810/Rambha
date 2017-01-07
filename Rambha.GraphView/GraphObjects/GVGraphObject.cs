using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    /// <summary>
    /// Accessibility for control flow
    /// 
    /// Subclasses should clarify if:
    /// - given subclass is eligible to be target for control flow connection
    /// - given subclass is eligible to be source for control flow connection
    ///   and the type of output (unconditional, conditional/named)
    /// 
    /// Accessibility for variables (data content)
    /// 
    /// Subclasses should clarify if:
    /// - they provide any data to other objects
    /// - they require any input data
    /// </summary>
    public class GVGraphObject: IRSSerializable, IRSObjectOrigin
    {
        protected GVGraph p_parent = null;

        public long Id = -1;

        public float X = 0;
        public float Y = 0;
        public float Width = 10;
        public float Height = 10;

        public bool Autosize = false;

        public const float TRACKER_DISTANCE = 4f;
        public const float TRACKER_SIZE = 24f;
        public const float DEFAULT_PADDING = 4f;

        public GVGraphObject ParentObject = null;
        public GVGraphObjectCollection Objects;

        /// <summary>
        /// This rectangle was painted to screen and in this rectangle
        /// we should test if mouse point is within this rectangle
        /// </summary>
        public RectangleF PaintedRect = RectangleF.Empty;

        public GVGraphObject(GVGraph g)
        {
            p_parent = g;
            Objects = new GVGraphObjectCollection(p_parent);
        }

        public SizeF Size
        {
            get
            {
                return new SizeF(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public PointF Location
        {
            get { return new PointF(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        public float Left { get { return X; } }
        public float Top { get { return Y; } }
        public float Right { get { return X + Width; } }
        public float Bottom { get { return Y + Height; } }
        public RectangleF Bounds
        {
            get { return new RectangleF(X, Y, Width, Height); }
            set { X = value.X; Y = value.Y; Width = value.Width; Height = value.Height; }
        }


        public void Paint(GVGraphViewContext p_drawContext, float relativeOriginX, float relativeOriginY)
        {
            Graphics g = p_drawContext.Graphics;
            Pen penBorder = Pens.Black;
            RectangleF rect = this.GetBounds(p_drawContext);
            string title = this.GetTitle();

            rect.X *= p_drawContext.Scale;
            rect.Y *= p_drawContext.Scale;
            if (!Autosize)
            {
                rect.Width *= p_drawContext.Scale;
                rect.Height *= p_drawContext.Scale;
            }

            if (ParentObject == null)
            {
                rect.Offset(p_drawContext.Offset);
                rect.Offset(p_drawContext.DiagramTempOffset);
            }
            rect.Offset(relativeOriginX, relativeOriginY);

            if (p_drawContext.MovedObject == this && ParentObject == null)
                rect.Offset(p_drawContext.MovedObjectTempOffset);

            if (p_drawContext.SelectedObject == this
                || p_drawContext.TrackedObject == this)
            {
                penBorder = Pens.Blue;
            }

            if (p_drawContext.SelectedObject == this)
            {
                List<GVTrackerBase> tracks = this.getTrackers();
                if (tracks != null)
                {
                    float x = rect.X;
                    float y = rect.Y - TRACKER_SIZE - TRACKER_DISTANCE;
                    RectangleF paintRect = new RectangleF(x, y, TRACKER_SIZE, TRACKER_SIZE);
                    foreach (GVTrackerBase ot in tracks)
                    {
                        p_drawContext.Graphics.FillRectangle(ot.GetBackgroundBrush(), paintRect);
                        ot.OnDraw(p_drawContext, paintRect);
                        ot.drawRect = paintRect;
                        paintRect.X += TRACKER_SIZE + TRACKER_DISTANCE;
                    }
                }
            }

            PaintContent(p_drawContext, rect, penBorder);

            this.PaintedRect = rect;

            // drawing 
            foreach (GVGraphObject go in this.Objects)
            {
                go.Paint(p_drawContext, rect.X, rect.Y);
            }

        }

        public virtual void PaintContent(GVGraphViewContext context, RectangleF rect, Pen penBorder)
        {
            context.Graphics.DrawRectangle(penBorder, rect.X, rect.Y, rect.Width, rect.Height);
            context.Graphics.DrawString(this.GetTitle(), SystemFonts.DefaultFont, Brushes.Black, rect.X + DEFAULT_PADDING, rect.Y + DEFAULT_PADDING);
        }

        public GVGraphObject LastParent
        {
            get
            {
                GVGraphObject par = this;
                while (par.ParentObject != null)
                    par = par.ParentObject;
                return par;
            }
        }

        public GVGraphObject FindObjectContainingClientPoint(float cX, float cY)
        {
            GVGraphObject start = null;
            GVGraphObject end = this;

            do
            {
                start = end;
                end = start.Objects.FindObjectContainingClientPoint(cX, cY);
            }
            while (end != null);

            return start;
        }

        public virtual void RefreshChildren()
        {
        }


        public void ResetTrackers()
        {
            foreach (GVTrackerBase ot in this.getTrackers())
            {
                ot.OnReset();
            }
        }

        public virtual List<GVTrackerBase> getTrackers()
        {
            return null;
        }

        public virtual string GetTitle()
        {
            return "";
        }

        public virtual void SetTitle(string value)
        {
        }

        public virtual RectangleF GetBounds(GVGraphViewContext context)
        {
            if (Autosize)
                return new RectangleF(Bounds.Location, GetBoundsSize(context));
            else
                return Bounds;
        }

        public virtual SizeF GetBoundsSize(GVGraphViewContext context)
        {
            if (Autosize)
            {
                SizeF f = context.Graphics.MeasureString(GetTitle(), SystemFonts.MenuFont);
                f.Width += 2 * DEFAULT_PADDING;
                f.Height += 2 * DEFAULT_PADDING;
                return f;
            }
            else
                return this.Bounds.Size;
        }

        public virtual bool AcceptsTracker(GVTrackerBase tracker)
        {
            return true;
        }

        public virtual List<GVDeclarationProcedure> getObjectMethods()
        {
            return new List<GVDeclarationProcedure>();
        }

        public virtual GVDeclarationFlowOut getControlOutNaming()
        {
            return null;
        }

        public virtual GVDeclarationDataEntry[] getDataProperties()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns>If return null, execution of successor should not happen. If returns empty string, then all uncoditioned control
        /// flows should be activated (their targets executed) and if returns non empty string, then executes those
        /// connections that have gven name</returns>
        public virtual string executeAction(GVGraphAction action)
        {
            return "";
        }

        public virtual void Load(RSFileReader R)
        {
            byte tag;
            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        Id = R.ReadInt64();
                        break;
                    case 11:
                        X = R.ReadFloat();
                        Y = R.ReadFloat();
                        Width = R.ReadFloat();
                        Height = R.ReadFloat();
                        break;
                    case 12:
                        Autosize = R.ReadBool();
                        break;
                    case 13:
                        Objects.Clear();
                        Objects.Load(R);
                        break;
                    case 14:
                        R.AddReference(p_parent, "GraphObject", R.ReadInt64(), 14, this);
                        break;
                    default:
                        throw new Exception("Unknown tag " + (int)tag + " in loading GVGraphObject at position " + R.Position);
                }
            }
        }

        public virtual void Save(RSFileWriter W)
        {
            W.WriteByte(10);
            W.WriteInt64(Id);

            W.WriteByte(11);
            W.WriteFloat(X);
            W.WriteFloat(Y);
            W.WriteFloat(Width);
            W.WriteFloat(Height);

            W.WriteByte(12);
            W.WriteBool(Autosize);

            W.WriteByte(13);
            Objects.Save(W);

            W.WriteByte(14);
            W.WriteInt64(ParentObject != null ? ParentObject.Id : -1);

            W.WriteByte(0);
        }

        public virtual void setReference(int tag, object obj)
        {
            switch (tag)
            {
                case 14:
                    ParentObject = (GVGraphObject)obj;
                    break;
            }
        }
    }



}
