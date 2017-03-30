using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Document;

namespace FileListGen
{
    class Program
    {
        static void Main(string[] args)
        {
            SVBookLibrary Library = new SVBookLibrary();

            String directory = @"e:\Dropbox\ReaderBooks";
            Library.GetCurrentBookDatabase(directory);

            string str = SVBookLibrary.DBToString(Library.GetLocalFileDatabase());
            File.WriteAllText(Path.Combine(directory, "root.txt"), str);

        }
    }
}
