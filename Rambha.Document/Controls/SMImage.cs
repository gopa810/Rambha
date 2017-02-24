using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Xml;
using System.IO;

using Rambha.Document.Views;
using Rambha.Serializer;

namespace Rambha.Document
{
    [Serializable()]
    public class SMImage: SMControl
    {
        [Editor(typeof(ImageSelectionPropertyEditor),typeof(UITypeEditor))]

        [Browsable(true), Category("Layout")]
        public SMContentScaling ContentScaling { get; set; }

        public MNLazyImage Img = null;

        public string DroppedTag { get; set; }

        private Rectangle showRect = Rectangle.Empty;

        public SMImage(MNPage p): base(p)
        {
            Img = new MNLazyImage(p.Document);
            ContentScaling = SMContentScaling.Fit;
            Evaluation = MNEvaluationType.Inherited;
            DroppedTag = "";
        }

        public override Size GetDefaultSize()
        {
            return new Size(64,64);
        }

        public override void Paint(MNPageContext context)
        {
            Graphics g = context.g;
            SMImage pi = this;
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle rect = area.GetBounds(context);

            PrepareBrushesAndPens();

            DrawStyledBorder(context, rect);

            Rectangle bounds = Style.ApplyPadding(rect);

            Image image = GetContentImage();

            if (image != null)
            {
                SMContentScaling scaling = ContentScaling;
                if (scaling == SMContentScaling.Normal)
                    scaling = SMContentScaling.Fit;
                if (scaling == SMContentScaling.Fit)
                {
                    Size imageSize = image.Size;
                    if (imageSize.Width > bounds.Width || imageSize.Height > bounds.Height)
                    {
                        double ratio = Math.Max(imageSize.Width / Convert.ToDouble(bounds.Width),
                            imageSize.Height / Convert.ToDouble(bounds.Height));
                        imageSize = new Size(Convert.ToInt32(imageSize.Width / ratio), Convert.ToInt32(imageSize.Height / ratio));
                    }

                    switch (GetVerticalAlign())
                    {
                        case SMVerticalAlign.Top:
                            break;
                        case SMVerticalAlign.Center:
                            bounds.Y += (bounds.Height - imageSize.Height) / 2;
                            break;
                        case SMVerticalAlign.Bottom:
                            bounds.Y += (bounds.Height - imageSize.Height);
                            break;
                    }
                    switch (GetHorizontalAlign())
                    {
                        case SMHorizontalAlign.Left:
                            break;
                        case SMHorizontalAlign.Center:
                        case SMHorizontalAlign.Justify:
                            bounds.X += (bounds.Width - imageSize.Width)/2;
                            break;
                        case SMHorizontalAlign.Right:
                            bounds.X += (bounds.Width - imageSize.Width);
                            break;
                    }
                    bounds.Width = imageSize.Width;
                    bounds.Height = imageSize.Height;

                }
                g.DrawImage(image, bounds);
                showRect = bounds;
            }

            base.Paint(context);
        }

        private System.Drawing.Image GetContentImage()
        {
            return Img.ImageData;
        }

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                byte tag;
                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10:
                            Img.ImageId = br.ReadInt64();
                            ContentScaling = (SMContentScaling)br.ReadInt32();
                            break;
                        default: 
                            return false;
                    }

                }

                return true;
            }

            return false;
        }

        public override SMTokenItem GetDraggableItem(Point point)
        {
            if (Img.ImageData != null)
            {
                SMTokenItem item = new SMTokenItem();
                item.Tag = Tag;
                item.Text = "";
                item.ContentSize = RestrictSize(Img.ImageData.Size, 32);
                item.Image = Img.ImageData;
                return item;
            }
            else
            {
                return null;
            }
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            base.OnTapEnd(dc);

            MNReferencedSpot spot = Img.Image.FindSpot(showRect, dc.lastPoint);
            if (spot != null)
            {
                MNReferencedCore obj = Document.FindContentObject(spot.ContentType, spot.ContentId);

                if (obj != null && (obj is MNReferencedSound) && Document.HasViewer)
                {
                    Document.Viewer.OnEvent("OnPlaySound", obj as MNReferencedSound);
                } 
            }
        }

        public override SMControl Duplicate()
        {
            SMImage label = new SMImage(Page);

            CopyContentTo(label);

            // copy label
            label.Img = new MNLazyImage(this.Img);
            label.ContentScaling = this.ContentScaling;

            return label;
        }

        public Size RestrictSize(Size size, int dim)
        {
            if (size.Width > dim && size.Height > dim)
            {
                if (size.Width > size.Height)
                {
                    if (size.Width > dim)
                    {
                        return new Size(32, Convert.ToInt32(size.Height * 32.0 / size.Width));
                    }
                }
                else
                {
                    if (size.Height > 32)
                    {
                        return new Size(Convert.ToInt32(size.Width * 32.0 / size.Height), 32);
                    }
                }
            }

            return new Size(dim, dim);
        }

        public override bool OnDropFinished(PVDragContext dc)
        {
            if (HasImmediateEvaluation)
            {
                if (!Tag.ToLower().Equals(dc.draggedItem.Tag.ToLower()))
                {
                    return false;
                }
            }

            DroppedItems.Clear();

            if (base.OnDropFinished(dc))
            {
                if (dc.draggedItem.Image != null)
                {
                    MNReferencedImage mri = new MNReferencedImage();
                    mri.ImageData = dc.draggedItem.Image;
                    DroppedTag = dc.draggedItem.Tag;
                    Img.Image = mri;
                    return true;
                }
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt64(Img.ImageId);
            bw.WriteInt32((Int32)ContentScaling);

            bw.WriteByte(0);
        }

        public override MNEvaluationResult Evaluate()
        {
            if (HasLazyEvaluation)
            {
                if (DroppedTag.ToLower().Equals(Tag.ToLower()))
                    UIStateError = MNEvaluationResult.Correct;
                else
                    UIStateError = MNEvaluationResult.Incorrect;
            }

            return UIStateError;
        }
    }
}
