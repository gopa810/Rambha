using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Rambha.Document;
using Rambha.Serializer;

namespace ConvertToHtml
{
    class Program
    {
        static void Main(string[] args)
        {
            MNExportContext etx = new MNExportContext();
            etx.DirAllBooks = @"e:\temp";
            string file = @"e:\Dropbox\ReaderBooks\STFK.smb";

            if (!File.Exists(file))
            {
                Console.WriteLine("File {0} does not exist.", file);
                return;
            }

            Console.WriteLine("File: {0}", file);
            MNDocument docx = Program.LoadDocument(file);

            docx.ExportToHtml(etx, Path.GetFileNameWithoutExtension(file));

            docx = null;

            StringBuilder Consolereport = new StringBuilder();
            Consolereport.AppendLine("Used controls:");
            foreach(string s in etx.UsedControls.Keys)
            {
                HashSet<long> hash = etx.UsedControls[s];
                Consolereport.AppendFormat("{0}:", s);
                foreach(long l in hash)
                {
                    Consolereport.AppendFormat(" {0}", l);
                }
                Consolereport.AppendLine();
            }

            Console.WriteLine(Consolereport.ToString());
            File.WriteAllText(Path.Combine(etx.DirCurrentBook, "info.txt"), Consolereport.ToString());

            Console.ReadLine();
        }

        /// <summary>
        /// Loading document
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MNDocument LoadDocument(string fileName)
        {
            MNDocument prevDocument = MNNotificationCenter.CurrentDocument;
            string prevFilename = MNNotificationCenter.CurrentFileName;

            if (LoadBookHeader(fileName))
            {
                MNNotificationCenter.CurrentDocument.Book.FindLanguageFiles(Path.GetDirectoryName(fileName));

                fileName = fileName.Replace(".smb", ".smd");
                LoadBookData(fileName);

                fileName = fileName.Replace(".smd", ".sme");
                LoadBookLang(fileName);
            }

            MNDocument newDocument = MNNotificationCenter.CurrentDocument;
            MNNotificationCenter.CurrentDocument = prevDocument;
            MNNotificationCenter.CurrentFileName = prevFilename;

            return newDocument;
        }
        private static bool LoadBookHeader(string fileName)
        {
            bool r = true;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\load_book.txt"))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
                {
                    MNDocument document = new MNDocument();
                    RSFileReader fr = new RSFileReader(br);
                    fr.logStream = sw;
                    try
                    {
                        document.Book.Load(fr);
                        document.Book.FilePath = fileName;
                        MNNotificationCenter.CurrentFileName = fileName;
                        MNNotificationCenter.CurrentDocument = document;
                    }
                    catch (Exception ex)
                    {
                        sw.Flush();
                        sw.WriteLine("Exception:");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine(ex.StackTrace);
                        sw.Flush();
                        r = false;
                    }
                }
            }

            return r;
        }

        private static void LoadBookData(string fileName)
        {
            if (MNNotificationCenter.CurrentDocument == null)
                return;

            if (!File.Exists(fileName))
                return;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\load_data.txt"))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
                {
                    MNDocument document = MNNotificationCenter.CurrentDocument;
                    RSFileReader fr = new RSFileReader(br);
                    fr.logStream = sw;
                    try
                    {
                        document.Data.Load(fr);
                    }
                    catch (Exception ex)
                    {
                        sw.Flush();
                        sw.WriteLine("Exception:");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine(ex.StackTrace);
                        sw.Flush();
                    }
                }
            }

            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (doc.Data.Pages.Count == 0)
            {
                doc.CreateNewPage();
            }
        }

        private static void LoadBookLang(string fileName)
        {
            if (MNNotificationCenter.CurrentDocument == null)
                return;

            if (!File.Exists(fileName))
                return;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\load_lang.txt"))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
                {
                    MNDocument document = MNNotificationCenter.CurrentDocument;
                    RSFileReader fr = new RSFileReader(br);
                    fr.logStream = sw;
                    try
                    {
                        document.DefaultLanguage.Load(fr, true);
                    }
                    catch (Exception ex)
                    {
                        sw.Flush();
                        sw.WriteLine("Exception:");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine(ex.StackTrace);
                        sw.Flush();
                    }
                }
            }

            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (doc.DefaultLanguage.Styles.Count == 0)
            {
                doc.InitialiseDefaultStyles();
            }

            foreach (MNReferencedImage ri in doc.DefaultLanguage.Images)
            {
                if (ri.Id < 1)
                    ri.Id = doc.Data.GetNextId();
            }


        }

    }
}
