using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    /// <summary>
    /// Graph
    /// 
    /// Graph consists of:
    /// - objects
    /// - connections
    /// 
    /// Connnection types:
    /// - control flow: handles execution to the next object or method
    ///         this can have name which is either conditional flow or name of method
    /// - ownership link: between method and object
    /// </summary>
    public class GVGraph: IRSSerializable, IRSObjectResolver, IRSUniqueIdProvider
    {
        private static long p_next_id = 1;



        public GVGraphObjectCollection Objects = null;

        public GVGraphConnectionCollection Connections = null;

        public IRSUniqueIdProvider ExternalIdProvider = null;

        static GVGraph()
        {
            p_next_id = Properties.Settings.Default.LastGraphId;
        }

        public GVGraph()
        {
            Connections = new GVGraphConnectionCollection(this);
            Objects = new GVGraphObjectCollection(this);
        }

        public long GetNextId()
        {
            if (ExternalIdProvider != null)
            {
                return ExternalIdProvider.GetNextId();
            }
            else
            {
                long i = p_next_id;
                p_next_id++;
                Properties.Settings.Default.LastGraphId = p_next_id;
                Properties.Settings.Default.Save();
                return i;
            }
        }

        private float GetMaxY()
        {
            float maxy = 0f;
            foreach (GVGraphObject go in Objects)
            {
                if (go.Y + go.Height > maxy)
                    maxy = go.Y + go.Height;
            }
            return maxy;
        }

        public GVGraphObject Add(GVGraphObject entity)
        {
            entity.Id = GetNextId();
            entity.X = 0;
            entity.Y = GetMaxY() + 120;
            Objects.Add(entity);

            return entity;
        }

        public virtual void Save(RSFileWriter W)
        {
            W.WriteByte(10);
            W.WriteInt64(p_next_id);

            if (Objects != null)
            {
                W.WriteByte(20);
                Objects.Save(W);
            }

            if (Connections != null)
            {
                W.WriteByte(30);
                Connections.Save(W);
            }

            W.WriteByte(0);
        }

        public virtual void Load(RSFileReader R)
        {
            byte tag;

            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        p_next_id = R.ReadInt64();
                        break;
                    case 20:
                        Objects = new GVGraphObjectCollection(this);
                        Objects.Load(R);
                        break;
                    case 30:
                        Connections = new GVGraphConnectionCollection(this);
                        Connections.Load(R);
                        break;
                    default:
                        break;
                }
            }

        }

        public virtual string ObjectToTag(object o)
        {
            // subclasses of GVGraphObject first
            if (o is GVGraphAction) return "GVGraphAction";
            // GVGraphObject then
            if (o is GVGraphObject) return "GVGraphObject";

            // subclasses of GVGraphConnection first
            if (o is GVGraphConnControlFlow) return "GVGraphConnectionControlFlow";
            if (o is GVGraphConnDataFlow) return "GVGraphConnectionDataFlow";
            if (o is GVGraphConnOwnership) return "GVGraphConnectionOwnership";
            if (o is GVGraphConnMember) return "GVGraphConnMember";
            // GVGraphConnection then
            if (o is GVGraphConnection) return "GVGraphConnection";
            return "";
        }

        public virtual object TagToObject(string s)
        {
            switch (s)
            {
                case "GVGraphAction": return new GVGraphAction(this);
                case "GVGraphObject": return new GVGraphObject(this);
                case "GVGraphConnection": return new GVGraphConnection(this);
                case "GVGraphConnectionControlFlow": return new GVGraphConnControlFlow(this);
                case "GVGraphConnectionDataFlow": return new GVGraphConnDataFlow(this);
                case "GVGraphConnectionOwnership": return new GVGraphConnOwnership(this);
                case "GVGraphConnMember": return new GVGraphConnMember(this);
                default: return null;
            }
        }

        public virtual object IRSResolver_FindObject(string objType, long objId)
        {
            if (objType == "GraphObject")
            {
                return Objects.FindObject(objId);
            }
            else
            {
                return null;
            }
        }
    }
}
