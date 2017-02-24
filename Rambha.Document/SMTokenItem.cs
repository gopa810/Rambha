using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMTokenItem: GSCore
    {
        public string Tag = "";
        public string Text = null;
        public Size ContentSize = Size.Empty;
        public Image Image = null;

        public void Load(RSFileReader br)
        {
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        Tag = br.ReadString();
                        break;
                    case 11:
                        Text = br.ReadString();
                        break;
                    case 12:
                        ContentSize = new Size(br.ReadInt32(), br.ReadInt32());
                        break;
                    case 13:
                        Image = br.ReadImage();
                        break;
                }
            }
        }

        public void Save(RSFileWriter bw)
        {
            if (Tag != null)
            {
                bw.WriteByte(10);
                bw.WriteString(Tag);
            }

            if (Text != null)
            {
                bw.WriteByte(11);
                bw.WriteString(Text);
            }

            if (!ContentSize.IsEmpty)
            {
                bw.WriteByte(12);
                bw.WriteInt32(ContentSize.Width);
                bw.WriteInt32(ContentSize.Height);
            }

            if (Image != null)
            {
                bw.WriteByte(13);
                bw.WriteImage(Image);
            }

            bw.WriteByte(0);
        }
    }
}
