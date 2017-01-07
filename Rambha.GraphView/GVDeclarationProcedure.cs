using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    /// <summary>
    /// Using this class we can declare procedure
    /// </summary>
    public class GVDeclarationProcedure
    {
        public string Name = string.Empty;
        public List<GVDeclarationDataEntry> Parameters = null;
        public GVDeclarationFlowOut OutNaming { get; set; }

        private string p_listName = null;
        public int ParametersCount
        {
            get
            {
                return Parameters != null ? Parameters.Count : 0;
            }
        }

        public GVDeclarationProcedure()
        {
            OutNaming = null;
        }

        public GVDeclarationProcedure(string name)
        {
            Name = name;
        }

        public GVDeclarationProcedure(string name, params GVDeclarationDataEntry[] parameters)
        {
            Name = name;
            Parameters = new List<GVDeclarationDataEntry>();
            Parameters.AddRange(parameters);
        }

        public override string ToString()
        {
            if (p_listName == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Name);
                if (Parameters != null)
                {
                    sb.Append(" (");
                    bool comma = false;
                    foreach (GVDeclarationDataEntry ap in Parameters)
                    {
                        if (comma) sb.Append(", ");
                        sb.Append(ap.Name);
                        comma = true;
                    }
                    sb.Append(")");
                }
            }
            return Name;
        }

        public void Load(RSFileReader R)
        {
            byte tag;
            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        Name = R.ReadString();
                        break;
                    case 20:
                        if (Parameters == null) Parameters = new List<GVDeclarationDataEntry>();
                        GVDeclarationDataEntry de = new GVDeclarationDataEntry();
                        de.Load(R);
                        break;
                    case 30:
                        OutNaming = new GVDeclarationFlowOut();
                        OutNaming.Load(R);
                        break;
                }
            }
        }

        public void Save(RSFileWriter W)
        {
            W.WriteByte(10);
            W.WriteString(Name);

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (GVDeclarationDataEntry de in Parameters)
                {
                    W.WriteByte(20);
                    de.Save(W);
                }
            }

            if (OutNaming != null)
            {
                W.WriteByte(30);
                OutNaming.Save(W);
            }

            W.WriteByte(0);
        }
        public GVDeclarationDataEntry GetParameter(int a)
        {
            return (Parameters == null || Parameters.Count == 0)? null : Parameters[Math.Max(a, Parameters.Count - 1)];
        }

        public GVDeclarationProcedure CreateCopy()
        {
            GVDeclarationProcedure A = new GVDeclarationProcedure();
            A.Name = this.Name;
            if (Parameters != null)
            {
                A.Parameters = new List<GVDeclarationDataEntry>();
                foreach (GVDeclarationDataEntry ap in Parameters)
                {
                    A.Parameters.Add(ap.CreateCopy());
                }
            }
            return A;
        }

        public virtual GVDeclarationDataEntry[] getDataProperties()
        {
            if (Parameters == null || Parameters.Count == 0)
                return null;

            return Parameters.ToArray<GVDeclarationDataEntry>();
        }
    }

}
