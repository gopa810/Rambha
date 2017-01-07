using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

using Rambha.Serializer;

namespace Rambha.GOF
{
    public class GOFImage: GOFCoreObject
    {
        private byte[] p_data = null;

        public byte[] GetData()
        {
            return p_data;
        }

        public override void Load(RSFileReader br)
        {
            int length = br.ReadInt32();
            p_data = br.ReadBytes(length);
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteInt32(p_data.Length);
            bw.WriteBytes(p_data);
            Modified = false;
        }

        public void SetData(byte[] pd)
        {
            p_data = pd;
            p_Image = null;
            Modified = true;
        }

        private Image p_Image = null;

        public Image Image
        {
            get
            {
                if (p_Image == null)
                {
                    using (var u = new MemoryStream(p_data))
                    {
                        p_Image = Image.FromStream(u);
                    }
                }
                return p_Image;
            }
        }
    }
}
