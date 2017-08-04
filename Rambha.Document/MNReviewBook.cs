using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace Rambha.Document
{
    public class MNReviewBook
    {
        public string BookNotes = "";

        public Dictionary<long, ReviewPage> Pages = new Dictionary<long, ReviewPage>();

        public void SaveToXml(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("doc");
            doc.AppendChild(root);

            XmlElement bn = doc.CreateElement("notes");
            root.AppendChild(bn);
            bn.InnerText = BookNotes;

            foreach (KeyValuePair<long, ReviewPage> page in Pages)
            {
                XmlElement e = doc.CreateElement("page");
                e.SetAttribute("id", page.Key.ToString());
                root.AppendChild(e);

                page.Value.Save(doc, e);
            }
        }

        public void LoadFromXml(XmlDocument doc)
        {
            foreach (XmlElement e in doc.ChildNodes)
            {
                if (e.Name.Equals("doc"))
                {
                    foreach (XmlElement ef in e.ChildNodes)
                    {
                        if (ef.Name.Equals("notes"))
                        {
                            BookNotes = ef.InnerText;
                        }
                        else if (ef.Name.Equals("page"))
                        {
                            ReviewPage rp = new ReviewPage();
                            rp.Load(ef);
                            Pages[int.Parse(ef.GetAttribute("id"))] = rp;
                        }
                    }
                }
            }
        }
    }

    public class ReviewPage
    {
        public string PageTitle = "";
        public string PageHelp = "";
        public string PageNotes = "";

        public Dictionary<long, ReviewItem> Items = new Dictionary<long, ReviewItem>();

        public void Save(XmlDocument doc, XmlElement e)
        {
            XmlElement e2 = doc.CreateElement("title");
            e.AppendChild(e2);
            e2.InnerText = PageTitle;

            e2 = doc.CreateElement("help");
            e.AppendChild(e2);
            e2.InnerText = PageHelp;

            e2 = doc.CreateElement("notes");
            e.AppendChild(e2);
            e2.InnerText = PageNotes;

            foreach (KeyValuePair<long, ReviewItem> item in Items)
            {
                XmlElement ei = doc.CreateElement("item");
                e.AppendChild(ei);
                ei.SetAttribute("id", item.Key.ToString());

                item.Value.Save(doc, ei);
            }
        }

        public void Load(XmlElement e)
        {
            if (e.HasChildNodes)
            {
                foreach (XmlElement ep in e.ChildNodes)
                {
                    if (ep.Name.Equals("title"))
                        PageTitle = ep.InnerText;
                    if (ep.Name.Equals("help"))
                        PageHelp = ep.InnerText;
                    if (ep.Name.Equals("notes"))
                        PageNotes = ep.InnerText;
                    if (ep.Name.Equals("item"))
                    {
                        ReviewItem it = new ReviewItem();
                        it.Load(ep);
                        Items[long.Parse(ep.GetAttribute("id"))] = it;
                    }
                }
            }
        }
    }

    public class ReviewItem
    {
        public string ItemText = "";
        public string ItemNotes = "";

        public void Save(XmlDocument doc, XmlElement e)
        {
            XmlElement e2 = doc.CreateElement("text");
            e.AppendChild(e2);
            e2.InnerText = ItemText;

            e2 = doc.CreateElement("notes");
            e.AppendChild(e2);
            e2.InnerText = ItemNotes;
        }

        public void Load(XmlElement e)
        {
            if (e.HasChildNodes)
            {
                foreach (XmlElement ei in e.ChildNodes)
                {
                    if (ei.Name.Equals("text"))
                        ItemText = ei.InnerText;
                    else if (ei.Name.Equals("notes"))
                        ItemNotes = ei.InnerText;
                }
            }
        }
    }
}
