using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class MNReferencedSound: MNReferencedCore
    {
        private byte[] p_data = null;
        private string p_audioType = "";

        public string FileName { get; set; }

        public MNReferencedSound()
        {
            FileName = "";
        }

        public string AudioType
        {
            get { return p_audioType; }
            set { p_audioType = value; Modified = true; }
        }

        public override void Load(RSFileReader br)
        {
            br.Log("= = = SOUND = = =\n");
            string s;
            byte b;

            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        AudioType = br.ReadString();
                        break;
                    case 11:
                        int length = br.ReadInt32();
                        p_data = br.ReadBytes(length);
                        break;
                    case 12:
                        Name = br.ReadString();
                        break;
                    case 13:
                        FileName = br.ReadString();
                        break;
                }
            }
        }

        public override void Save(RSFileWriter bw)
        {
            bw.Log("= = = SOUND = = =\n");
            bw.WriteByte(10);
            bw.WriteString(AudioType);

            bw.WriteByte(11);
            bw.WriteInt32(p_data.Length);
            bw.WriteBytes(p_data);

            bw.WriteByte(12);
            bw.WriteString(Name);

            bw.WriteByte(13);
            bw.WriteString(FileName);

            bw.WriteByte(0);
            Modified = false;
        }

        public void InitializeWithFile(string fileName)
        {
            p_data = File.ReadAllBytes(fileName);
            p_audioType = Path.GetExtension(fileName).ToLower();
            if (Name.Length == 0)
                Name = Path.GetFileNameWithoutExtension(fileName);
            FileName = fileName;
            Modified = true;
        }

        public byte[] GetData()
        {
            return p_data;
        }

    }
}
