using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    /// <summary>
    /// Specification of the ways, how output link can work
    /// Type = NoName can be provided only by methods of objects, for objects itself does not have meaning
    /// </summary>
    public class GVDeclarationFlowOut
    {
        public GVFlowOutNaming NamingType = GVFlowOutNaming.NoName;
        public string[] Names = null;

        public GVDeclarationFlowOut()
        {
        }

        public GVDeclarationFlowOut(GVFlowOutNaming anyText)
        {
            NamingType = anyText;
        }

        public GVDeclarationFlowOut(params string[] names)
        {
            NamingType = GVFlowOutNaming.Selection;
            Names = names;
        }

        public GVDeclarationFlowOut(GVDeclarationFlowOut baseFlow, params string[] names)
        {
            NamingType = GVFlowOutNaming.Selection;
            if (baseFlow != null && baseFlow.NamingType == GVFlowOutNaming.Selection)
            {
                Names = new string[names.Length + baseFlow.Names.Length];
                Array.Copy(baseFlow.Names, Names, baseFlow.Names.Length);
                Array.Copy(names, 0, Names, baseFlow.Names.Length, names.Length);
            }
            else
            {
                Names = names;
            }
        }

        public void Save(RSFileWriter W)
        {
            W.WriteByte(10);
            W.WriteInt32((Int32)NamingType);

            if (Names != null)
            {
                W.WriteByte(20);
                W.WriteInt32(Names.Length);
                foreach (string s in Names)
                {
                    W.WriteString(s);
                }
            }

            W.WriteByte(0);
        }

        public void Load(RSFileReader R)
        {
            byte tag = 0;
            Names = null;

            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        NamingType = (GVFlowOutNaming)R.ReadInt32();
                        break;
                    case 20:
                        int count = R.ReadInt32();
                        Names = new string[count];
                        for (int i = 0; i < count; i++)
                        {
                            Names[i] = R.ReadString();
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// enumeration of control out types
    /// </summary>
    public enum GVFlowOutNaming
    {
        NoName,
        Text,
        Selection
    }
}
