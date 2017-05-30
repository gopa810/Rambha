using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;
using Rambha.Script;

namespace Rambha.Document
{
    public class MNMenu
    {
        public string APIName { get; set; }

        public string UserTitle { get; set; }

        public List<MNMenuItem> Items = new List<MNMenuItem>();


        public Rectangle drawRect;

        public MNMenu()
        {
            APIName = "menu1";
            UserTitle = "";

        }

        public void Load(MNDocument document, RSFileReader br)
        {
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        APIName = br.ReadString();
                        break;
                    case 11:
                        UserTitle = br.ReadString();
                        break;
                    case 20:
                        MNMenuItem mi = new MNMenuItem(document);
                        mi.Load(document, br);
                        Items.Add(mi);
                        break;
                }
            }
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteString(APIName);

            bw.WriteByte(11);
            bw.WriteString(UserTitle);

            foreach (MNMenuItem mi in Items)
            {
                bw.WriteByte(20);
                mi.Save(bw);
            }

            bw.WriteByte(0);
        }

        public void Paint(MNPageContext Context)
        {
            // if needed, calculate dimensions
            int spaceItems = 16;
            int itemHeight = 64;
            int padding = 32;

            SizeF titleSize = SizeF.Empty;
            bool hasTitle = false;
            if (UserTitle.Length > 0)
            {
                titleSize = Context.g.MeasureString(UserTitle, Context.MenuTitleFont);
                hasTitle = true;
            }

            float maxItemWidth = 324f;
            float itemsAreaHeight = 0;
            foreach (MNMenuItem mi in Items)
            {
                SizeF szf = Context.g.MeasureString(mi.Text, Context.MenuFont);
                maxItemWidth = Math.Max(maxItemWidth, szf.Width);
                itemsAreaHeight += mi.IsSeparator ? spaceItems : itemHeight;
            }

            int ay = padding;
            int by = ay + (hasTitle ? (int)titleSize.Height + spaceItems : 0);
            int cy = by + (int)itemsAreaHeight + padding;

            int width = (int)Math.Max(titleSize.Width, maxItemWidth) + 2 * padding;

            drawRect = new Rectangle(Context.PageWidth / 2 - width / 2, Context.PageHeight / 2 - cy / 2,
                width, cy);

            // draw menu
            Context.g.FillRectangle(Context.semitransparentGrayBrush, 0, 0, Context.PageWidth, Context.PageHeight);
            Context.g.FillRectangle(Brushes.White, drawRect);
            Context.g.DrawRectangle(Pens.Black, drawRect);

            if (hasTitle)
            {
                Context.g.DrawString(UserTitle, Context.MenuTitleFont, Brushes.DarkGray, 
                    Context.PageWidth / 2 - (int)titleSize.Width / 2, drawRect.Top + ay);
                Context.g.FillRectangle(Brushes.Gray, drawRect.X + 8, drawRect.Top + (int)titleSize.Height + 2 * ay, drawRect.Width - 16, 3);
            }

            int ix = 0;
            int iy = by;
            foreach (MNMenuItem mi in Items)
            {
                mi.drawRect.X = drawRect.X;
                mi.drawRect.Y = drawRect.Top + iy;
                mi.drawRect.Width = drawRect.Width;
                mi.drawRect.Height = mi.IsSeparator ? spaceItems : itemHeight;

                if (ix == Context.selectedMenuItem)
                {
                    Context.g.FillRectangle(Brushes.LightSkyBlue, mi.drawRect);
                }

                if (!mi.IsSeparator)
                {
                    Context.g.DrawImage(mi.Image, drawRect.Left + 8,
                        mi.drawRect.Top + 8, itemHeight - 16, itemHeight - 16);
                    Rectangle rtext = new Rectangle(mi.drawRect.Left + itemHeight + 16, mi.drawRect.Top, 
                        mi.drawRect.Width - itemHeight - 16, itemHeight);
                    Context.g.DrawString(mi.Text, Context.MenuFont, Brushes.Black, rtext, SMGraphics.StrFormatLeftCenter);
                    iy += itemHeight;
                }
                else
                {
                    Context.g.DrawLine(Pens.Gray, mi.drawRect.Left + spaceItems, mi.drawRect.Top + spaceItems/2, 
                        mi.drawRect.Right - spaceItems, mi.drawRect.Top + spaceItems/2);
                    iy += spaceItems;
                }
                ix++;
            }
        }

        /// <summary>
        /// Tests context.lastPoint
        /// </summary>
        /// <param name="context"></param>
        public int TestHit(PVDragContext context)
        {
            int i = 0;
            foreach (MNMenuItem mi in Items)
            {
                if (mi.drawRect.Contains(context.lastPoint) && !mi.IsSeparator)
                {
                    return i;
                }
                i++;
            }

            return -1;
        }
    }
}
