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
                        MNMenuItem mi = new MNMenuItem();
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
            int itemWidth = 64;
            int itemHeight = 64;
            int padding = 32;

            SizeF titleSize = SizeF.Empty;
            bool hasTitle = false;
            if (UserTitle.Length > 0)
            {
                titleSize = Context.g.MeasureString(UserTitle, Context.MenuTitleFont);
                hasTitle = true;
            }

            SizeF itemTextSize = Context.g.MeasureString("ITEM", Context.MenuFont);
            Size itemsRect = new Size(itemWidth * Items.Count + (Items.Count > 0 ? Items.Count - 1 : 0) * spaceItems, itemHeight);

            int ay = padding;
            int by = ay + (hasTitle ? (int)titleSize.Height + spaceItems : 0);
            int cy = by + itemHeight + spaceItems;
            int dy = cy + (int)itemTextSize.Height;
            int ey = dy + padding;

            int width = Math.Max((int)titleSize.Width, itemsRect.Width) + 2 * padding;

            drawRect = new Rectangle(Context.PageWidth / 2 - width / 2, Context.PageHeight / 2 - ey / 2,
                width, ey);

            int currX = (drawRect.Left + drawRect.Right) / 2 - itemsRect.Width / 2;
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].drawRect = new Rectangle(currX, drawRect.Top + by, itemWidth, dy - by);
                currX += itemWidth + spaceItems;
            }

            // draw menu
            Context.g.FillRectangle(Context.semitransparentGrayBrush, 0, 0, Context.PageWidth, Context.PageHeight);
            Context.g.FillRectangle(Brushes.White, drawRect);
            Context.g.DrawRectangle(Pens.Black, drawRect);

            if (hasTitle)
            {
                Context.g.DrawString(UserTitle, Context.MenuTitleFont, Brushes.DarkGray, 
                    Context.PageWidth / 2 - (int)titleSize.Width / 2, drawRect.Top + ay);
            }

            int ix = 0;
            foreach (MNMenuItem mi in Items)
            {
                if (ix == Context.selectedMenuItem)
                {
                    Context.g.FillRectangle(Brushes.LightSkyBlue, mi.drawRect);
                }
                Context.g.DrawImage(mi.Image.ImageData, mi.drawRect.Left, 
                    mi.drawRect.Top, itemWidth, itemHeight);
                SizeF textSize = Context.g.MeasureString(mi.Text, Context.MenuFont);
                Context.g.DrawString(mi.Text, Context.MenuFont, Brushes.Black, 
                    mi.drawRect.Left + mi.drawRect.Width / 2 - (int)textSize.Width / 2, drawRect.Top + cy);
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
                if (mi.drawRect.Contains(context.lastPoint))
                {
                    return i;
                }
                i++;
            }

            return -1;
        }
    }
}
