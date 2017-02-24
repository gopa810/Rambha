using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class MNLazyImage
    {
        public MNDocument Document = null;

        public MNLazyImage(MNDocument doc)
        {
            Document = doc;
        }

        public MNLazyImage(MNLazyImage li)
        {
            Document = li.Document;
            Image = li.Image;
            image_id = Image.Id;
        }

        public MNReferencedImage Image
        {
            get
            {
                if (p_image == null && image_id > 0 && Document != null)
                    p_image = Document.FindImage(image_id);
                return p_image;
            }
            set
            {
                p_image = value;
            }
        }

        public Image ImageData
        {
            get
            {
                MNReferencedImage img = Image;
                if (img != null)
                    return img.ImageData;
                return null;
            }
        }

        public long ImageId
        {
            get { return p_image == null ? image_id : p_image.Id; }
            set { image_id = value; }
        }
        private MNReferencedImage p_image = null;
        private long image_id = -1;
    }
}
