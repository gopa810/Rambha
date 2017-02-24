using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class EditSpotsEditorView : UserControl
    {
        public LocalizationMainForm ParentFrame { get; set; }
        private MNReferencedImage p_image = null;

        private Rectangle showRect = Rectangle.Empty;

        private MNReferencedSpot CurrentSpot = null;

        public MNReferencedImage Image 
        {
            get
            {
                return p_image;
            }
            set
            {
                p_image = value;
                if (p_image != null)
                    Invalidate();
            }
        }
        public EditSpotsEditorView()
        {
            InitializeComponent();
        }

        private void ImageSpotsEditorView_Paint(object sender, PaintEventArgs e)
        {
            if (p_image == null)
                return;

            Size imgSize = p_image.ImageData.Size;
            if (imgSize.Width < 2 || imgSize.Height < 2)
                return;

            Size viewSize = this.Size;

            double ratio = Math.Min(Convert.ToDouble(viewSize.Width)/imgSize.Width, Convert.ToDouble(viewSize.Height)/imgSize.Height);

            Size showSize = new Size(Convert.ToInt32(ratio * imgSize.Width), Convert.ToInt32(ratio * imgSize.Height));

            // rectangle for showing image
            showRect = new Rectangle(viewSize.Width/2 - showSize.Width/2, viewSize.Height/2 - showSize.Height/2, showSize.Width, showSize.Height);

            e.Graphics.DrawImage(p_image.ImageData, showRect);

            if (p_image.HasSpots())
            {
                foreach (MNReferencedSpot spot in p_image.SafeSpots)
                {
                    spot.Paint(e.Graphics, showRect, (spot == CurrentSpot));
                }
            }

        }

        private void addNewRectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (p_image == null) return;

            MNReferencedSpot spot = new MNReferencedSpot();
            spot.Shape = MNRefSpotShape.Rectangle;
            spot.Center = lastRelUp;
            spot.AnchorA = new Point(10, 0);
            spot.AnchorB = new Point(0, 10);

            p_image.SafeSpots.Add(spot);
            Invalidate();
        }

        private void addNewCircleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (p_image == null) return;

            MNReferencedSpot spot = new MNReferencedSpot();
            spot.Shape = MNRefSpotShape.Circle;
            spot.Center = lastRelUp;
            spot.AnchorA = new Point(10, 0);
            spot.AnchorB = new Point(0, 10);

            p_image.SafeSpots.Add(spot);
            Invalidate();
        }

        private MNReferencedSpot p_movedSpot = null;
        private Point p_moveCenterCorr = Point.Empty;

        private MNReferencedSpot p_resizedSpot = null;
        private int p_resizedAnchor = 0;

        private void ImageSpotsEditorView_MouseDown(object sender, MouseEventArgs e)
        {
            if (p_image == null) return;
            Point rel = p_image.AbsToRel(showRect, new Point(e.X, e.Y));
            p_movedSpot = null;
            p_resizedSpot = null;
            MNReferencedSpot lastSpot = CurrentSpot;
            CurrentSpot = null;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (p_image.HasSpots())
                {
                    foreach (MNReferencedSpot spot in p_image.SafeSpots)
                    {
                        if (lastSpot != null && spot == lastSpot)
                        {
                            for (int anchorIndex = 0; anchorIndex < 2; anchorIndex++)
                            {
                                Point A = spot.AbsoluteAnchor(showRect, anchorIndex);
                                Debugger.Log(0, "", "Anchor " + anchorIndex + ": " + A.X + ", " + A.Y + ";   e:" + e.X + ", " + e.Y + "\n");
                                if (Math.Abs(A.X - e.X) <= 3 && Math.Abs(A.Y - e.Y) <= 3)
                                {
                                    CurrentSpot = spot;
                                    p_resizedSpot = spot;
                                    p_resizedAnchor = anchorIndex;
                                    break;
                                }
                            }
                        }

                        if (p_resizedSpot != null)
                            break;

                        if (spot.Contains(rel))
                        {
                            CurrentSpot = spot;
                            p_movedSpot = spot;
                            Point A = spot.AbsoluteCenter(showRect);
                            p_moveCenterCorr = new Point(e.X - A.X, e.Y - A.Y);
                            MNNotificationCenter.BroadcastMessage(p_image, "ObjectSelected", spot);
                            break;
                        }

                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
            }
        }

        private void ImageSpotsEditorView_MouseMove(object sender, MouseEventArgs e)
        {
            if (p_image == null) return;

            if (p_image.HasSpots())
            {
                if (p_movedSpot != null)
                {
                    Point newAbsCenter = new Point(e.X - p_moveCenterCorr.X, e.Y - p_moveCenterCorr.Y);
                    p_movedSpot.Center = p_image.AbsToRel(showRect, newAbsCenter);
                    Invalidate();
                }
                else if (p_resizedSpot != null)
                {
                    Point p = p_image.AbsToRel(showRect, new Point(e.X, e.Y));
                    p.X -= p_resizedSpot.Center.X;
                    p.Y -= p_resizedSpot.Center.Y;
                    if (p_resizedAnchor == 0)
                        p_resizedSpot.AnchorA = p;
                    else
                        p_resizedSpot.AnchorB = p;
                    Invalidate();
                }
            }
        }

        private Point lastPointUp = Point.Empty;
        private Point lastRelUp = Point.Empty;

        private void ImageSpotsEditorView_MouseUp(object sender, MouseEventArgs e)
        {
            if (p_image == null) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                p_movedSpot = null;
                p_resizedSpot = null;
                if (p_image.HasSpots())
                {
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                lastPointUp = new Point(e.X, e.Y);
                lastRelUp = p_image.AbsToRel(showRect, lastPointUp);
                contextMenuStrip1.Show(PointToScreen(lastPointUp));
            }
        }

        private void ImageSpotsEditorView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (CurrentSpot == null || p_image == null)
                    return;

                if (MessageBox.Show("Do you want to delete selected spot area?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    p_image.SafeSpots.Remove(CurrentSpot);
                    Invalidate();
                }
            }
        }

    }
}
