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

        public string AudioType
        {
            get { return p_audioType; }
            set { p_audioType = value; Modified = true; }
        }

        public override void Load(RSFileReader br)
        {
            string s;
            s = br.ReadString();
            AudioType = s;
            int length = br.ReadInt32();
            p_data = br.ReadBytes(length);
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteString(AudioType);
            bw.WriteInt32(p_data.Length);
            bw.WriteBytes(p_data);
            Modified = false;
        }

        public void InitializeWithFile(string fileName)
        {
            p_data = File.ReadAllBytes(fileName);
            p_audioType = Path.GetExtension(fileName).ToLower();
            Modified = true;
        }

        public byte[] GetData()
        {
            return p_data;
        }

    }
}
