using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordImage : SMWordBase
    {
        public Image image = null;
        public Size imageSize = Size.Empty;

        public override SMTokenItem GetDraggableItem()
        {
            SMTokenItem item = new SMTokenItem();
            item.Tag = tag;
            item.Image = image;
            item.ContentSize = Size.Empty;
            return item;
        }

        public SMWordImage(SMFont f)
            : base(f)
        {
        }

        public SMWordImage(SMFont f, SMTokenItem item)
            : base(f)
        {
            tag = item.Tag;
            image = item.Image;
        }

        public override void Paint(MNPageContext context, SMStatusLayout layout, int X, int Y)
        {
            context.g.DrawImage(image, rect.X + X, rect.Y + Y, rect.Width, rect.Height);
        }
    }


}
