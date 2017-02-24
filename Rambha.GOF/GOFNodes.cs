using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;

namespace Rambha.GOF
{
    /// <summary>
    /// This class represents subnodes of one node withni the tree of objects
    /// </summary>
    public class GOFNodes: MNReferencedCore
    {

        private Dictionary<string, MNReferencedCore> Nodes = new Dictionary<string, MNReferencedCore>();

        public GOFNodes()
        {
            Modified = false;
        }

        public List<KeyValuePair<string, MNReferencedCore>> GetEnumerableNodes()
        {
            List<KeyValuePair<string,MNReferencedCore>> list = new List<KeyValuePair<string,MNReferencedCore>>();
            foreach(KeyValuePair<string,MNReferencedCore> v in Nodes)
            {
                list.Add(v);
            }
            return list;
        }

        public override bool IsModified()
        {
            if (Modified) return Modified;
            foreach (MNReferencedCore obj in Nodes.Values)
            {
                if (obj.Modified) return true;
            }
            return false;
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        public override void Load(RSFileReader br)
        {
            int count = br.ReadInt32();
            string key = "";
            string objType = "";

            Nodes.Clear();
            for (int i = 0; i < count; i++)
            {
                MNReferencedCore obj = null;
                key = br.ReadString();
                objType = br.ReadString();
                obj = GOFile.CreateInstance(objType);
                obj.Load(br);
                Nodes.Add(key, obj);
            }
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteInt32(Nodes.Count);
            foreach (KeyValuePair<string, MNReferencedCore> obj in Nodes)
            {
                bw.WriteString(obj.Key);
                bw.WriteString(GOFile.InstanceToTag(obj.Value));
                obj.Value.Save(bw);
                obj.Value.Modified = false;
            }
            Modified = false;
        }

        public void SetValue(string key, MNReferencedCore value)
        {
            Modified = true;
            if (Nodes.ContainsKey(key))
                Nodes[key] = value;
            else
                Nodes.Add(key, value);
        }

        public MNReferencedCore GetValue(string name)
        {
            return Nodes.ContainsKey(name) ? Nodes[name] : null;
        }

    }
}
