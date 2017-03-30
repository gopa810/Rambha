using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

using Rambha.Serializer;
using Rambha.Document.Views;


namespace Rambha.Document
{
    public class MNBookHeader
    {
        private static byte[] p_fileHeader = new byte[] {
            1, 1, 2, 3, 
            5, 8, 13, 21,
            34, 55, 89, 144,
            0, 7, 7, 0
        };

        [Browsable(true), DisplayName("Book Title"), Category("Document")]
        public string BookTitle { get; set; }

        [Browsable(true), DisplayName("Book Code"), Category("Document")]
        public string BookCode { get; set; }

        [Browsable(true), DisplayName("Book Author"), Category("Document")]
        public string BookAuthor { get; set; }

        [Browsable(true), DisplayName("Publisher"), Category("Document")]
        public string BookPublisher { get; set; }

        [Browsable(true), DisplayName("Copyright"), Category("Document")]
        public string BookCopyright { get; set; }

        [Browsable(true), DisplayName("Collection"), Category("Document")]
        public string BookCollection { get; set; }

        [Browsable(true), DisplayName("Book Color"), Category("Document")]
        public Color BookColor { get; set; }

        [Browsable(true), DisplayName("Ordering Priority"), Category("Presentation")]
        public int BookPriority { get; set; }

        [Browsable(false), Category("Presentation")]
        public int DefaultFontSize { get; set; }

        [Editor(typeof(ImageSelectionPropertyEditor), typeof(UITypeEditor))]
        [Browsable(true), DisplayName("Book Icon"), Category("Document")]
        public MNReferencedImage BookImageRef { get; set; }

        [Browsable(true), Category("Evaluation")]
        public MNEvaluationType Evaluation { get; set; }

        [Browsable(true), Category("Document")]
        public String StartPage { get; set; }

        [Browsable(true), Category("Document")]
        public String HomePage { get; set; }

        [Browsable(true), ReadOnly(true), Category("Saving")]
        public string LastTimeSave { get; set; }

        [Browsable(true), ReadOnly(true), Category("Saving")]
        public long TotalWorkSeconds
        {
            get { return WorkTime.PeekSeconds(); }
        }

        private MNFileWorkTime WorkTime = new MNFileWorkTime();

        public int Version { get; set; }

        [Browsable(false)]
        public Image BookImage 
        {
            get
            {
                return BookImageRef != null ? BookImageRef.ImageData : null;
            }
            set
            {
                if (BookImageRef == null) BookImageRef = new MNReferencedImage();
                BookImageRef.ImageData = value;
            }
        }

        private int sizeOfInt = sizeof(int);
        public string FilePath = "";
        public List<MNBookLanguage> Languages = new List<MNBookLanguage>();

        public MNBookHeader()
        {
            BookTitle = "";
            BookCode = "";
            BookAuthor = "";
            BookCollection = "";
            BookCopyright = "";
            BookImageRef = null;
            BookPublisher = "";
            BookColor = Color.White;
            BookPriority = 0;
            HomePage = "start";
            Version = 2;
            DefaultFontSize = 32;

            WorkTime.SetTotalWorkTime(0);
        }

        public override string ToString()
        {
            return BookTitle + " [" + BookCode + "]";
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteHeader(p_fileHeader);

            bw.WriteByte(5);
            bw.WriteByte(sizeof(int));

            bw.WriteTagString(10, BookTitle);
            bw.WriteTagString(11, BookCode);
            bw.WriteTagString(12, BookAuthor);
            bw.WriteTagString(13, BookPublisher);
            bw.WriteTagString(14, BookCopyright);
            bw.WriteTagString(15, BookCollection);
            
            bw.WriteByte(16);
            bw.WriteColor(BookColor);

            bw.WriteTagString(17, StartPage);

            if (BookImageRef != null)
            {
                bw.WriteByte(18);
                BookImageRef.Save(bw);
            }

            bw.WriteByte(19);
            bw.WriteInt32((Int32)Evaluation);

            bw.WriteByte(20);
            bw.WriteString(WorkTime.GetLastTime());

            bw.WriteByte(21);
            bw.WriteInt64(WorkTime.GetTotalWorkTime());

            bw.WriteByte(22);
            bw.WriteInt32(BookPriority);

            bw.WriteByte(23);
            bw.WriteString(HomePage);

            bw.WriteByte(24);
            bw.WriteInt32(Version);

            bw.WriteByte(25);
            bw.WriteInt32(DefaultFontSize);

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            if (!br.ReadHeader(p_fileHeader))
                throw new Exception("Invalid header for book file");

            Version = 1;
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 5:
                        sizeOfInt = br.ReadByte();
                        break;
                    case 10:
                        BookTitle = br.ReadString();
                        break;
                    case 11:
                        BookCode = br.ReadString();
                        break;
                    case 12:
                        BookAuthor = br.ReadString();
                        break;
                    case 13:
                        BookPublisher = br.ReadString();
                        break;
                    case 14:
                        BookCopyright = br.ReadString();
                        break;
                    case 15:
                        BookCollection = br.ReadString();
                        break;
                    case 16:
                        BookColor = br.ReadColor();
                        break;
                    case 17:
                        StartPage = br.ReadString();
                        break;
                    case 18:
                        BookImageRef = new MNReferencedImage();
                        BookImageRef.Load(br);
                        break;
                    case 19:
                        Evaluation = (MNEvaluationType)br.ReadInt32(); 
                        break;
                    case 20:
                        LastTimeSave = br.ReadString();
                        break;
                    case 21:
                        WorkTime.SetTotalWorkTime(br.ReadInt64());
                        break;
                    case 22:
                        BookPriority = br.ReadInt32();
                        break;
                    case 23:
                        HomePage = br.ReadString();
                        break;
                    case 24:
                        Version = br.ReadInt32();
                        break;
                    case 25:
                        DefaultFontSize = br.ReadInt32();
                        break;
                }
            }
        }

        public bool LoadHeader(string file)
        {
            bool value = true;
            FilePath = file;
            using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
            {
                RSFileReader reader = new RSFileReader(br);
                try
                {
                    Load(reader);
                }
                catch
                {
                    value = false;
                }
            }

            return value;
        }

        public MNDocument LoadFull()
        {
            string file = FilePath;
            MNDocument doc = new MNDocument();
            doc.Book = this;

            file = file.Replace(".smb", ".smd");
            using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
            {
                RSFileReader reader = new RSFileReader(br);
                try
                {
                    doc.Data.Load(reader);
                }
                catch
                {
                }
            }

            file = file.Replace(".smd", ".sme");
            if (File.Exists(file))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
                {
                    RSFileReader reader = new RSFileReader(br);
                    try
                    {
                        doc.DefaultLanguage.Load(reader, true);
                    }
                    catch
                    {
                    }
                }
            }

            file = file.Replace(".sme", ".sms");
            if (File.Exists(file))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
                {
                    RSFileReader reader = new RSFileReader(br);
                    try
                    {
                        doc.Data.LoadStatus(reader);
                    }
                    catch
                    {
                    }
                }
            }

            return doc;
        }



        public void FindLanguageFiles(string directory)
        {
            if (directory.Length == 0)
                return;

            List<string> langFileNames = new List<string>();

            foreach (string s in Directory.EnumerateFiles(directory))
            {
                if (s.EndsWith(".sme"))
                {
                    MNBookLanguage bl = new MNBookLanguage();
                    PreviewLanguage(bl, s);
                    if (bl.BookCode == this.BookCode)
                        Languages.Add(bl);
                }
            }
        }

        public void PreviewLanguage(MNBookLanguage bl, string fileName)
        {
            MNLocalisation file = new MNLocalisation();
            bl.FilePath = fileName;
            file.Load(bl.FilePath, false);
            bl.BookCode = file.GetProperty("BookCode");
            bl.LanguageName = file.GetProperty("LanguageName");
            bl.LastTimeSave = file.GetProperty("LastChange");
        }

    }
}
