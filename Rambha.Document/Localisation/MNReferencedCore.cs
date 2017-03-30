using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;
using Rambha.Script;

namespace Rambha.Document
{
    public class MNReferencedCore: GSCore
    {
        public string Name { get; set; }

        [Browsable(false)]
        public bool Modified { get; set; }

        public MNReferencedCore()
        {
            Name = "";
            Modified = false;
        }

        public override string ToString()
        {
            return Name ?? "<untitled>";
        }

        public virtual void Save(RSFileWriter bw)
        {
        }

        public virtual void Load(RSFileReader br)
        {
        }

        public virtual bool IsModified()
        {
            return Modified;
        }
    }
}
