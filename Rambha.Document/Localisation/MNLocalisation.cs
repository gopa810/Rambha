using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using Rambha.Serializer;

namespace Rambha.Document
{
    /// <summary>
    /// This class contains all data needed for customization of the book
    /// for specific language
    /// </summary>
    public class MNLocalisation
    {
        public bool Modified { get; set; }

        /// <summary>
        /// Properties of the file
        /// </summary>
        private Dictionary<string, string> Properties = new Dictionary<string, string>();

        /// <summary>
        /// tree for storing datas
        /// </summary>
        public List<MNReferencedImage> Images = new List<MNReferencedImage>();
        public List<MNReferencedSound> Sounds = new List<MNReferencedSound>();
        public List<MNReferencedText> Texts = new List<MNReferencedText>();
        public List<MNReferencedAudioText> AudioTexts = new List<MNReferencedAudioText>();
        public List<MNReferencedStyle> Styles = new List<MNReferencedStyle>();

        private static byte[] p_FileHeader = new byte[] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 0, (byte)'G', (byte)'O', (byte)'F', 0 };

        private MNFileWorkTime WorkTime = new MNFileWorkTime();



        public MNLocalisation()
        {
            Modified = false;
            WorkTime.SetTotalWorkTime(0);
        }

        public bool IsModified()
        {
            if (Modified) return true;
            foreach (MNReferencedText text in Texts)
                if (text.IsModified()) return true;
            foreach (MNReferencedSound sound in Sounds)
                if (sound.IsModified()) return true;
            foreach (MNReferencedImage image in Images)
                if (image.IsModified()) return true;
            foreach (MNReferencedAudioText at in AudioTexts)
                if (at.IsModified()) return true;
            foreach (MNReferencedStyle rs in Styles)
                if (rs.IsModified()) return true;

            return false;
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
                Save(fw);
                Modified = false;
            }
        }

        public void Save(RSFileWriter fw)
        {
            // write header
            fw.WriteHeader(p_FileHeader);

            Properties["LastTime"] = WorkTime.GetLastTime();
            Properties["TotalWork"] = WorkTime.GetTotalWorkTime().ToString();

            // write general data
            foreach (KeyValuePair<string, string> ps in Properties)
            {
                fw.WriteByte(10);
                fw.WriteString(ps.Key);
                fw.WriteString(ps.Value);
            }

            // write objects
            foreach (MNReferencedText text in Texts)
            {
                fw.WriteByte(100);
                text.Save(fw);
            }

            foreach (MNReferencedImage image in Images)
            {
                fw.WriteByte(101);
                image.Save(fw);
            }

            foreach (MNReferencedSound sound in Sounds)
            {
                fw.WriteByte(102);
                sound.Save(fw);
            }

            foreach (MNReferencedAudioText audio in AudioTexts)
            {
                fw.WriteByte(103);
                audio.Save(fw);
            }

            foreach (MNReferencedStyle rs in Styles)
            {
                fw.WriteByte(104);
                rs.Save(fw);
            }

            fw.WriteByte(0);
        }

        public string LoadMessage = string.Empty;

        public void Clear()
        {
            Properties.Clear();
            Texts.Clear();
            AudioTexts.Clear();
            Sounds.Clear();
            Images.Clear();
            Styles.Clear();
        }

        public bool Load(string fileName, bool fullRead)
        {
            bool result = false;

            Clear();

            using (BinaryReader fr = new BinaryReader(File.OpenRead(fileName)))
            {
                RSFileReader br = new RSFileReader(fr);
                result = Load(br, fullRead);
            }

            return result;
        }

        public bool Load(RSFileReader br, bool fullRead)
        {
            if (!br.ReadHeader(p_FileHeader))
                return false;

            Clear();
            byte tag;

            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        string key = br.ReadString();
                        string value = br.ReadString();
                        Properties.Add(key, value);
                        if (key.Equals("TotalWork"))
                        {
                            long la;
                            if (long.TryParse(value, out la))
                            {
                                WorkTime.SetTotalWorkTime(la);
                            }
                        }
                        break;
                    case 100:
                        if (!fullRead) return true;
                        MNReferencedText rt = new MNReferencedText();
                        rt.Load(br);
                        Texts.Add(rt);
                        break;
                    case 101:
                        if (!fullRead) return true;
                        MNReferencedImage ri = new MNReferencedImage();
                        ri.Load(br);
                        Images.Add(ri);
                        break;
                    case 102:
                        if (!fullRead) return true;
                        MNReferencedSound rs = new MNReferencedSound();
                        rs.Load(br);
                        Sounds.Add(rs);
                        break;
                    case 103:
                        if (!fullRead) return true;
                        MNReferencedAudioText ra = new MNReferencedAudioText();
                        ra.Load(br);
                        AudioTexts.Add(ra);
                        break;
                    case 104:
                        if (!fullRead) return true;
                        MNReferencedStyle rsa = new MNReferencedStyle();
                        rsa.Load(br);
                        Styles.Add(rsa);
                        break;
                    default:
                        LoadMessage = string.Format("Unknown tag {0} at position {1} in the file.", tag, br.Position);
                        return false;
                }
            }

            Modified = false;
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
                case "Sound": return new MNReferencedSound();
                case "Image": return new MNReferencedImage();
                case "String": return new MNReferencedText();
                case "AudioText": return new MNReferencedAudioText();
                case "Style": return new MNReferencedStyle();
                default: return new MNReferencedCore();
            }
        }

        public static string InstanceToTag(MNReferencedCore obj)
        {
            if (obj is MNReferencedText) return "String";
            if (obj is MNReferencedImage) return "Image";
            if (obj is MNReferencedAudioText) return "AudioText";
            if (obj is MNReferencedSound) return "Sound";
            if (obj is MNReferencedStyle) return "Style";
            return "Core";
        }

        public MNReferencedAudioText FindAudioText(string p)
        {
            foreach (MNReferencedAudioText at in AudioTexts)
                if (at.Name.Equals(p))
                    return at;
            return null;
        }

        public MNReferencedSound FindSound(string p)
        {
            foreach (MNReferencedSound at in Sounds)
            {
                //Debugger.Log(0,"", "--Looking in sound: " + at.Name + "\n");
                if (at.Name.Equals(p))
                    return at;
            }
            return null;
        }

        public MNReferencedText FindText(string p)
        {
            foreach (MNReferencedText at in Texts)
                if (at.Name.Equals(p))
                    return at;
            return null;
        }

        public MNReferencedImage FindImage(string p)
        {
            foreach (MNReferencedImage at in Images)
                if (at.Name.Equals(p))
                    return at;
            return null;
        }

        public MNReferencedStyle FindStyle(string p)
        {
            foreach (MNReferencedStyle rs in Styles)
                if (rs.Name.Equals(p))
                    return rs;
            return null;
        }

        /// <summary>
        /// Note: Styles are intentionaly omited, because only content objects are searched
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public MNReferencedCore FindObject(string path)
        {
            MNReferencedCore c;
            c = FindText(path);
            if (c != null) return c;
            c = FindImage(path);
            if (c != null) return c;
            c = FindSound(path);
            if (c != null) return c;
            c = FindAudioText(path);
            if (c != null) return c;
            return null;
        }

        public List<MNReferencedCore> GetObjects()
        {
            List<MNReferencedCore> list = new List<MNReferencedCore>();
            foreach (MNReferencedText text in Texts)
                list.Add(text);
            foreach (MNReferencedImage ri in Images)
                list.Add(ri);
            foreach (MNReferencedSound rs in Sounds)
                list.Add(rs);
            foreach (MNReferencedAudioText ra in AudioTexts)
                list.Add(ra);
            foreach (MNReferencedStyle rsa in Styles)
                list.Add(rsa);
            return list;
        }
    }
}
