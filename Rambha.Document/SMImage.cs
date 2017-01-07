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
        [Browsable(true),Category("Content")]
        public MNReferencedImage Image { get; set; }

        [Browsable(true), Category("Layout")]
        public SMContentScaling ContentScaling { get; set; }

        public string DroppedTag { get; set; }

        public SMImage(MNPage p): base(p)
        {
            Image = null;
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

            Rectangle bounds = rect;
            bounds.X += Style.ContentPadding.Left;
            bounds.Y += Style.ContentPadding.Top;
            bounds.Width -= (Style.ContentPadding.Left + Style.ContentPadding.Right);
            bounds.Height -= (Style.ContentPadding.Top + Style.ContentPadding.Bottom);

            if (Image != null)
            {
                SMContentScaling scaling = ContentScaling;
                if (scaling == SMContentScaling.Normal)
                    scaling = SMContentScaling.Fit;
                if (scaling == SMContentScaling.Fit)
                {
                    Size imageSize = pi.Image.ImageData.Size;
                    if (imageSize.Width > bounds.Width || imageSize.Height > bounds.Height)
                    {
                        double ratio = Math.Max(imageSize.Width / Convert.ToDouble(bounds.Width),
                            imageSize.Height / Convert.ToDouble(bounds.Height));
                        imageSize = new Size(Convert.ToInt32(imageSize.Width / ratio), Convert.ToInt32(imageSize.Height / ratio));
                    }

                    switch (Style.ContentAlign)
                    {
                        case SMContentAlign.TopLeft:
                            break;
                        case SMContentAlign.TopCenter:
                            bounds.X += (bounds.Width - imageSize.Width)/2;
                            break;
                        case SMContentAlign.TopRight:
                            bounds.X += (bounds.Width - imageSize.Width);
                            break;
                        case SMContentAlign.CenterLeft:
                            bounds.Y += (bounds.Height - imageSize.Height) / 2;
                            break;
                        case SMContentAlign.Center:
                            bounds.Y += (bounds.Height - imageSize.Height) / 2;
                            bounds.X += (bounds.Width - imageSize.Width)/2;
                            break;
                        case SMContentAlign.CenterRight:
                            bounds.Y += (bounds.Height - imageSize.Height) / 2;
                            bounds.X += (bounds.Width - imageSize.Width);
                            break;
                        case SMContentAlign.BottomLeft:
                            bounds.Y += (bounds.Height - imageSize.Height);
                            break;
                        case SMContentAlign.BottomCenter:
                            bounds.Y += (bounds.Height - imageSize.Height);
                            bounds.X += (bounds.Width - imageSize.Width)/2;
                            break;
                        case SMContentAlign.BottomRight:
                            bounds.Y += (bounds.Height - imageSize.Height);
                            bounds.X += (bounds.Width - imageSize.Width);
                            break;
                    }
                    bounds.Width = imageSize.Width;
                    bounds.Height = imageSize.Height;

                }
                g.DrawImage(pi.Image.ImageData, bounds);
            }

            base.Paint(context);
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
                            //Image = new MNReferencedImageLoadingPlaceholder() { imageId = br.ReadInt32() };
                            br.AddReference(Document, "Image", br.ReadInt64(), 10, this);
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

        public override void setReference(int tag, object obj)
        {
            switch (tag)
            {
                case 10:
                    if (obj is MNReferencedImage)
                        Image = (MNReferencedImage)obj;
                    break;
            }

            base.setReference(tag, obj);
        }

        public override SMTokenItem GetDraggableItem(Point point)
        {
            if (Image != null && Image.ImageData != null)
            {
                SMTokenItem item = new SMTokenItem();
                item.Tag = Tag;
                item.Text = "";
                item.ContentSize = Image.ImageData.Size;
                item.Image = Image.ImageData;
                return item;
            }
            else
            {
                return null;
            }
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
                MNReferencedImage mri = new MNReferencedImage(Page.Document);
                mri.ImageData = dc.draggedItem.Image;
                DroppedTag = dc.draggedItem.Tag;
                return true;
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt64(Image != null ? Image.Id : -1);
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
