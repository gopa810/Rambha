using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    public class GVGraphObjectCollection : List<GVGraphObject>
    {
        public GVGraph Parent = null;

        public bool Modified = false;


        public GVGraphObjectCollection(GVGraph p)
        {
            Parent = p;
        }

        public new void Add(GVGraphObject obj)
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

        public GVGraphObject FindObject(long oid)
        {
            foreach (GVGraphObject gv in this)
                if (gv.Id == oid)
                    return gv;
            return null;
        }

        public GVGraphObject FindObjectContainingClientPoint(PointF point)
        {
            return FindObjectContainingClientPoint(point.X, point.Y);
        }

        public GVGraphObject FindObjectContainingClientPoint(float X, float Y)
        {
            foreach (GVGraphObject go in this)
            {
                if (go.PaintedRect.Left <= X && go.PaintedRect.Right >= X
                    && go.PaintedRect.Top <= Y && go.PaintedRect.Bottom >= Y)
                {
                    return go.FindObjectContainingClientPoint(X, Y);
                }
            }
            return null;
        }


        public virtual void Load(RSFileReader R)
        {
            byte tag;
            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        string s = R.ReadString();
                        GVGraphObject go = (GVGraphObject)Parent.TagToObject(s);
                        if (go != null)
                        {
                            go.Load(R);
                            this.Add(go);
                        }
                        else
                        {
                            throw new Exception("Unknown object type " + s + " in loading object collection at position " + R.Position);
                        }
                        break;
                    default:
                        throw new Exception("Unknown tag " + (int)tag + " in loading " + GetType().Name + " at position " + R.Position);
                }
            }
        }

        public virtual void Save(RSFileWriter W)
        {
            for (int i = 0; i < Count; i++)
            {
                W.WriteByte(10);
                W.WriteString(Parent.ObjectToTag(this[i]));
                this[i].Save(W);
            }

            W.WriteByte(0);
        }

        public virtual void setReference(int tag, object obj)
        {
        }
    }

}
