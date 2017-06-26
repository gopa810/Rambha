using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Xml;
using System.IO;
using System.Diagnostics;

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

        [Browsable(true), Category("Content")]
        public string BuiltInImage { get; set; }

        public MNLazyImage Img = null;

        public string DroppedTag { get; set; }
        public string DroppedText { get; set; }
        public Image DroppedImage { get; set; }

        private Rectangle sourceRect = Rectangle.Empty;
        private Rectangle showRect = Rectangle.Empty;

        private SMRichText rt = null;

        public MNReferencedSpot HoverSpot = null;

        public SMImage(MNPage p): base(p)
        {
            Img = new MNLazyImage(p.Document);
            ContentScaling = SMContentScaling.Fit;
            Evaluation = MNEvaluationType.Inherited;
            DroppedTag = "";
            DroppedImage = null;
            DroppedText = "";
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

            SMStatusLayout layout = PrepareBrushesAndPens();

            Debugger.Log(0, "", "State of " + Text + " Image is " + UIStateChecked.ToString() + "\n");
            Rectangle bounds = ContentPadding.ApplyPadding(rect);
            SMContentArangement argm = this.ContentArangement;
            MNReferencedImage refImage = null;
            Rectangle imgRect = bounds;

            Image image = GetContentImage(out refImage);
            if (image == null)
                argm = SMContentArangement.TextOnly;

            if (ExpectedChecked != Bool3.Undef)
            {
                DrawStyledBackground(context, layout, bounds);
            }

            if (argm == SMContentArangement.ImageOnly)
            {
                SMContentScaling scaling = ContentScaling;
                Rectangle rc = DrawImage(context, layout, bounds, image, scaling, SourceOffsetX, SourceOffsetY);
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
                Size textSize = Size.Empty;
                Font usedFont = GetUsedFont();
                string plainText = Text.Length > 0 ? Text : DroppedText;
                if (plainText.IndexOf("_") >= 0 && DroppedTag.Length > 0)
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
                    Rectangle rc = DrawImage(context, layout, imgRect, image, ContentScaling, SourceOffsetX, SourceOffsetY);
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
                    if (argm == SMContentArangement.TextOnly)
                    {
                        DrawStyledBorder(context, layout, bounds);
                    }
                    textRect.Inflate(2, 2);
                    rt.DrawString(context, layout, plainText, textRect);
                }
            }

            if (!imgRect.IsEmpty && refImage != null && refImage.HasSpots())
            {
                foreach (MNReferencedSpot rs in refImage.SafeSpots)
                {
                    if (rs.ContentType != SMContentType.TaggedArea) continue;
                    if (rs.UIStateHighlighted || (HoverSpot == rs))
                    {
                        rs.Paint(context.g, showRect, false, context.SpotAreaBorderPen, null);
                    }
                }
            }

            if (UIStateError == MNEvaluationResult.Incorrect && UIStateChecked)
            {
                if (Document.HasViewer)
                {
                    Document.Viewer.ScheduleCall(MNNotificationCenter.RectifyDelay, this, "clearCheck");
                }
            }

            base.Paint(context);
        }

        private System.Drawing.Image GetContentImage(out MNReferencedImage refImage)
        {
            if (DroppedImage != null)
            {
                refImage = null;
                return DroppedImage;
            }

            if (BuiltInImage != null && Document != null)
            {
                if (BuiltInImage.Length > 0 && Document.HasViewer)
                {
                    refImage = null;
                    return Document.Viewer.GetBuiltInImage(BuiltInImage);
                }
            }

            refImage = Img.Image;
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
                        case 15:
                            BuiltInImage = br.ReadString();
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
                item.Tag = SafeTag;
                item.Image = Img.ImageData;
                if (Text != null && Text.Length > 0)
                {
                    item.Text = Text;
                    item.ContentSize = Size.Empty;
                }
                else
                {
                    item.Text = "";
                    item.ContentSize = RestrictSize(Img.ImageData.Size, 32);
                }
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
            MNReferencedImage img = Img.Image;
            if (img == null)
                return;

            MNReferencedSpot spot = img.FindSpot(showRect, sourceRect, dc.lastPoint);
            if (spot != null)
            {
                if (spot.ContentType != SMContentType.TaggedArea)
                {
                    MNReferencedCore obj = Document.FindContentObject(spot.ContentType, spot.ContentId);

                    if (obj != null && (obj is MNReferencedSound) && Document.HasViewer)
                    {
                        Document.Viewer.OnEvent("OnPlaySound", obj as MNReferencedSound);
                    }
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

        public override void OnDropMove(PVDragContext dc)
        {
            HoverSpot = null;
            if (Img != null && Img.Image != null)
            {
                Debugger.Log(0, "", string.Format("Finding spot {0} :: {1} :: {2}\n", showRect, sourceRect, dc.lastPoint));
                MNReferencedSpot spot = Img.Image.FindSpot(showRect, sourceRect, dc.lastPoint);
                if (spot != null && spot.ContentType == SMContentType.TaggedArea)
                {
                    HoverSpot = spot;
                }
            }

            base.OnDropMove(dc);
        }

        public override bool OnDropFinished(PVDragContext dc)
        {
            HoverSpot = null;
            if (HasImmediateEvaluation)
            {
                if (Img != null && Img.Image != null)
                {
                    MNReferencedImage img = Img.Image;
                    MNReferencedSpot spot = img.FindSpot(showRect, sourceRect, dc.lastPoint);
                    if (spot != null && spot.ContentType == SMContentType.TaggedArea
                        && dc.draggedItem.Tag.Equals(spot.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        spot.UIStateHighlighted = true;
                        return base.OnDropFinished(dc);
                    }
                }

                if (!Tag.Equals(dc.draggedItem.Tag, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
            }

            DroppedImage = dc.draggedItem.Image;
            DroppedTag = dc.draggedItem.Tag;
            DroppedText = dc.draggedItem.Text;

            return base.OnDropFinished(dc);
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

            if (BuiltInImage != null)
            {
                bw.WriteByte(15);
                bw.WriteString(BuiltInImage);
            }

            bw.WriteByte(0);
        }

        public override MNEvaluationResult Evaluate()
        {
            UIStateError = GenericCheckedEvaluation();

            if (HasLazyEvaluation)
            {
                if (DroppedTag.ToLower().Equals(Tag.ToLower()))
                    UIStateError = MNEvaluationResult.Correct;
                else
                    UIStateError = MNEvaluationResult.Incorrect;
            }

            return UIStateError;
        }

        public override void SaveStatus(RSFileWriter bw)
        {
            base.SaveStatusCore(bw);

            if (Img != null && Img.Image != null && Img.Image.HasSpots())
            {
                foreach (MNReferencedSpot rs in Img.Image.SafeSpots)
                {
                    // tag for spot status info
                    bw.WriteByte(15);
                    rs.SaveStatus(bw);
                }
            }
            bw.WriteByte(0);
        }

        public override void LoadStatus(RSFileReader br)
        {
            base.LoadStatusCore(br);
            byte b;
            byte b2;
            int currentSpotIndex = 0;
            List<MNReferencedSpot> spots = null;
            if (Img != null && Img.Image != null && Img.Image.HasSpots())
                spots = Img.Image.SafeSpots;

            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 15:
                        if (spots != null && currentSpotIndex < spots.Count)
                        {
                            spots[currentSpotIndex].LoadStatus(br);
                        }
                        break;
                }
            }
        }

        public override void ResetStatus()
        {
            DroppedTag = "";
            DroppedText = "";
            DroppedImage = null;
            base.ResetStatus();
        }

    }
}
