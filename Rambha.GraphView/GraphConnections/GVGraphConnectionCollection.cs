using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    public class GVGraphConnectionCollection : List<GVGraphConnection>
    {
        public GVGraph Parent = null;
        public bool Modified = false;

        public GVGraphConnectionCollection(GVGraph p)
        {
            Parent = p;
        }

        public new void Add(GVGraphConnection obj)
        {
            if (obj.Id < 0)
                obj.Id = Parent.GetNextId();
            base.Add(obj);
            Modified = true;
        }

        public new void Clear()
        {
            base.Clear();
            Modified = true;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            Modified = true;
        }

        public List<GVGraphConnection> FindSourced(GVGraphObject oid)
        {
            List<GVGraphConnection> list = new List<GVGraphConnection>();
            foreach (GVGraphConnection gv in this)
                if (gv.Source == oid)
                    list.Add(gv);
            return list;
        }

        public List<GVGraphConnection> FindTargeted(GVGraphObject oid)
        {
            List<GVGraphConnection> list = new List<GVGraphConnection>();
            foreach (GVGraphConnection gv in this)
                if (gv.Target == oid)
                    list.Add(gv);
            return list;
        }

        public void Load(RSFileReader R)
        {
            byte tag;
            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        string s = R.ReadString();
                        GVGraphConnection gc = (GVGraphConnection)Parent.TagToObject(s);
                        if (gc != null)
                        {
                            gc.Load(R);
                            this.Add(gc);
                        }
                        else
                        {
                            throw new Exception("Unknown object type " + s + " in loading Connection collection at position " + R.Position);
                        }
                        break;
                    default:
                        throw new Exception("Unknown tag " + (int)tag + " in loading " + GetType().Name + " at position " + R.Position);
                }
            }
        }

        public void Save(RSFileWriter W)
        {
            foreach (GVGraphConnection g in this)
            {
                W.WriteByte(10);
                W.WriteString(Parent.ObjectToTag(g));
                g.Save(W);
            }

            W.WriteByte(0);
        }
    }

}
