using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class MNExportContext
    {
        public string DirAllBooks;
        public string DirCurrentBook;
        public string FileCurrentPage;
        public MNPage CurrentPage;

        public int MaxFiles = 300;
        public int Files = 0;
        public string TemplatePage = @"e:\Dropbox\Projects\LTRWeb\page.html";


        public HashSet<string> UsedControls = new HashSet<string>();

        public StringBuilder sbControlList = new StringBuilder();
        public StringBuilder sbResizeList = new StringBuilder();

        public string GetFileNameFromImage(Image image)
        {
            return "";
        }
        public string GetFileNameFromImage(MNReferencedImage image)
        {
            if (image == null) return null;
            return string.Format("img/image{0}.png", image.Id);
        }
        public string GetFileNameFromSound(MNReferencedSound sound)
        {
            return string.Format("sound/{0}.mp3", sound.Name);
        }

        public void Clear()
        {
            sbControlList.Clear();
            sbResizeList.Clear();
        }

        public void AppendToControlList(params string[] args)
        {
            AppendToDictionaryList(sbControlList, args);
        }
        public void AppendToResizeList(params string[] args)
        {
            AppendToDictionaryList(sbResizeList, args);
        }
        public void AppendToDictionaryList(StringBuilder sb, params string[] args)
        {
            if (sb.Length > 0)
                sb.Append(",\n");
            sb.Append("  {\n");
            for (int i = 0; i < args.Length - 1; i += 2)
            {
                sb.AppendFormat("    \"{0}\": \"{1}\"{2}\n", args[i], args[i + 1],
                    (i < args.Length - 2 ? "," : ""));
            }
            sb.Append("  }");
        }
    }
}
