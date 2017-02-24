using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class MNReferencedText: MNReferencedCore
    {
        public string Text { get; set; }

        public MNReferencedText()
        {
            Name = string.Empty;
            Text = string.Empty;
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteString(Name);
            bw.WriteByte(12);
            bw.WriteString(Text);

            bw.WriteByte(0);
            Modified = false;
        }

        public override void Load(RSFileReader br)
        {
            byte tag;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10: Name = br.ReadString(); break;
                    case 12: Text = br.ReadString(); break;
                }
            }
        }
    }
}
