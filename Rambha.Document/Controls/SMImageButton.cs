using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Xml;
using System.IO;

using Rambha.Serializer;

namespace Rambha.Document
{
    [Serializable()]
    public class SMImageButton: SMControl
    {
        [Browsable(true),Category("Layout")]
        public SMContentArangement ContentArangement { get; set; }

        public MNLazyImage ImgA = null;

        [Browsable(true), Category("Content")]
        public string BuiltInImage { get; set; }

        private Rectangle showRect = Rectangle.Empty;
        private SMRichText rt = null;


        public SMImageButton(MNPage p): base(p)
        {
            ImgA = new MNLazyImage(p.Document);
            Evaluation = MNEvaluationType.None;
            ContentArangement = SMContentArangement.ImageOnly;
            Clickable = true;
            rt = new SMRichText(this);

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

            SMStatusLayout layout = PrepareBrushesAndPens();

            DrawStyledBackground(context, layout, rect);
            DrawStyledBorder(context, layout, rect);

            Rectangle bounds = ContentPadding.ApplyPadding(rect);
            Rectangle imageBounds = Rectangle.Empty;
            Rectangle textBounds = Rectangle.Empty;
            SMContentArangement argm = this.ContentArangement;
            SMContentScaling ContentScaling = SMContentScaling.Fit;

            Image image = GetContentImage();
            if (image == null)
                argm = SMContentArangement.TextOnly;

            if (argm == SMContentArangement.ImageOnly)
            {
                SMContentScaling scaling = ContentScaling;
                Rectangle rc = DrawImage(context, null, bounds, image, scaling, 0, 0);
                showRect = rc;
            }
            else
            {
                Rectangle imgRect = bounds;
                Size textSize = Size.Empty;
                Font usedFont = GetUsedFont();
                string plainText = Text;
                if (plainText.Length != 0)
                {
                    textSize = rt.MeasureString(context, plainText, bounds.Width);
                }

                Rectangle textRect = bounds;

                if (argm == SMContentArangement.ImageAbove)
                {
                    textRect.Height = textSize.Height;
                    textRect.Y = bounds.Bottom - textRect.Height;
                    textRect.X = (textRect.Left + textRect.Right) / 2 - textSize.Width / 2;
                    textRect.Width = textSize.Width;
                    imgRect.Height = bounds.Height - textRect.Height - ContentPadding.Top;
                }
                else if (argm == SMContentArangement.ImageBelow)
                {
                    textRect.Height = textSize.Height;
                    textRect.X = (textRect.Left + textRect.Right) / 2 - textSize.Width / 2;
                    textRect.Width = textSize.Width;
                    imgRect.Y = textRect.Bottom + ContentPadding.Bottom;
                    imgRect.Height = bounds.Height - textRect.Height - ContentPadding.Bottom;
                }
                else if (argm == SMContentArangement.ImageOnLeft)
                {
                    imgRect.Size = SMControl.GetImageDrawSize(bounds, image);
                    if (bounds.Width - imgRect.Width < textSize.Width + ContentPadding.LeftRight)
                    {
                        imgRect.Width = bounds.Width - textSize.Width - ContentPadding.LeftRight;
                        imgRect.Size = SMControl.GetImageDrawSize(imgRect, image);
                    }
                    textRect.Width = bounds.Width - imgRect.Width - ContentPadding.LeftRight;
                    textRect.X = bounds.Right - textRect.Width;
                    imgRect.Y = (bounds.Top + bounds.Bottom)/2 - imgRect.Height/2;
                }
                else if (argm == SMContentArangement.ImageOnRight)
                {
                    imgRect.Size = SMControl.GetImageDrawSize(bounds, image);
                    if (bounds.Width - imgRect.Width < textSize.Width + ContentPadding.LeftRight)
                    {
                        imgRect.Width = bounds.Width - textSize.Width - ContentPadding.LeftRight;
                        imgRect.Size = SMControl.GetImageDrawSize(imgRect, image);
                    }
                    textRect.Width = bounds.Width - imgRect.Width - ContentPadding.LeftRight;
                    imgRect.X = textRect.Right + ContentPadding.Right;
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
                    Rectangle rc = DrawImage(context, null, imgRect, image, ContentScaling, 0, 0);
                    showRect = rc;
                }

                if (!textRect.IsEmpty)
                {
                    textRect.Inflate(1, 1);
                    rt.DrawString(context, layout, plainText, textRect);
                }
            }

            base.Paint(context);
        }


        public override void ExportToHtml(MNExportContext ctx, int zorder, StringBuilder sbHtml, StringBuilder sbCss, StringBuilder sbJS)
        {
            Rectangle rect = Area.RelativeArea;

            SMStatusLayout layout = PrepareBrushesAndPens();

            Rectangle bounds = ContentPadding.ApplyPadding(rect);
            SMContentArangement argm = this.ContentArangement;
            Rectangle imgRect = bounds;

            Image image = GetContentImage();
            if (string.IsNullOrEmpty(BuiltInImage))
                argm = SMContentArangement.TextOnly;



            string plainText = Text;


            string blockFormat = Font.HtmlString() + Paragraph.Html() + ContentPaddingHtml() + "position:absolute;" + Area.HtmlLTRB();
            sbCss.AppendFormat(".c{0}n {{ {1} {2} cursor:pointer; }}\n", Id, HtmlFormatColor(false), blockFormat);
            sbCss.AppendFormat(".c{0}h {{ {1} {2} cursor:pointer; }}\n", Id, HtmlFormatColor(true), blockFormat);
            string imgText = "", textText = "";



            if (argm != SMContentArangement.TextOnly)
            {
                imgText = string.Format("<img src=\"../rs/{1}.png\" style='object-fit:contain;width:100%;height:100%'></td>\n", Id, BuiltInImage != null ? BuiltInImage : "default");
            }
            if (argm != SMContentArangement.ImageOnly)
            {
                // wrapping text into vertical/horizontal alignment DIV
                textText += plainText;
            }

            string onclick = GetOnclickHtml();

            sbHtml.AppendFormat("<div class=\"c{0}n\" onclick=\"{1}\"", Id, onclick);
            sbHtml.Append(">");

            switch (argm)
            {
                case SMContentArangement.ImageAbove:
                    sbHtml.AppendFormat("  <table class=\"c{0}n\"", Id);
                    if (onclick.Length > 0) sbHtml.AppendFormat(" onclick=\"{0}\"", onclick);
                    sbHtml.Append(">\n");
                    sbHtml.Append("<tr><td>");
                    sbHtml.Append(imgText);
                    sbHtml.Append("<tr><td>");
                    sbHtml.Append(textText);
                    sbHtml.Append("</table>\n");
                    break;
                case SMContentArangement.ImageBelow:
                    sbHtml.AppendFormat("  <table class=\"c{0}n\"", Id);
                    if (onclick.Length > 0) sbHtml.AppendFormat(" onclick=\"{0}\"", onclick);
                    sbHtml.Append(">\n");
                    sbHtml.Append("<tr><td>");
                    sbHtml.Append(textText);
                    sbHtml.Append("<tr><td>");
                    sbHtml.Append(imgText);
                    sbHtml.Append("</table>\n");
                    break;
                case SMContentArangement.ImageOnLeft:
                    sbHtml.Append(string.Format("<img src=\"../rs/{1}.png\" style='object-fit:contain;float:left;height:100%'>\n", Id, BuiltInImage != null ? BuiltInImage : "default"));
                    sbHtml.Append("<div class=\"vertCenter\"><div>" + textText);
                    sbHtml.Append("</div></div>\n");
                    break;
                case SMContentArangement.ImageOnRight:
                    sbHtml.Append(string.Format("<img src=\"../rs/{1}.png\" style='object-fit:contain;float:right;height:100%'>\n", Id, BuiltInImage != null ? BuiltInImage : "default"));
                    sbHtml.Append("<div class=\"vertCenter\"><div>" + textText);
                    sbHtml.Append("</div></div>\n");
                    break;
                case SMContentArangement.TextOnly:
                    sbHtml.AppendFormat("  <div class=\"c{0}n\" style='display:flex;flex-direction:column;justify-content:center;'", Id);
                    if (onclick.Length > 0) sbHtml.AppendFormat(" onclick=\"{0}\"", onclick);
                    sbHtml.Append("><div>\n");
                    sbHtml.Append(textText);
                    sbHtml.Append("</div></div>\n");
                    break;
                case SMContentArangement.ImageOnly:
                    sbHtml.AppendFormat("  <div class=\"c{0}n\" onclick=\"{1}\">\n", Id, onclick);
                    sbHtml.Append(imgText);
                    sbHtml.Append("</div>\n");
                    break;
            }

            sbHtml.Append("</div>");
        }
        private System.Drawing.Image GetContentImage()
        {
            if (BuiltInImage == null || Document == null)
                return null; 
            if (BuiltInImage.Length > 0 && Document.HasViewer)
                return Document.Viewer.GetBuiltInImage(BuiltInImage);
            return ImgA.ImageData;
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
                            br.ReadInt64();
                            break;
                        case 13:
                            ContentArangement = (SMContentArangement)br.ReadInt32();
                            break;
                        case 14:
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
            bw.WriteByte(13);
            bw.WriteInt32((Int32)ContentArangement);
            if (BuiltInImage != null)
            {
                bw.WriteByte(14);
                bw.WriteString(BuiltInImage);
            } 

            bw.WriteByte(0);
        }

    }
}
