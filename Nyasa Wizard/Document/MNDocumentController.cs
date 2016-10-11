using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlideMaker.Document
{
    public class MNDocumentController
    {
        /// <summary>
        /// List of delegates for receiving notifications about document
        /// </summary>
        private static List<IDocumentDelegate> delegates = new List<IDocumentDelegate>();

        /// <summary>
        /// Currently edited document
        /// </summary>
        public static MNDocument CurrentDocument { get; set; }

        /// <summary>
        /// Current file name of document (full path)
        /// </summary>
        public static string CurrentFileName { get; set; }


        public static void RegisterDelegate(IDocumentDelegate dd)
        {
            if (delegates.IndexOf(dd) < 0)
                delegates.Add(dd);
        }

        public static void UnregisterDelegate(IDocumentDelegate dd)
        {
            int idx = delegates.IndexOf(dd);
            if (idx >= 0)
                delegates.RemoveAt(idx);
        }

        public static void SendDocumentHasChanged(MNDocument doc)
        {
            foreach (IDocumentDelegate deleg in delegates)
            {
                deleg.documentHasChanged(doc);
            }
        }

        public static void CreateNewDocument()
        {
            MNDocument document = new MNDocument();
            MNDocumentController.CurrentDocument = document;
            MNDocumentController.SendDocumentHasChanged(document);        }
    }
}
