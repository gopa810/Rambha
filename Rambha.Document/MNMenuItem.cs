using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;
using Rambha.Script;

namespace Rambha.Document
{
    public class MNMenuItem
    {
        public MNReferencedImage Image
        {
            get
            {
                if (p_image == null && image_id > 0 && doc != null)
                    p_image = doc.FindImage(image_id);
                return p_image;
            }
            set
            {
                p_image = value;
            }
        }
        public long ImageId
        {
            get { return p_image == null ? -1 : p_image.Id; }
            set { image_id = value; }
        }
        private MNReferencedImage p_image = null;
        private MNDocument doc = null;
        private long image_id = -1;

        public string Text { get; set; }

        public string ActionScript { get; set; }


        public Rectangle drawRect; 

        public MNMenuItem()
        {
            Image = null;
            Text = "MenuItem";
            ActionScript = "";
        }

        public override string ToString()
        {
            return string.Format("MenuItem: {0}", Text);
        }

        public void Load(MNDocument document, RSFileReader br)
        {
            doc = document;
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        ImageId = br.ReadInt64();
                        break;
                    case 11:
                        Text = br.ReadString();
                        break;
                    case 12:
                        ActionScript = br.ReadString();
                        break;
                }
            }
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt64(ImageId);

            bw.WriteByte(11);
            bw.WriteString(Text);

            bw.WriteByte(12);
            bw.WriteString(ActionScript);

            bw.WriteByte(0);
        }
    }
}
