using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;

namespace Rambha.Serializer
{
    public class RSFileReader
    {
        public BinaryReader br;

        public StreamWriter logStream = null;


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

        /// <summary>
        /// Comparing two byte arrays
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        public bool ReadHeader(byte[] ba)
        {
            Log("HEADER\n");
            byte[] ha = ReadBytes(ba.Length);
            if (!ByteArrayCompare(ha, ba))
            {
                Log("File Header does not equal to expected value.");
                return false;
            }

            return true;
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

        private static CultureInfo p_ci = null;
        public static CultureInfo SafeCultureInfo
        {
            get
            {
                if (p_ci == null)
                    p_ci = new CultureInfo("en");
                return p_ci;
            }
        }

        public double ReadDouble()
        {
            string str = ReadString();
            return double.Parse(str, SafeCultureInfo);
        }

        public float ReadFloat()
        {
            string str = ReadString();
            return float.Parse(str, SafeCultureInfo);
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

    }

}
