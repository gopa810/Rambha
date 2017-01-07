using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;

namespace Rambha.GOF
{
    public class GOFString: GOFCoreObject
    {
        private string p_Text = String.Empty;

        public string Text
        {
            get { return p_Text; }
            set { p_Text = value; Modified = true; }
        }

        public override void Load(RSFileReader br)
        {
            Text = br.ReadString();
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteString(Text);
            Modified = false;
        }
    }
}
