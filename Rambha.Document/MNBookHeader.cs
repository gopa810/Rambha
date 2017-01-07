using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Rambha.Serializer;


namespace Rambha.Document
{
    public class MNBookHeader
    {
        public string BookTitle = "";
        public string BookCode = "";
        public string BookAuthor = "";
        public string BookCollection = "";
        public string BookCopyright = "";
        public Image BookImage = null;
        public string BookPublisher = "";

        public string FilePath = "";
        public List<MNBookLanguage> Languages = new List<MNBookLanguage>();

        public override string ToString()
        {
            return BookTitle + " [" + BookCode + "]";
        }


        public bool LoadHeader(string file)
        {
            bool value = false;
            FilePath = file;
            using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
            {
                RSFileReader reader = new RSFileReader(br);
                MNDocument doc = new MNDocument();
                value = doc.Load(reader, false);
                if (value)
                {
                    BookTitle = doc.BookTitle;
                    BookCode = doc.BookCode;
                    BookAuthor = doc.BookAuthor;
                    BookCollection = doc.BookCollection;
                    BookCopyright = doc.BookCopyright;
                    BookImage = doc.BookImage;
                    BookPublisher = doc.BookPublisher;
                }
                doc = null;
            }

            return value;
        }

        public MNDocument LoadFull()
        {
            string file = FilePath;
            MNDocument doc = new MNDocument();
            using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
            {
                RSFileReader reader = new RSFileReader(br);
                if (!doc.Load(reader, true))
                {
                    doc = null;
                }
            }

            return doc;
        }
    }
}
