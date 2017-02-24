using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class MNReferencedAudioText: MNReferencedCore
    {
        private string p_Text = string.Empty;
        private MNReferencedSound p_Sound = null;
        private List<GOFRunningTextItem> p_words = new List<GOFRunningTextItem>();

        public List<GOFRunningTextItem> Words
        {
            get { return p_words; }
        }

        public int currentWord = -1;

        public MNReferencedSound Sound
        {
            get { return p_Sound; }
            set { p_Sound = value; Modified = true; }
        }

        public string Text
        {
            get { return p_Text; }
            set { p_Text = value; Modified = true; }
        }

        public override bool IsModified()
        {
            bool b = false;
            if (p_words != null)
            {
                foreach (GOFRunningTextItem ti in p_words)
                {
                    b = (ti.IsModified() ? true : b);
                }
            }
            return base.IsModified() || b || (Sound != null ? Sound.IsModified() : false);
        }

        public void ClearWords()
        {
            p_words.Clear();
            Modified = true;
        }

        public List<GOFRunningTextItem> GetWords()
        {
            return p_words;
        }

        public GOFRunningTextItem AddWord(GOFRunningTextItem word)
        {
            p_words.Add(word);
            Modified = true;
            return word;
        }

        public GOFRunningTextItem AddWord(string word)
        {
            GOFRunningTextItem ti = new GOFRunningTextItem();
            ti.Text = word;
            p_words.Add(ti);
            Modified = true;
            return ti;
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteByte(1);
            bw.WriteInt32(p_words.Count);
            foreach (GOFRunningTextItem goi in p_words)
            {
                goi.Save(bw);
            }

            if (Sound != null)
            {
                bw.WriteByte(2);
                Sound.Save(bw);
            }

            bw.WriteByte(3);
            bw.WriteString(Text);

            bw.WriteByte(0);
        }

        public override void Load(RSFileReader br)
        {
            byte tag = 0;
            p_words.Clear();
            Sound = null;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 1:
                        int count = br.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            GOFRunningTextItem gri = new GOFRunningTextItem();
                            gri.Load(br);
                            p_words.Add(gri);
                        }
                        break;
                    case 2:
                        Sound = new MNReferencedSound();
                        Sound.Load(br);
                        break;
                    case 3:
                        Text = br.ReadString();
                        break;
                }
            }
        }

        public int GetCurrentTimeInterval()
        {
            if (currentWord >= 0 && currentWord < p_words.Count)
            {
                return (int)(p_words[currentWord].TimeOffset - (currentWord > 0 
                    ? p_words[currentWord-1].TimeOffset : 0));
            }

            return 0;
        }

        public int GetWordCount()
        {
            return p_words.Count;
        }
    }

    public class GOFRunningTextItem: MNReferencedCore
    {
        private long p_timeoffset = 0;
        private string p_text = "";
        private bool p_valid = false;

        public long TimeOffset
        {
            get { return p_timeoffset; }
            set { p_timeoffset = value; Modified = true; }
        }
        public string Text
        {
            get { return p_text; }
            set { p_text = value; Modified = true; }
        }

        public bool Valid
        {
            get { return p_valid; }
            set { p_valid = value; Modified = true; }
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteByte(1);
            bw.WriteInt64(TimeOffset);
            bw.WriteByte(2);
            bw.WriteBool(Valid);
            bw.WriteByte(3);
            bw.WriteString(Text);
            bw.WriteByte(0);
            Modified = true;
        }

        public override void Load(RSFileReader br)
        {
            byte tag = 0;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 1: TimeOffset = br.ReadInt64(); break;
                    case 2: Valid = (br.ReadByte() == 1); break;
                    case 3: Text = br.ReadString(); break;
                    default: break;
                }
            }
        }

        public override string ToString()
        {
            if (Valid)
                return string.Format("{0}  [{1} ms]", Text, TimeOffset);
            else
                return Text;
        }
    }

}
