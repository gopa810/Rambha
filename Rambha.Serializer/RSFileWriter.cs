using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Rambha.Serializer
{
    public class RSFileWriter
    {
        BinaryWriter bw;

        public StreamWriter logStream = null;

        public RSFileWriter(BinaryWriter w)
        {
            bw = w;
        }

        public void Log(string format, params object[] p)
        {
            if (logStream != null)
            {
                logStream.Write(format, p);
            }
        }

        public void WriteHeader(byte[] ba)
        {
            Log("HEADER\n");
            WriteBytes(ba);
        }

        public void WriteBool(bool b)
        {
            Log("BOOL {0}\n", b);
            bw.Write((byte)(b ? 1 : 0));
        }

        public void WriteDouble(double d)
        {
            WriteString(d.ToString());
        }

        public void WriteFloat(float f)
        {
            WriteString(f.ToString());
        }

        public void WriteFont(Font font)
        {
            Log("FONT\n");
            WriteString(font.FontFamily.Name);
            bw.Write((Int32)(font.Size * 10));
            bw.Write((Int32)font.Style);
        }

        public void WriteColor(Color clr)
        {
            Log("COLOR {0},{1},{2},{3}\n", clr.A, clr.R, clr.G, clr.B);
            bw.Write(clr.A);
            bw.Write(clr.R);
            bw.Write(clr.G);
            bw.Write(clr.B);
        }

        public void WriteImage(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] bimg = ms.ToArray();
                this.WriteInt32(bimg.Length);
                this.WriteBytes(bimg);
            }
        }

        public void WriteTagString(byte tag, string s)
        {
            var utf8 = Encoding.UTF8;
            byte[] bs = utf8.GetBytes(s ?? "");
            bw.Write(tag);
            bw.Write((Int32)bs.Length);
            bw.Write(bs);
            Log("BYTE {0}\n", tag);
            Log("STRING {0}\n", s);
        }

        public void WriteString(string s)
        {
            var utf8 = Encoding.UTF8;
            byte[] bs = utf8.GetBytes(s ?? "");
            bw.Write((Int32)bs.Length);
            bw.Write(bs);
            Log("STRING {0}\n", s);
        }

        public void WriteByte(byte a)
        {
            Log("BYTE {0}\n", a);
            bw.Write(a);
        }

        public void WriteBytes(byte[] ar)
        {
            Log("BYTES LENGTH {0}\n", ar.Length);
            bw.Write(ar);
        }

        public void WriteInt32(int a)
        {
            Log("INT {0}\n", a);
            bw.Write(a);
        }

        public void WriteInt64(long a)
        {
            Log("LONG {0}\n", a);
            bw.Write(a);
        }
    }
}
