using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class MNBookData
    {
        private static byte[] p_fileHeader = new byte[] {
            1, 1, 2, 3, 
            5, 8, 13, 21,
            34, 55, 89, 144,
            0, 8, 12, 0
        };

        private static byte[] p_fileStatusHeader = new byte[] {
            1, 1, 2, 3, 
            5, 8, 13, 21,
            34, 55, 89, 144,
            0, 8, 14, 0
        };

        private MNDocument p_document = null;

        public List<MNPage> Pages = new List<MNPage>();
        public List<MNPage> Templates = new List<MNPage>();
        public List<MNReferencedText> Scripts = new List<MNReferencedText>();
        public List<MNMenu> Menus = new List<MNMenu>();

        public MNBookData(MNDocument doc)
        {
            p_document = doc;
        }

        public void Clear()
        {
            Pages.Clear();
            Templates.Clear();
            Scripts.Clear();
            Menus.Clear();
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteHeader(p_fileHeader);

            bw.WriteByte(6);
            bw.WriteInt32(p_id);


            foreach (MNPage page in Pages)
            {
                bw.WriteByte(10);
                page.Save(bw);
            }

            foreach (MNPage tmp in Templates)
            {
                bw.WriteByte(11);
                tmp.Save(bw);
            }

            foreach (MNReferencedText rt in Scripts)
            {
                bw.WriteByte(13);
                rt.Save(bw);
            }

            foreach (MNMenu menu in Menus)
            {
                bw.WriteByte(14);
                menu.Save(bw);
            }

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            if (!br.ReadHeader(p_fileHeader))
                return;

            Clear();

            int pageIndex = 0;
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 6:
                        p_id = br.ReadInt32();
                        break;
                    case 10:
                        MNPage p = new MNPage(p_document);
                        p.Index = pageIndex;
                        p.Load(br);
                        Pages.Add(p);
                        pageIndex++;
                        break;
                    case 11:
                        MNPage pt = new MNPage(p_document);
                        pt.Load(br);
                        Templates.Add(pt);
                        break;
                    case 12:
                        MNReferencedText tx = new MNReferencedText();
                        tx.Load(br);
                        Scripts.Add(tx);
                        break;
                    case 13:
                        MNMenu mn = new MNMenu();
                        mn.Load(p_document, br);
                        Menus.Add(mn);
                        break;
                }
            }
        }

        public void SaveStatus(RSFileWriter bw)
        {
            bw.WriteHeader(p_fileStatusHeader);

            foreach (MNPage page in Pages)
            {
                bw.WriteByte(10);
                bw.WriteInt64(page.Id);
                page.SaveStatus(bw);
            }

            bw.WriteByte(0);
        }

        public void LoadStatus(RSFileReader br)
        {
            if (!br.ReadHeader(p_fileStatusHeader))
                return;

            byte b;
            long mid;
            MNPage page;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        mid = br.ReadInt64();
                        page = FindPage(mid);
                        if (page != null)
                            page.LoadStatus(br);
                        break;
                }
            }
        }

        public MNPage FindPage(long mid)
        {
            foreach (MNPage p in Pages)
                if (p.Id == mid)
                    return p;
            return null;
        }

        public static int p_id = -1;


        public void InitializeID()
        {
            if (p_id < 0)
                p_id = Properties.Settings.Default.UniqueID;
        }

        public void SaveID()
        {
            Properties.Settings.Default.UniqueID = p_id;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Generating unique ID
        /// </summary>
        /// <returns></returns>
        public long GetNextId()
        {
            return p_id++;
        }
    }
}
