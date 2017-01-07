using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Serializer;

namespace Rambha.GraphView
{
    /// <summary>
    /// We declare data entry using this class
    /// data entry is for example parameter of the function
    /// </summary>
    public class GVDeclarationDataEntry
    {
        public string DataType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public GVDataDirection Direction { get; set; }

        public GVDeclarationDataEntry()
        {
            DataType = "";
            Name = "";
            Value = null;
            Direction = GVDataDirection.Input;
        }

        public GVDeclarationDataEntry(string name)
        {
            DataType = "";
            Name = name;
            Value = null;
            Direction = GVDataDirection.Input;
        }

        public GVDeclarationDataEntry(string name, GVDataDirection dir)
        {
            DataType = "";
            Name = name;
            Value = null;
            Direction = dir;
        }

        public GVDeclarationDataEntry(string dataType, string name, string value)
        {
            DataType = dataType;
            Name = name;
            Value = value;
            Direction = GVDataDirection.Input;
        }

        public GVDeclarationDataEntry(string dataType, string name, string value, GVDataDirection dir)
        {
            DataType = dataType;
            Name = name;
            Value = value;
            Direction = dir;
        }


        public GVDeclarationDataEntry(string dataType, string name)
        {
            DataType = dataType;
            Name = name;
            Value = null;
            Direction = GVDataDirection.Input;
        }

        public GVDeclarationDataEntry(string dataType, string name, GVDataDirection dir)
        {
            DataType = dataType;
            Name = name;
            Value = null;
            Direction = dir;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, DataType);
        }

        public GVDeclarationDataEntry CreateCopy()
        {
            GVDeclarationDataEntry ap = new GVDeclarationDataEntry();
            ap.DataType = this.DataType;
            ap.Name = this.Name;
            ap.Value = this.Value;
            ap.Direction = this.Direction;
            return ap;
        }

        public void Save(RSFileWriter W)
        {
            if (DataType != null)
            {
                W.WriteByte(10);
                W.WriteString(DataType);
            }
            if (Name != null)
            {
                W.WriteByte(11);
                W.WriteString(Name);
            }
            if (Value != null)
            {
                W.WriteByte(12);
                W.WriteString(Value);
            }

            W.WriteByte(13);
            W.WriteInt32((Int32)Direction);

            W.WriteByte(0);
        }

        public void Load(RSFileReader R)
        {
            byte tag;
            while ((tag = R.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10: DataType = R.ReadString(); break;
                    case 11: Name = R.ReadString(); break;
                    case 12: Value = R.ReadString(); break;
                    case 13: Direction = (GVDataDirection)R.ReadInt32(); break;
                    default: break;
                }
            }
        }
    }

    public enum GVDataDirection
    {
        Input,
        Output,
        InputOutput,
        Self
    }
}
