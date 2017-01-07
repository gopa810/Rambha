using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;

namespace Rambha.GOF
{
    public class GOFCoreObject
    {
        public bool Modified { get; set; }

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
