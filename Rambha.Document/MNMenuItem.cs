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
        public Image Image
        {
            get
            {
                if (Document.HasViewer)
                {
                    return Document.Viewer.GetBuiltInImage(ImageName);
                }
                return null;
            }
        }
        public string ImageName {get;set;}

        public MNDocument Document = null;

        public string Text { get; set; }

        public string ActionScript { get; set; }


        public Rectangle drawRect; 

        public MNMenuItem(MNDocument d)
        {
            Document = d;
            ImageName = "";
            Text = "";
            ActionScript = "";
        }

        public override string ToString()
        {
            return string.Format("MenuItem: {0}", Text);
        }

        public bool IsSeparator
        {
            get
            {
                return Text.Length == 0;
            }
        }

        public void Load(MNDocument document, RSFileReader br)
        {
            Document = document;
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        ImageName = br.ReadString();
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
            bw.WriteString(ImageName);

            bw.WriteByte(11);
            bw.WriteString(Text);

            bw.WriteByte(12);
            bw.WriteString(ActionScript);

            bw.WriteByte(0);
        }
    }
}
