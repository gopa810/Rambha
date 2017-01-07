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
    public class GOFNodes: GOFCoreObject
    {

        private Dictionary<string, GOFCoreObject> Nodes = new Dictionary<string, GOFCoreObject>();

        public GOFNodes()
        {
            Modified = false;
        }

        public List<KeyValuePair<string, GOFCoreObject>> GetEnumerableNodes()
        {
            List<KeyValuePair<string,GOFCoreObject>> list = new List<KeyValuePair<string,GOFCoreObject>>();
            foreach(KeyValuePair<string,GOFCoreObject> v in Nodes)
            {
                list.Add(v);
            }
            return list;
        }

        public override bool IsModified()
        {
            if (Modified) return Modified;
            foreach (GOFCoreObject obj in Nodes.Values)
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
                GOFCoreObject obj = null;
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
            foreach (KeyValuePair<string, GOFCoreObject> obj in Nodes)
            {
                bw.WriteString(obj.Key);
                bw.WriteString(GOFile.InstanceToTag(obj.Value));
                obj.Value.Save(bw);
                obj.Value.Modified = false;
            }
            Modified = false;
        }

        public void SetValue(string key, GOFCoreObject value)
        {
            if (Nodes.ContainsKey(key))
                Nodes[key] = value;
            else
                Nodes.Add(key, value);
        }

        public GOFCoreObject GetValue(string name)
        {
            return Nodes.ContainsKey(name) ? Nodes[name] : null;
        }

    }
}
