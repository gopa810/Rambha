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
    public class SMImageButton: SMControl
    {
        [Browsable(true),Category("Layout")]
        public SMContentArangement ContentArangement { get; set; }

        public MNLazyImage ImgA = null;
        public MNLazyImage ImgB = null;

        private Rectangle showRect = Rectangle.Empty;

        public SMImageButton(MNPage p): base(p)
        {
            ImgA = new MNLazyImage(p.Document);
            ImgB = new MNLazyImage(p.Document);
            Evaluation = MNEvaluationType.None;
            ContentArangement = SMContentArangement.ImageOnly;
            Clickable = true;
        }

        public override Size GetDefaultSize()
        {
            return new Size(64,64);
        }

        public override void Paint(MNPageContext context)
        {
            Graphics g = context.g;
            SMImageButton pi = this;
            Rectangle rect = Area.GetBounds(context);

            PrepareBrushesAndPens();

            DrawStyledBorder(context, rect);

            Rectangle bounds = ContentPadding.ApplyPadding(rect);
            Rectangle imageBounds = Rectangle.Empty;
            Rectangle textBounds = Rectangle.Empty;

            Image image = GetContentImage();
            SMContentArangement scaling = ContentArangement;
            if (scaling != SMContentArangement.TextOnly && image == null)
                scaling = SMContentArangement.TextOnly;

            if (scaling == SMContentArangement.ImageOnly)
            {
                imageBounds = bounds;
                Size imageSize = image.Size;
                if (imageSize.Width > 0 && imageBounds.Width > 0 && imageSize.Height > 0 && imageBounds.Height > 0)
                {
                    double ratio = Math.Max(imageSize.Width / Convert.ToDouble(imageBounds.Width),
                        imageSize.Height / Convert.ToDouble(imageBounds.Height));
                    imageSize = new Size(Convert.ToInt32(imageSize.Width / ratio), Convert.ToInt32(imageSize.Height / ratio));
                }

                switch (GetVerticalAlign())
                {
                    case SMVerticalAlign.Top:
                        break;
                    case SMVerticalAlign.Center:
                        imageBounds.Y += (imageBounds.Height - imageSize.Height) / 2;
                        break;
                    case SMVerticalAlign.Bottom:
                        imageBounds.Y += (imageBounds.Height - imageSize.Height);
                        break;
                }
                switch (GetHorizontalAlign())
                {
                    case SMHorizontalAlign.Left:
                        break;
                    case SMHorizontalAlign.Center:
                    case SMHorizontalAlign.Justify:
                        imageBounds.X += (imageBounds.Width - imageSize.Width)/2;
                        break;
                    case SMHorizontalAlign.Right:
                        imageBounds.X += (imageBounds.Width - imageSize.Width);
                        break;
                }
                imageBounds.Width = imageSize.Width;
                imageBounds.Height = imageSize.Height;

            }
            else if (scaling == SMContentArangement.TextOnly)
            {
                SizeF sizef = context.g.MeasureString(Text, Font.Font);
                Size textSize = new Size((int)sizef.Width + 1, (int)sizef.Height + 1);
                textBounds.Size = textSize;
                switch (GetVerticalAlign())
                {
                    case SMVerticalAlign.Top:
                        textBounds.Y = bounds.Y;
                        break;
                    case SMVerticalAlign.Center:
                        textBounds.Y = bounds.Y + bounds.Height / 2 - textSize.Height / 2;
                        break;
                    case SMVerticalAlign.Bottom:
                        textBounds.Y = bounds.Bottom - textSize.Height;
                        break;
                }
                switch (GetHorizontalAlign())
                {
                    case SMHorizontalAlign.Left:
                        textBounds.X = bounds.X;
                        break;
                    case SMHorizontalAlign.Center:
                    case SMHorizontalAlign.Justify:
                        textBounds.X = bounds.X + bounds.Width / 2 - textSize.Width / 2;
                        break;
                    case SMHorizontalAlign.Right:
                        textBounds.X = bounds.Right - textSize.Width;
                        break;
                }
            }

            if (!imageBounds.IsEmpty)
            {
                g.DrawImage(image, imageBounds);
                showRect = imageBounds;
            }

            if (!textBounds.IsEmpty)
            {
                g.DrawString(Text, Font.Font, tempForeBrush, textBounds);
            }

            base.Paint(context);
        }

        private System.Drawing.Image GetContentImage()
        {
            return (UIStatePressed && (ImgB.ImageData != null)) ? ImgB.ImageData : ImgA.ImageData;
        }

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                br.Log("* * * SMImageButton * * *\n");
                byte tag;
                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10:
                            ImgA.ImageId = br.ReadInt64();
                            break;
                        case 11:
                            ImgB.ImageId = br.ReadInt64();
                            break;
                        case 13:
                            ContentArangement = (SMContentArangement)br.ReadInt32();
                            break;
                        default: 
                            return false;
                    }

                }

                return true;
            }

            return false;
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


        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.Log("* * * SMImageButton * * *\n");
            bw.WriteByte(10);
            bw.WriteInt64(ImgA.ImageId);
            bw.WriteByte(11);
            bw.WriteInt64(ImgB.ImageId);
            bw.WriteByte(13);
            bw.WriteInt32((Int32)ContentArangement);

            bw.WriteByte(0);
        }

    }
}
