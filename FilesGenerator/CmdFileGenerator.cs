using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace FilesGenerator
{
    public class CmdFileGenerator
    {
        public static void Generate(string WorkDir)
        {
            foreach (string bookDir in Directory.GetDirectories(WorkDir))
            {
                GenerateCommandFileForDir(bookDir);
            }
        }

        private static void GenerateCommandFileForDir(string bookDir)
        {
            string bookName = null;
            string coverImage = null;

            foreach (string file in Directory.GetFiles(bookDir))
            {
                string ext = Path.GetExtension(file);
                string filewe = Path.GetFileNameWithoutExtension(file);
                if (ext.Equals(".pdf"))
                {
                    if (filewe.Contains("Activity Book"))
                    {
                    }
                    else if (filewe.Contains("Spots"))
                    {
                    }
                    else
                    {
                        bookName = filewe;
                    }
                }
                else if (ext.Equals(".jpg") && filewe.Contains("Cover"))
                {
                    coverImage = file;
                }
            }

            if (bookName != null && coverImage != null)
            {
                StringBuilder sb = new StringBuilder();
                /*sb.AppendLine("BookCode:");
                sb.AppendLine("BookColor:");
                sb.AppendLine("BookName:" + bookName);
                sb.AppendLine("BookIcon:" + coverImage);
                sb.AppendLine("Author:Urmila devi dasi");
                sb.AppendLine("Copyright:© 2010 Padma Inc.");
                sb.AppendLine("Publisher:© 2010 Padma Inc.");
                sb.AppendLine("Collection:Phase One");
                sb.AppendLine("ExecuteFile:" + Path.Combine(bookDir, "Cmd.txt"));
                sb.AppendLine("ExecuteFile:" + Path.Combine(bookDir, "Lang.txt"));

                File.WriteAllText(Path.Combine(bookDir, "Root.txt"), sb.ToString());
                */
                sb.Clear();
                sb.AppendLine("Language:Default");
                sb.AppendLine("AddImage:" + Path.GetFileNameWithoutExtension(coverImage) + "," + coverImage);

                string audioDir = null;

                foreach (string subDir in Directory.GetDirectories(bookDir))
                {
                    string sub = subDir;
                    bool isAudio = (Path.GetFileNameWithoutExtension(subDir) == "Audio");
                    if (isAudio)
                    {
                        audioDir = subDir;
                        if (Directory.Exists(Path.Combine(subDir, "English")))
                            sub = Path.Combine(subDir, "English");
                        else
                            continue;
                    }

                    foreach (string subFile in Directory.GetFiles(sub))
                    {
                        string ext = Path.GetExtension(subFile);
                        if (ext == ".mp3")
                            sb.AppendLine("AddSound:" + Path.GetFileNameWithoutExtension(subFile) + "," + subFile);
                        if (ext == ".png")
                            sb.AppendLine("AddImage:" + Path.GetFileNameWithoutExtension(subFile) + "," + subFile);
                    }
                }

                sb.AppendLine("EndLanguage:");


                File.WriteAllText(Path.Combine(bookDir, "Cmd.txt"), sb.ToString());

                // build all languages
                if (audioDir != null)
                {
                    sb.Clear();

                    foreach (string subDir in Directory.GetDirectories(audioDir))
                    {
                        if (Path.GetFileNameWithoutExtension(subDir) == "English")
                            continue;

                        sb.AppendLine("Language:" + Path.GetFileNameWithoutExtension(subDir));
                        foreach (string subFile in Directory.GetFiles(subDir))
                        {
                            string ext = Path.GetExtension(subFile);
                            if (ext == ".mp3")
                                sb.AppendLine("AddSound:" + Path.GetFileNameWithoutExtension(subFile) + "," + subFile);
                            if (ext == ".png")
                                sb.AppendLine("AddImage:" + Path.GetFileNameWithoutExtension(subFile) + "," + subFile);
                        }

                        sb.AppendLine("EndLanguage:");
                    }


                    File.WriteAllText(Path.Combine(bookDir, "Lang.txt"), sb.ToString());
                }
                else
                {
                    Debugger.Log(0, "", "Missing English for " + bookDir + "\n");
                }

            }
        }
    }
}
