using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FilesGenerator
{
    public class LangFileChecker
    {
        public static void Check(string WorkDir)
        {
            HashSet<string> langs = new HashSet<string>();

            foreach (string langDorA in Directory.GetFiles(@"e:\Dropbox\Books for Software\00 General\Language"))
            {
                langs.Add(Path.GetFileNameWithoutExtension(langDorA));
            }

            foreach (string bookDir in Directory.GetDirectories(WorkDir))
            {
                foreach(string subDir in Directory.GetDirectories(bookDir))
                {
                    if (Path.GetFileName(subDir) == "Audio")
                    {
                        List<string> dirs = new List<string>(Directory.GetDirectories(subDir));
                        foreach (string langDir in dirs)
                        {
                            string LangName = Path.GetFileName(langDir);
                            
                            if (!langs.Contains(LangName))
                            {
                                //Debugger.Log(0, "", "rename \"" + langDir + "\" \"" + langDir + "\"\n");
                                Debugger.Log(0, "", string.Format("Unknown language: {0}\n", langDir));
                            }
                        }
                    }
                }
            }
        }
    }
}
