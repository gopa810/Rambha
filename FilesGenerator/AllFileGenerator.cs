using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;

using Rambha.Document;
using Rambha.Script;
using Rambha.Serializer;

namespace FilesGenerator
{
    public class AllFileGenerator
    {
        public static string OutputDir = "";
        public static string BookCode = "";

        public static MNDocument Doc = null;
        public static MNBookHeader Book = null;

        public static MNBookData Data = null;

        public static MNLocalisation Localisation = null;

        public static void Generate(string WorkDir)
        {
            foreach (string bookDir in Directory.GetDirectories(WorkDir))
            {
                Debugger.Log(0,"", "Starting file " + Path.GetFileName(bookDir) + "\n");
                string cmdFile = Path.Combine(bookDir, "Root.txt");
                if (File.Exists(cmdFile))
                {
                    OutputDir = bookDir;
                    ProcessFile(cmdFile);
                    if (Book != null)
                    {
                        string fn = Path.Combine(OutputDir, Book.BookCode + ".smb");
                        if (File.Exists(fn))
                            File.Delete(fn);
                        using (Stream s = File.OpenWrite(fn))
                        {
                            using (BinaryWriter bw = new BinaryWriter(s))
                            {
                                RSFileWriter fw = new RSFileWriter(bw);
                                Book.Save(fw);
                            }
                        }
                    }
                    if (Data != null)
                    {
                        string fn = Path.Combine(OutputDir, Book.BookCode + ".smd");
                        if (File.Exists(fn))
                            File.Delete(fn);
                        using (Stream s = File.OpenWrite(fn))
                        {
                            using (BinaryWriter bw = new BinaryWriter(s))
                            {
                                RSFileWriter fw = new RSFileWriter(bw);
                                Data.Save(fw);
                            }
                        }
                    }
                    Book = null;
                    Data = null;
                    Localisation = null;
                }
            }
            Debugger.Log(0, "", "Finished all files\n\n");
        }

        private static void ProcessFile(string cmdFile)
        {
            foreach (string line in File.ReadAllLines(cmdFile))
            {
                int idx = line.IndexOf(':');
                if (idx > 0)
                {
                    string cmd = line.Substring(0, idx);
                    string arg = line.Substring(idx + 1);
                    ProcessCommand(cmd, arg);
                }
            }
        }

        private static void ProcessCommand(string cmd, string arg)
        {
            string[] p;
            string fn;
            switch(cmd)
            {
                case "BookCode":
                    Doc = new MNDocument();
                    Book = new MNBookHeader();
                    Book.BookCode = arg;
                    BookCode = arg;
                    Data = new MNBookData(Doc);
                    break;
                case "BookName":
                    Book.BookTitle = arg;
                    break;
                case "BookColor":
                    string[] rgb = arg.Split(',');
                    Book.BookColor = Color.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]),
                        int.Parse(rgb[2]));
                    break;
                case "BookIcon":
                    Book.BookImage = GetSmallImage(arg);
                    break;
                case "Author":
                    Book.BookAuthor = arg;
                    break;
                case "Copyright":
                    Book.BookCopyright = arg;
                    break;
                case "Publisher":
                    Book.BookPublisher = arg;
                    break;
                case "Collection":
                    Book.BookCollection = arg;
                    break;
                case "ExecuteFile":
                    ProcessFile(arg);
                    break;
                case "Language":
                    Localisation = new MNLocalisation();
                    Localisation.SetProperty("BookCode", Book.BookCode);
                    Localisation.SetProperty("LanguageName", arg);
                    break;
                case "EndLanguage":
                    string ln = Localisation.GetProperty("LanguageName");
                    if (ln == "Default")
                    {
                        fn = BookCode + ".sme";
                    }
                    else
                        fn = string.Format("{0}_{1}.sme", BookCode, Localisation.GetProperty("LanguageName"));
                    Localisation.Save(Path.Combine(OutputDir,fn));
                    Localisation = null;
                    break;
                case "AddImage":
                    MNReferencedImage img = new MNReferencedImage();
                    p = arg.Split(',');
                    img.Name = p[0];
                    img.ImageData = Image.FromFile(p[1]);
                    Localisation.Images.Add(img);
                    break;
                case "AddSound":
                    MNReferencedSound snd = new MNReferencedSound();
                    p = arg.Split(',');
                    snd.Name = p[0];
                    snd.InitializeWithFile(p[1]);
                    Localisation.Sounds.Add(snd);
                    break;
            }
        }

        public static Image GetSmallImage(string fileName)
        {
            Image ret = null;
            string newSmallFile = null;
            using (Image img = Image.FromFile(fileName))
            {
                Size n = img.Size;
                Size newSize = GetNewSize(n);
                //Debugger.Log(0, "", fileName + " [" + n.Width + "," + n.Height + "] => " + newSize.Width + "," + newSize.Height + "\n");
                using (var bmp = new System.Drawing.Bitmap(newSize.Width, newSize.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(img, 0, 0, newSize.Width, newSize.Height);
                        newSmallFile = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + " Small.png");
                        bmp.Save(newSmallFile, System.Drawing.Imaging.ImageFormat.Png);
                        //ret = bmp;
                    }
                }
            }

            return Image.FromFile(newSmallFile);
        }

        private static Size GetNewSize(Size n)
        {
            return new Size(72,Convert.ToInt32(n.Height/(n.Width/72.0)));
        }
    }
}
