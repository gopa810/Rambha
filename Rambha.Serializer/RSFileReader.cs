using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Rambha.Serializer
{
    public class RSFileReader
    {
        public BinaryReader br;

        public StreamWriter logStream = null;

        public List<RSObjectReference> References = new List<RSObjectReference>();


        public RSFileReader(BinaryReader r)
        {
            br = r;
        }


        public void Log(string format, params object[] p)
        {
            if (logStream != null)
            {
                logStream.Write(format, p);
            }
        }

        public int PeekChar()
        {
            return br.PeekChar();
        }

        public long Position
        {
            get
            {
                return br.BaseStream.Position;
            }
            set
            {
                br.BaseStream.Position = value;
            }
        }

        public bool ReadBool()
        {
            bool b = (br.ReadByte() == 1);
            Log("BOOL {0}\n", b);
            return b;
        }

        public byte ReadByte()
        {
            byte b = br.ReadByte();
            Log("BYTE {0}\n", b);
            return b;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] b = br.ReadBytes(count);
            Log("BYTES LENGTH {0}\n", b.Length);
            return b;
        }

        public int ReadInt32()
        {
            int b = br.ReadInt32();
            Log("INT {0}\n", b);
            return b;
        }

        public long ReadInt64()
        {
            long b = br.ReadInt64();
            Log("LONG {0}\n", b);
            return b;
        }

        public Image ReadImage()
        {
            int a = this.ReadInt32();
            byte[] b = this.ReadBytes(a);
            Image ImageData = null;
            using (MemoryStream ms = new MemoryStream(b))
            {
                ImageData = Image.FromStream(ms);
            }
            return ImageData;
        }

        public double ReadDouble()
        {
            return double.Parse(ReadString());
        }

        public float ReadFloat()
        {
            return float.Parse(ReadString());
        }

        public Font ReadFont()
        {
            Log("FONT\n");
            string name = ReadString();
            int size = br.ReadInt32();
            FontStyle fs = (FontStyle)br.ReadInt32();
            return new Font(new FontFamily(name), size / 10f, fs);
        }

        public Color ReadColor()
        {
            byte a, r, g, b;
            a = br.ReadByte();
            r = br.ReadByte();
            g = br.ReadByte();
            b = br.ReadByte();
            Log("COLOR {0},{1},{2},{3}\n", a, r, g, b);
            return Color.FromArgb(a, r, g, b);
        }

        public string ReadString()
        {
            int l = br.ReadInt32();
            byte[] b = br.ReadBytes(l);
            var utf8 = Encoding.UTF8;
            string s = utf8.GetString(b);
            Log("STRING {0}\n", s);
            return s;
        }


        public void Offset(long a)
        {
            br.BaseStream.Position += a;
        }

        public void AddReference(IRSObjectResolver parentObject, string referencedObjectType, long referencedObjectId, int tagInSource, IRSObjectOrigin sourceObject)
        {
            RSObjectReference reference = new RSObjectReference();
            reference.Resolver = parentObject;
            reference.Tag = tagInSource;
            reference.RefObjectId = referencedObjectId;
            reference.RefObjectType = referencedObjectType;
            reference.ObjectOrigin = sourceObject;
            reference.Evaluated = false;

            References.Add(reference);
        }

        public void ResolveReferences(IRSObjectResolver resolver)
        {
            object found = null;
            foreach (RSObjectReference objectRef in References)
            {
                if (objectRef.Evaluated) continue;
                found = resolver.IRSResolver_FindObject(objectRef.RefObjectType, objectRef.RefObjectId);
                if (found != null)
                {
                    objectRef.ObjectOrigin.setReference(objectRef.Tag, found);
                    objectRef.Evaluated = true;
                }
            }
        }
    }

    public class RSObjectReference
    {
        public IRSObjectResolver Resolver = null;
        public string RefObjectType = null;
        public long RefObjectId = -1;
        public int Tag = 0;
        public IRSObjectOrigin ObjectOrigin = null;
        public bool Evaluated = false;
    }


}
