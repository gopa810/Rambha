using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.IO;

using Rambha.Serializer;


namespace Rambha.Document
{
    [Serializable()]
    public class MNReferencedImage
    {
        public long Id { get; set; }
        public MNDocument Document { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public Image ImageData { get; set; }
        public string Description { get; set; }

        public int ItemHeight = 0;
        public int ItemTextHeight = 0;

        public MNReferencedImage(MNDocument doc)
        {
            Document = doc;
            Title = "";
            FilePath = "";
            Description = "";
        }

        public override string ToString()
        {
            return Title;
        }


        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt64(Id);

            bw.WriteByte(11);
            bw.WriteString(Title);
            bw.WriteString(FilePath);
            bw.WriteString(Description);

            bw.WriteByte(12);
            bw.WriteImage(ImageData);

            // end of object
            bw.WriteByte(0);
        }

        public bool Load(RSFileReader br)
        {
            byte tag;
            int a;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10: Id = br.ReadInt64(); break;
                    case 11:
                        Title = br.ReadString();
                        FilePath = br.ReadString();
                        Description = br.ReadString();
                        break;
                    case 12:
                        ImageData = br.ReadImage();
                        break;
                    default:
                        break;
                }
            }


            return true;
        }

    }
}
