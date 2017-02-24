using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using Rambha.Serializer;

namespace Rambha.GOF
{
    /// <summary>
    /// This class contains all data needed for customization of the book
    /// for specific language
    /// </summary>
    public class GOFile
    {
        private bool Modified { get; set; }

        /// <summary>
        /// Properties of the file
        /// </summary>
        private Dictionary<string, string> Properties = new Dictionary<string, string>();

        /// <summary>
        /// tree for storing datas
        /// </summary>
        private GOFNodes Data = new GOFNodes();

        private static byte[] p_FileHeader = new byte[] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 0, (byte)'G', (byte)'O', (byte)'F', 0 };

        public GOFile()
        {
            Modified = false;
        }

        public bool IsModified()
        {
            return Modified || Data.IsModified();
        }

        public GOFNodes GetNodes()
        {
            return Data;
        }

        public string GetProperty(string propertyName)
        {
            if (Properties.ContainsKey(propertyName))
                return Properties[propertyName];
            else
                return string.Empty;
        }

        public void SetProperty(string propertyName, string value)
        {
            if (Properties.ContainsKey(propertyName))
                Properties[propertyName] = value;
            else
                Properties.Add(propertyName, value);
            Modified = true;
        }

        public void Save(string fileName)
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileName)))
            {
                RSFileWriter fw = new RSFileWriter(bw);
                // write header
                fw.WriteBytes(p_FileHeader);

                // write general data
                foreach (KeyValuePair<string, string> ps in Properties)
                {
                    fw.WriteByte(10);
                    fw.WriteString(ps.Key);
                    fw.WriteString(ps.Value);
                }

                // write objects
                fw.WriteByte(100);
                Data.Save(fw);

                fw.WriteByte(0);
            }
        }

        public string LoadMessage = string.Empty;

        public bool Load(string fileName, bool fullRead)
        {
            using (BinaryReader fr = new BinaryReader(File.OpenRead(fileName)))
            {
                RSFileReader br = new RSFileReader(fr);
                byte[] b = br.ReadBytes(16);
                if (!ByteArrayCompare(b, p_FileHeader))
                {
                    LoadMessage = "Header of file does not meet criteria for GOF file.";
                    return false;
                }

                Properties.Clear();
                byte tag;

                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10:
                            string key = br.ReadString();
                            string value = br.ReadString();
                            Properties.Add(key, value);
                            break;
                        case 100:
                            if (!fullRead) return true;
                            Data.Clear();
                            Data.Load(br);
                            break;
                        default:
                            LoadMessage = string.Format("Unknown tag {0} at position {1} in the file.", tag, br.Position);
                            return false;
                    }
                }
            }

            LoadMessage = string.Empty;
            return true;
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


        public static MNReferencedCore CreateInstance(string s)
        {
            switch (s)
            {
                case "Nodes": return new GOFNodes();
                case "Sound": return new MNReferencedSound();
                case "Image": return new GOFImage();
                case "String": return new MNReferencedText();
                case "RunningText": return new MNReferencedAudioText();
                default: return new MNReferencedCore();
            }
        }

        public static string InstanceToTag(MNReferencedCore obj)
        {
            if (obj is GOFNodes) return "Nodes";
            if (obj is MNReferencedText) return "String";
            if (obj is GOFImage) return "Image";
            if (obj is MNReferencedAudioText) return "RunningText";
            if (obj is MNReferencedSound) return "Sound";
            return "Core";
        }

        public MNReferencedCore FindObject(string path)
        {
            Debugger.Log(0, "", "FIND OBJECT AT PATH: " + path + "\n");
            string[] p = null;
            if (path.IndexOf('/') >= 0)
            {
                p = path.Split('/');
            }
            else
            {
                p = new string[] { path };
            }

            GOFNodes data = Data;
            MNReferencedCore value = null;
            foreach (string component in p)
            {
                if (data == null)
                    return null;
                value = data.GetValue(component);
                if (value is GOFNodes)
                    data = (GOFNodes)value;
                else
                    data = null;

                if (value != null)
                {
                    Debugger.Log(0, "", "Value [" + component + "] from path '" + path + "' found\n");
                }
            }

            return value;
        }
    }

    public enum GOFObjectType
    {
        Directory,
        Image,
        String,
        RunningText,
        SoundFile
    }
}
