using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace SlideViewer
{
    public class ErrorCatcher
    {
        public static string currentOperation = "";

        public static StringBuilder sb = new StringBuilder();


        public static void Add(string format)
        {
            sb.Append(format);
        }
        public static void Add(string format, params object[] args)
        {
            sb.AppendFormat(format, args);
        }

        public static void Save()
        {
            File.WriteAllText(Path.Combine(AssemblyDirectory, "errors.txt"), sb.ToString());
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
