using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Rambha.Document
{
    public class MNNotificationCenter
    {
        /// <summary>
        /// Currently edited document
        /// </summary>
        private static MNDocument p_document = null;
        public static MNDocument CurrentDocument 
        {
            get
            {
                return p_document;
            }
            set
            {
                p_document = value;
                BroadcastMessage(null, "DocumentChanged", value);
            }
        }

        public static MNPage CurrentPage { get; set; }

        /// <summary>
        /// Current file name of document (full path)
        /// </summary>
        public static string CurrentFileName { get; set; }

        public static List<KeyValuePair<string, INotificationTarget>> Receivers = new List<KeyValuePair<string, INotificationTarget>>();

        public static void AddReceiver(INotificationTarget target, string message)
        {
            if (target == null) return;
            Receivers.Add(new KeyValuePair<string, INotificationTarget>(message,target));
            if (EqualsMessage(message, "DocumentChanged") && CurrentDocument != null)
            {
                target.OnNotificationReceived(null, "DocumentChanged", CurrentDocument);
            }
        }

        public static bool EqualsMessage(string receiverMessage, string testedMessage)
        {
            return (receiverMessage == null || receiverMessage.Equals(testedMessage));
        }

        public static void RemoveReceiver(INotificationTarget target, string message)
        {
            int i = 0;
            while (i < Receivers.Count)
            {
                if (message == null || Receivers[i].Key == message)
                {
                    if (target == null || Receivers[i].Value == target)
                    {
                        Receivers.RemoveAt(i);
                        i--;
                    }
                }
                i++;
            }
        }

        public static void BroadcastMessage(object sender, string msg, params object[] args)
        {
            foreach (KeyValuePair<string, INotificationTarget> item in Receivers)
            {
                if (item.Key == null || item.Key == msg)
                {
                    item.Value.OnNotificationReceived(sender, msg, args);
                }
            }
        }

        public static void CreateNewDocument()
        {
            CurrentDocument = new MNDocument();
            CurrentPage = null;
        }


        public static bool AudioOn { get; set; }
    }

    public interface INotificationTarget
    {
        void OnNotificationReceived(object sender, string message, params object[] args);
    }
}
