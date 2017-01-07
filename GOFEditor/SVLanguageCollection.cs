using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOFEditor
{
    public class SVLanguageCollection
    {
        private static List<SVLanguage> p_list = null;
        public static List<SVLanguage> List
        {
            get
            {
                if (p_list == null)
                {
                    p_list = new List<SVLanguage>();
                    string[] lines = Properties.Resources.Languages.Split('\r', '\n');

                    foreach (string rawLine in lines)
                    {
                        string line = rawLine.Trim();
                        if (line.Length == 0 || line.StartsWith("#"))
                            continue;
                        string[] parts = line.Split('\t');
                        if (parts.Length == 5)
                        {
                            string code = parts[3].Trim().ToUpper();
                            if (code.IndexOf('/') > 0)
                            {
                                code = code.Substring(0, code.IndexOf('/'));
                            }
                            p_list.Add(new SVLanguage()
                            {
                                Name = parts[0],
                                EnglishNames = parts[1],
                                ISOCode2 = code
                            });
                        }
                    }
                }

                return p_list;
            }
        }

        public static string GetName(string SelectedLanguageCode)
        {
            foreach (SVLanguage svn in p_list)
            {
                if (svn.ISOCode2.Equals(SelectedLanguageCode))
                    return svn.Name;
            }

            return "";
        }
    }

    public class SVLanguage
    {
        public string Name = string.Empty;
        public string EnglishNames = string.Empty;
        public string ISOCode2 = string.Empty;

        public override string ToString()
        {
            return string.Format("{0} - {1}", ISOCode2.ToUpper(), Name);
        }
    }
}
