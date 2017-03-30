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
        [Browsable(true), Category("Layout")]
        public SMContentArangement ContentArangement { get; set; }

        [Browsable(true), Category("Layout")]
        public SMContentScaling ContentScaling { get; set; }

        [Browsable(true), Category("Source Offset Fill")]
        public int SourceOffsetX { get; set; }

        [Browsable(true), Category("Source Offset Fill")]
        public int SourceOffsetY { get; set; }

        public MNLazyImage Img = null;

        public string DroppedTag { get; set; }

        private Rectangle sourceRect = Rectangle.Empty;
        private Rectangle showRect = Rectangle.Empty;

        private SMRichText rt = null;

        public SMImage(MNPage p): base(p)
        {
            Img = new MNLazyImage(p.Document);
            ContentScaling = SMContentScaling.Fit;
            Evaluation = MNEvaluationType.Inherited;
            DroppedTag = "";
            ContentArangement = SMContentArangement.ImageOnly;
            SourceOffsetX = 50;
            SourceOffsetY = 50;
            rt = new SMRichText(this);
        }

        public override Size GetDefaultSize()
        {
            return new Size(64,64);
        }

        public override void Paint(MNPageContext context)
        {
            Graphics g = context.g;
            SMImage pi = this;
            Rectangle rect = Area.GetBounds(context);

            SMConnection conn = context.CurrentPage.FindConnection(this);
            if (conn != null)
                UIStateHover = true;

            PrepareBrushesAndPens();

            Rectangle bounds = ContentPadding.ApplyPadding(rect);
            SMContentArangement argm = this.ContentArangement;

            Image image = GetContentImage();
            if (image == null)
                argm = SMContentArangement.TextOnly;

            if (argm == SMContentArangement.ImageOnly)
            {
                SMContentScaling scaling = ContentScaling;
                Rectangle rc = DrawImage(context, bounds, image, scaling, SourceOffsetX, SourceOffsetY);
                if (ContentScaling == SMContentScaling.Fill)
                {
                    showRect = bounds;
                    sourceRect = rc;
                }
                else
                {
                    showRect = rc;
                    sourceRect = new Rectangle(0, 0, image.Width, image.Height);
                }
            }
            else
            {
                Rectangle imgRect = bounds;
                Size textSize = Size.Empty;
                Font usedFont = GetUsedFont();
                string plainText = Text;
                if (plainText.IndexOf("_") > 0 && DroppedTag.Length > 0)
                    plainText = plainText.Replace("_", DroppedTag);
                if (plainText.Length != 0)
                {
                    textSize = rt.MeasureString(context, plainText, bounds.Width);
                }

                Rectangle textRect = bounds;

                if (argm == SMContentArangement.ImageAbove)
                {
                    textRect.Height = textSize.Height;
                    textRect.Y = bounds.Bottom - textRect.Height;
                    textRect.X = (textRect.Left + textRect.Right) / 2 - textSize.Width/2;
                    textRect.Width = textSize.Width;
                    imgRect.Height = bounds.Height - textRect.Height - ContentPadding.Top;
                }
                else if (argm == SMContentArangement.ImageBelow)
                {
                    textRect.Height = textSize.Height;
                    textRect.X = (textRect.Left + textRect.Right) / 2 - textSize.Width/2;
                    textRect.Width = textSize.Width;
                    imgRect.Y = textRect.Bottom + ContentPadding.Bottom;
                    imgRect.Height = bounds.Height - textRect.Height - ContentPadding.Bottom;
                }
                else if (argm == SMContentArangement.ImageOnLeft)
                {
                    textRect.Width = textSize.Width;
                    textRect.X = bounds.Right - textSize.Width;
                    imgRect.Width = bounds.Width - textSize.Width - ContentPadding.Left;
                }
                else if (argm == SMContentArangement.ImageOnRight)
                {
                    textRect.Width = textSize.Width;
                    imgRect.X = textRect.Right + ContentPadding.Right;
                    imgRect.Width = bounds.Width - textSize.Width - ContentPadding.Right;
                }
                else if (argm == SMContentArangement.ImageOnly)
                {
                    textRect = Rectangle.Empty;
                }
                else if (argm == SMContentArangement.TextOnly)
                {
                    imgRect = Rectangle.Empty;
                }


                if (!imgRect.IsEmpty)
                {
                    Rectangle rc = DrawImage(context, imgRect, image, ContentScaling, SourceOffsetX, SourceOffsetY);
                    if (ContentScaling == SMContentScaling.Fill)
                    {
                        showRect = imgRect;
                        sourceRect = rc;
                    }
                    else
                    {
                        showRect = rc;
                        sourceRect = new Rectangle(0, 0, image.Width, image.Height);
                    }
                }

                if (!textRect.IsEmpty)
                {
                    textRect.Inflate(1, 1);
                    rt.DrawString(context, plainText, textRect);
                }
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
                br.Log("* * * SMImage * * *\n");
                byte tag;
                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10:
                            Img.ImageId = br.ReadInt64();
                            ContentScaling = (SMContentScaling)br.ReadInt32();
                            break;
                        case 13:
                            ContentArangement = (SMContentArangement)br.ReadInt32();
                            break;
                        case 14:
                            SourceOffsetX = br.ReadInt32();
                            SourceOffsetY = br.ReadInt32();
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

            MNReferencedSpot spot = Img.Image.FindSpot(showRect, sourceRect, dc.lastPoint);
            if (spot != null)
            {
                MNReferencedCore obj = Document.FindContentObject(spot.ContentType, spot.ContentId);

                if (obj != null && (obj is MNReferencedSound) && Document.HasViewer)
                {
                    Document.Viewer.OnEvent("OnPlaySound", obj as MNReferencedSound);
                } 
            }
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

            DroppedTag = dc.draggedItem.Tag;
            DroppedItems.Clear();

            if (base.OnDropFinished(dc))
            {
                if (dc.draggedItem.Image != null)
                {
                    MNReferencedImage mri = new MNReferencedImage();
                    mri.ImageData = dc.draggedItem.Image;
                    DroppedTag = dc.draggedItem.Tag;
                    Img.Image = mri;
                }

                return true;
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.Log("* * * SMImage * * *\n");
            bw.WriteByte(10);
            bw.WriteInt64(Img.ImageId);
            bw.WriteInt32((Int32)ContentScaling);

            bw.WriteByte(13);
            bw.WriteInt32((Int32)ContentArangement);

            bw.WriteByte(14);
            bw.WriteInt32(SourceOffsetX);
            bw.WriteInt32(SourceOffsetY);

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
