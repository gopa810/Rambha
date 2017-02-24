using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.IO;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMRectangleArea: GSCore
    {
        /// <summary>
        /// This is relative point (as if page or image has 1000 x 1000 pt)
        /// </summary>
        [Browsable(true)]
        public SMRuler LeftRuler;
        [Browsable(true)]
        public SMRuler TopRuler;
        [Browsable(true)]
        public SMRuler RightRuler;
        [Browsable(true)]
        public SMRuler BottomRuler;

        [Browsable(false)]
        public bool Selected { get; set; }


        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            LeftRuler.Save(bw);
            TopRuler.Save(bw);
            RightRuler.Save(bw);
            BottomRuler.Save(bw);

            bw.WriteByte(11);
            bw.WriteBool(Selected);

            // end of object
            bw.WriteByte(0);
        }

        public bool Load(RSFileReader br)
        {
            byte tag;

            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        LeftRuler.Load(br);
                        TopRuler.Load(br);
                        RightRuler.Load(br);
                        BottomRuler.Load(br);
                        break;
                    case 11:
                        Selected = br.ReadBool();
                        break;
                }
            }

            return true;
        }


        public Rectangle[] LastBounds = new Rectangle[9];

        [Browsable(false)]
        public SMControlSelection TrackedSelection
        {
            get
            {
                SMControlSelection sm = SMControlSelection.None;
                if (LeftRuler.SelectedForTracking) sm |= SMControlSelection.Left;
                if (TopRuler.SelectedForTracking) sm |= SMControlSelection.Top;
                if (BottomRuler.SelectedForTracking) sm |= SMControlSelection.Bottom;
                if (RightRuler.SelectedForTracking) sm |= SMControlSelection.Right;
                return sm;
            }
            set
            {
                LeftRuler.SelectedForTracking = ((value & SMControlSelection.Left) == SMControlSelection.Left);
                TopRuler.SelectedForTracking = ((value & SMControlSelection.Top) == SMControlSelection.Top);
                RightRuler.SelectedForTracking = ((value & SMControlSelection.Right) == SMControlSelection.Right);
                BottomRuler.SelectedForTracking = ((value & SMControlSelection.Bottom) == SMControlSelection.Bottom);
            }
        }


        public SMRectangleArea()
        {
            TopRuler = new SMRuler(SMAxis.Y);
            BottomRuler = new SMRuler(SMAxis.Y);
            RightRuler = new SMRuler(SMAxis.X);
            LeftRuler = new SMRuler(SMAxis.X);

            TopRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 0.0);
            BottomRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 1.0);
            LeftRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 0.0);
            RightRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 1.0);

            Selected = false;
            TrackedSelection = SMControlSelection.None;

        }

        public SMRectangleArea(SMRectangleArea source)
        {
            if (source == null)
            {
                TopRuler = new SMRuler(SMAxis.Y);
                BottomRuler = new SMRuler(SMAxis.Y);
                RightRuler = new SMRuler(SMAxis.X);
                LeftRuler = new SMRuler(SMAxis.X);

                TopRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 0.0);
                BottomRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 1.0);
                LeftRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 0.0);
                RightRuler.SetRawValue(PageEditDisplaySize.LandscapeBig, 1.0);

                Selected = false;
                TrackedSelection = SMControlSelection.None;
            }
            else
            {
                Copy(source, this);
            }
        }

        public bool Changed
        {
            get
            {
                return TopRuler.Changed | BottomRuler.Changed | LeftRuler.Changed | RightRuler.Changed;
            }
            set
            {
                TopRuler.Changed = value;
                BottomRuler.Changed = value;
                RightRuler.Changed = value;
                LeftRuler.Changed = value;
            }
        }

        public static void Copy(SMRectangleArea source, SMRectangleArea target)
        {
            target.LeftRuler = new SMRuler(source.LeftRuler);
            target.RightRuler = new SMRuler(source.RightRuler);
            target.TopRuler = new SMRuler(source.TopRuler);
            target.BottomRuler = new SMRuler(source.BottomRuler);
            target.Selected = source.Selected;
            target.TrackedSelection = source.TrackedSelection;
        }

        /// <summary>
        /// Move object by specified offset defined in logical coordinate system
        /// </summary>
        /// <param name="offset">Logical offset (in logical coordinate system)</param>
        public virtual void Move(MNPageContext context, System.Drawing.Point offset)
        {
            if (TopRuler.SelectedForTracking)
                TopRuler.AddValue(context, offset);
            if (BottomRuler.SelectedForTracking)
                BottomRuler.AddValue(context, offset);
            if (LeftRuler.SelectedForTracking)
                LeftRuler.AddValue(context, offset);
            if (RightRuler.SelectedForTracking)
                RightRuler.AddValue(context, offset);
        }

        public virtual void MoveRaw(double rx, double ry)
        {
            TopRuler.AddRawValue(ry);
            BottomRuler.AddRawValue(ry);
            LeftRuler.AddRawValue(rx);
            RightRuler.AddRawValue(rx);
        }
        public void SetCenterSize(Point center, Size defSize, PageEditDisplaySize dispSize)
        {
            LeftRuler.SetValue(dispSize, center.X - defSize.Width / 2);
            TopRuler.SetValue(dispSize, center.Y - defSize.Height / 2);
            RightRuler.SetValue(dispSize, center.X + defSize.Width / 2);
            BottomRuler.SetValue(dispSize, center.Y + defSize.Height / 2);
        }

        public virtual SMControlSelection TestHitLogical(MNPageContext context, Point logicalPoint)
        {
            if (Selected)
            {
                if (LastBounds[1].Contains(logicalPoint)) return SMControlSelection.Top | SMControlSelection.Left;
                if (LastBounds[2].Contains(logicalPoint)) return SMControlSelection.Top;
                if (LastBounds[3].Contains(logicalPoint)) return SMControlSelection.Top | SMControlSelection.Right;
                if (LastBounds[4].Contains(logicalPoint)) return SMControlSelection.Left;
                if (LastBounds[5].Contains(logicalPoint)) return SMControlSelection.Right;
                if (LastBounds[6].Contains(logicalPoint)) return SMControlSelection.Bottom | SMControlSelection.Left;
                if (LastBounds[7].Contains(logicalPoint)) return SMControlSelection.Bottom;
                if (LastBounds[8].Contains(logicalPoint)) return SMControlSelection.Bottom | SMControlSelection.Right;
                /*if (LastBounds[9].Contains(logicalPoint)) return SMControlSelection.LeftBoundary;
                if (LastBounds[10].Contains(logicalPoint)) return SMControlSelection.TopBoundary;
                if (LastBounds[11].Contains(logicalPoint)) return SMControlSelection.RightBoundary;
                if (LastBounds[12].Contains(logicalPoint)) return SMControlSelection.BottomBoundary;*/
            }
            return LastBounds[0].Contains(logicalPoint) ? SMControlSelection.All : SMControlSelection.None;
        }

        /// <summary>
        /// returns bounds of control in logical coordinate system
        /// if control is tracked, then returns modified bounds according tracking values
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public Rectangle GetBounds(MNPageContext ctx)
        {
            if (Selected && ctx.isTracking)
                RecalcAllBounds(ctx);
            return GetBoundsRecalc(ctx);
        }

        public Rectangle GetBounds(PageEditDisplaySize dsp)
        {
            int x = LeftRuler.GetValue(dsp);
            int y = TopRuler.GetValue(dsp);
            int r = RightRuler.GetValue(dsp);
            int b = BottomRuler.GetValue(dsp);

            return new Rectangle(x, y, r - x, b - y);
        }

        public Rectangle GetBoundsRecalc(MNPageContext ctx)
        {
            int x = LeftRuler.GetValue(ctx.DisplaySize);
            int y = TopRuler.GetValue(ctx.DisplaySize);
            int r = RightRuler.GetValue(ctx.DisplaySize);
            int b = BottomRuler.GetValue(ctx.DisplaySize);

            if (TopRuler.SelectedForTracking)
                y += ctx.TrackedDrawOffset.Y;
            if (BottomRuler.SelectedForTracking)
                b += ctx.TrackedDrawOffset.Y;
            if (LeftRuler.SelectedForTracking)
                x += ctx.TrackedDrawOffset.X;
            if (RightRuler.SelectedForTracking)
                r += ctx.TrackedDrawOffset.X;

            LastBounds[0] = new Rectangle(x, y, r - x, b - y);
            return LastBounds[0];
        }

        public void RecalcAllBounds(MNPageContext ctx)
        {
            int x = LeftRuler.GetValue(ctx.DisplaySize);
            int y = TopRuler.GetValue(ctx.DisplaySize);
            int r = RightRuler.GetValue(ctx.DisplaySize);
            int b = BottomRuler.GetValue(ctx.DisplaySize);

            if (TopRuler.SelectedForTracking)
                y += ctx.TrackedDrawOffset.Y;
            if (BottomRuler.SelectedForTracking)
                b += ctx.TrackedDrawOffset.Y;
            if (LeftRuler.SelectedForTracking)
                x += ctx.TrackedDrawOffset.X;
            if (RightRuler.SelectedForTracking)
                r += ctx.TrackedDrawOffset.X;

            int markWidth = ctx.PhysicalToLogical(3);
            LastBounds[0] = new Rectangle(x, y, r - x, b - y);
            LastBounds[1] = new Rectangle(x - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
            LastBounds[2] = new Rectangle((x + r) / 2 - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
            LastBounds[3] = new Rectangle(r - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
            LastBounds[4] = new Rectangle(x - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);
            LastBounds[5] = new Rectangle(r - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);
            LastBounds[6] = new Rectangle(x - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
            LastBounds[7] = new Rectangle((x + r) / 2 - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
            LastBounds[8] = new Rectangle(r - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
        }

        public SMRuler GetBoundaryRuler(SMControlSelection testResult)
        {
            if ((testResult & SMControlSelection.LeftBoundary) == SMControlSelection.LeftBoundary)
                return LeftRuler;
            if ((testResult & SMControlSelection.RightBoundary) == SMControlSelection.RightBoundary)
                return RightRuler;
            if ((testResult & SMControlSelection.TopBoundary) == SMControlSelection.TopBoundary)
                return TopRuler;
            if ((testResult & SMControlSelection.BottomBoundary) == SMControlSelection.BottomBoundary)
                return BottomRuler;
            return null;
        }

        public Rectangle GetRawRectangle(PageEditDisplaySize ds)
        {
            int x = Convert.ToInt32(LeftRuler.GetRawValue(ds) * 1000);
            int y = Convert.ToInt32(TopRuler.GetRawValue(ds) * 1000);
            int w = Convert.ToInt32(RightRuler.GetRawValue(ds) * 1000) - x;
            int h = Convert.ToInt32(BottomRuler.GetRawValue(ds) * 1000) - y;
            return new Rectangle(x, y, w, h);
        }

        public void Set(SMRectangleArea area, int x, int y)
        {
            Copy(area, this);
            this.MoveRaw(x / 1024.0, y / 768.0);
        }
    }
}
