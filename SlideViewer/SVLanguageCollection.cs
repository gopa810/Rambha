using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlideViewer
{
    public class SVLanguageCollection
    {
        private static List<SMLanguage> p_list = null;
        public static List<SMLanguage> List
        {
            get
            {
                if (p_list == null)
                {
                    p_list = new List<SMLanguage>();
                    string[] lines = Properties.Resources.Languages.Split('\r', '\n');

                    foreach (string rawLine in lines)
                    {
                        string line = rawLine.Trim();
                        if (line.Length == 0 || line.StartsWith("#"))
                            continue;
                        string[] parts = line.Split('\t');
                        if (parts.Length == 5)
                        {
                            p_list.Add(new SMLanguage()
                            {
                                Name = parts[0],
                                EnglishNames = parts[1],
                                ISOCode1 = parts[4].Trim(),
                                ISOCode2 = parts[3].Trim()
                            });
                        }
                    }
                }

                return p_list;
            }
        }
    }

    public class SMLanguage
    {
        public string Name = string.Empty;
        public string EnglishNames = string.Empty;
        public string ISOCode1 = string.Empty;
        public string ISOCode2 = string.Empty;

        public override string ToString()
        {
            if (ISOCode1.Length > 0)
                return string.Format("{0} - {1} ({2})", ISOCode1.ToUpper(), Name, ISOCode2.ToUpper());
            else
                return string.Format("{0} - {1}", ISOCode2.ToUpper(), Name);
        }
    }
}
