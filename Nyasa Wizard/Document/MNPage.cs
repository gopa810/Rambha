using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

using SlideMaker.DocumentViews;
using System.Drawing.Design;

using SlideMaker.Document;

namespace SlideMaker
{
    [Serializable()]
    public class MNPage
    {
        [Browsable(false)]
        public MNDocument Document { get; set; }

        [Browsable(true),DisplayName("Page Title"),Category("Page")]
        public string Title { get; set; }

        [Browsable(false)]
        public int PageIndex { get; set; }

        [Browsable(false)] 
        public Size DefaultTextImageSize { get; set; }

        [NonSerialized(),Browsable(false)]
        public TreeNode TreeNode = null;

        public List<MNPageRuler> Rulers = new List<MNPageRuler>();
        /// <summary>
        /// local value for this page
        /// </summary>
        private int def_font_size = -1;

        /// <summary>
        /// Wrapper property for label size on this page
        /// if font size is -1 or so, then font size for document is used
        /// </summary>
        [Browsable(true),DisplayName("Default Font Size"),Editor(typeof(FontSizeEditor),typeof(UITypeEditor)),Category("Page")]
        public int DefaultLabelFontSize
        {
            get
            {
                if (def_font_size < 10)
                    return (int)Document.LabelFontSize;
                return def_font_size;
            }
            set
            {
                def_font_size = (value == (int)Document.LabelFontSize ? -1 : value);
            }
        }

        [Browsable(true),DisplayName("Label Font"),Category("Inherited")]
        public Font DefaultLabelFont
        {
            get
            {
                if (DefaultLabelFontSize < 10)
                    return Document.LabelFont;
                return MNDocument.FontOfSize(DefaultLabelFontSize);
            }
        }

        [Browsable(true), DisplayName("Note Font"),Category("Inherited")]
        public Font DefaultSubLabelFont
        {
            get
            {
                return Document.SubLabelFont;
            }
        }

        public List<SMControl> Objects = new List<SMControl>();

        public MNPage(MNDocument doc)
        {
            Document = doc;
            DefaultTextImageSize = new Size(180,180);
            DefaultLabelFontSize = -1;

            Rulers.Add(new MNPageRuler() { IsRelative = true, IsHorizontal = true, Value = 0, Name = "Page Top Side" });
            Rulers.Add(new MNPageRuler() { IsRelative = true, IsHorizontal = true, Value = 1000, Name = "Page Bottom Side" });
            Rulers.Add(new MNPageRuler() { IsRelative = true, IsVertical = true, Value = 0, Name = "Page Left Side" });
            Rulers.Add(new MNPageRuler() { IsRelative = true, IsVertical = true, Value = 1000, Name = "Page Right Side" });
        }

        public bool HasSelectedObjects()
        {
            foreach (SMControl po in Objects)
            {
                if (po.Selected != SMControlSelection.None)
                    return true;
            }

            return false;
        }

        public void ClearSelection()
        {
            foreach (SMControl item in Objects)
            {
                item.Selected = SMControlSelection.None;
            }
        }

        public void DeleteSelectedObjects()
        {
            List<SMControl> objectsForDelete = new List<SMControl>();
            HashSet<SMControl> selectedParentObjects = new HashSet<SMControl>();

            foreach (SMControl item in Objects)
            {
                if (item.Selected != SMControlSelection.None)
                {
                    if (item.ParentObject == null)
                    {
                        objectsForDelete.Add(item);
                    }
                    else
                    {
                        if (!selectedParentObjects.Contains(item.ParentObject))
                        {
                            objectsForDelete.Add(item.ParentObject);
                            selectedParentObjects.Add(item.ParentObject);
                        }
                    }
                }
            }

            foreach (SMControl item in Objects)
            {
                try
                {
                    if (selectedParentObjects.Contains(item.ParentObject))
                    {
                        objectsForDelete.Add(item);
                    }
                }
                catch { }
            }

            // actual delete
            foreach (SMControl item in objectsForDelete)
            {
                try {
                    Objects.Remove(item);
                }
                catch {
                }
            }
        }

        public void Paint(MNPageContext context)
        {
            try
            {
                Graphics g = context.g;

                // draw rulers
                foreach (MNPageRuler pr in this.Rulers)
                {
                    pr.Paint(context);
                }

                // draw objects
                foreach (SMControl po in this.Objects)
                {
                    po.Paint(context);
                }

            }
            catch (Exception ex)
            {
                Debugger.Log(0, "", ex.StackTrace);
                Debugger.Log(0, "", "\n\n");
            }
        }


        public XmlElement Save(XmlDocument doc)
        {
            XmlElement e = doc.CreateElement("page");

            e.SetAttribute("idx", PageIndex.ToString());
            e.SetAttribute("title", Title);
            e.SetAttribute("dlfs", DefaultLabelFontSize.ToString());
            foreach (SMControl po in Objects)
            {
                if (po.ParentObject != null) continue;
                e.AppendChild(po.Save(doc));
            }
            return e;
        }

        public void Load(XmlElement e)
        {
            PageIndex = int.Parse(e.GetAttribute("idx"));
            Title = e.GetAttribute("title");
            DefaultLabelFontSize = int.Parse(e.GetAttribute("dlfs"));
            foreach (XmlElement e1 in e.ChildNodes)
            {
                SMControl po = null;

                try
                {
                    // creating MNPageImage, MNPagePoint, etc... object from class' name
                    string typeName = this.GetType().Namespace + "." + e1.Name;
                    Type t = Type.GetType(typeName);
                    object instance = Activator.CreateInstance(t, this);
                    if (instance is SMControl)
                        po = (SMControl)instance;
                }
                catch
                {
                    po = null;
                }

/*                if (e1.Name.Equals("MNPageImage"))
                {
                    po = new MNPageImage(this);
                }
                else if (e1.Name.Equals("MNPagePoint"))
                {
                    po = new MNPagePoint(this);
                }
                else if (e1.Name.Equals("MNPageMantra"))
                {
                    po = new MNPageMantra(this);
                }
                else if (e1.Name.Equals("MNPageTextObject"))
                {
                    po = new MNPageTextObject(this);
                }
                else if (e1.Name.Equals("MNPageTextWithImage"))
                {
                    po = new MNPageTextWithImage(this);
                }
                else if (e1.Name.Equals("MNLine"))
                {
                    po = new MNLine(this);
                }*/

                if (po != null)
                {
                    po.Page = this;
                    List<SMControl> list = po.Load(e1);
                    Objects.AddRange(list);
                }
            }
        }
    }
}
