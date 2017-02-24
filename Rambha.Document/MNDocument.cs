using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Design;
using System.Diagnostics;
using System.Xml;

using Rambha.Script;
using Rambha.Serializer;
using Rambha.Document;
using Rambha.Document.Views;

namespace Rambha.Document
{
    [Serializable()]
    public class MNDocument: GSCore
    {
        /// <summary>
        /// Header for book contains basic information for preview
        /// </summary>
        public MNBookHeader Book { get; set; }

        /// <summary>
        /// Main content of book
        /// </summary>
        public MNBookData Data { get; set; }

        /// <summary>
        /// Default language content
        /// </summary>
        public MNLocalisation DefaultLanguage { get; set; }

        /// <summary>
        /// Optional localized content
        /// </summary>
        public MNLocalisation CurrentLanguage { get; set; }


        public IDocumentViewDelegate Viewer = null;

        public MNDocument()
        {
            Book = new MNBookHeader();
            Data = new MNBookData(this);
            DefaultLanguage = new MNLocalisation();
            CurrentLanguage = null;

            // initialize global ID provider
            Data.InitializeID();

            // initialize default style
            InitialiseDefaultStyles();

            // create new default page
            CreateNewPage();

        }

        private void InitialiseDefaultStyles()
        {
            MNReferencedStyle ds = new MNReferencedStyle();
            ds.Name = "Default";
            ds.FontSize = 20f;
            ds.ForeColor = Color.Black;
            ds.BackColor = Color.Transparent;
            ds.BorderStyle = SMBorderStyle.None;
            ds.Align = SMHorizontalAlign.Center;
            ds.VertAlign = SMVerticalAlign.Center;
            ds.HighForeColor = Color.Blue;
            ds.HighBackColor = Color.LightBlue;
            DefaultLanguage.Styles.Add(ds);

            ds = new MNReferencedStyle();
            ds.Name = "Normal";
            ds.FontSize = 20f;
            ds.ForeColor = Color.Black;
            ds.BackColor = Color.Transparent;
            ds.BorderStyle = SMBorderStyle.None;
            ds.Align = SMHorizontalAlign.Left;
            ds.VertAlign = SMVerticalAlign.Top;
            ds.HighForeColor = Color.Blue;
            ds.HighBackColor = Color.LightBlue;
            ds.ContentPadding.Bottom = 5;
            ds.ContentPadding.Left = 5;
            ds.ContentPadding.Right = 5;
            ds.ContentPadding.Top = 5;
            DefaultLanguage.Styles.Add(ds);

            ds = new MNReferencedStyle();
            ds.Name = "NavigationButton";
            ds.FontSize = 20f;
            ds.ForeColor = Color.Black;
            ds.BackColor = Color.White;
            ds.BorderStyle = SMBorderStyle.RoundRectangle;
            ds.BorderColor = Color.Black;
            ds.BorderWidth = 1f;
            ds.HighBackColor = Color.Gray;
            ds.HighBorderColor = Color.DarkGreen;
            ds.HighBorderStyle = SMBorderStyle.RoundRectangle;
            ds.HighBorderWidth = 2f;
            ds.HighForeColor = Color.DarkGreen;
            ds.Align = SMHorizontalAlign.Center;
            ds.VertAlign = SMVerticalAlign.Center;
            DefaultLanguage.Styles.Add(ds);


            ds = new MNReferencedStyle();
            ds.Name = "PageHeader";
            ds.FontSize = 20f;
            ds.ForeColor = Color.White;
            ds.BackColor = Color.Gray;
            ds.BorderStyle = SMBorderStyle.Rectangle;
            ds.BorderColor = Color.LightGray;
            ds.BorderWidth = 2f;
            ds.Align = SMHorizontalAlign.Center;
            ds.VertAlign = SMVerticalAlign.Center;
            DefaultLanguage.Styles.Add(ds);

            ds = new MNReferencedStyle();
            ds.Name = "Footnote";
            ds.FontSize = 16f;
            ds.ForeColor = Color.Black;
            ds.BackColor = Color.Silver;
            ds.BorderStyle = SMBorderStyle.RoundRectangle;
            ds.CornerRadius = 10;
            ds.ContentPadding.Bottom = 10;
            ds.ContentPadding.Left = 15;
            ds.ContentPadding.Right = 15;
            ds.ContentPadding.Top = 10;
            ds.Align = SMHorizontalAlign.Left;
            ds.VertAlign = SMVerticalAlign.Top;
            ds.HighForeColor = Color.Blue;
            ds.HighBackColor = Color.LightBlue;
            DefaultLanguage.Styles.Add(ds);

            ds = new MNReferencedStyle();
            ds.Name = "BigLetters";
            ds.FontSize = 50f;
            ds.ForeColor = Color.Black;
            ds.BackColor = Color.Transparent;
            ds.BorderStyle = SMBorderStyle.None;
            ds.Align = SMHorizontalAlign.Center;
            ds.VertAlign = SMVerticalAlign.Center;
            ds.HighForeColor = Color.Blue;
            ds.HighBackColor = Color.LightBlue;
            DefaultLanguage.Styles.Add(ds);

            ds = new MNReferencedStyle();
            ds.Name = "MediumLetters";
            ds.FontSize = 35f;
            ds.ForeColor = Color.Black;
            ds.BackColor = Color.Transparent;
            ds.BorderStyle = SMBorderStyle.None;
            ds.Align = SMHorizontalAlign.Center;
            ds.VertAlign = SMVerticalAlign.Center;
            ds.HighForeColor = Color.Blue;
            ds.HighBackColor = Color.LightBlue;
            DefaultLanguage.Styles.Add(ds);


        }

        ~MNDocument()
        {
            Data.SaveID();
        }


        public MNReferencedStyle GetDefaultStyle()
        {
            return DefaultLanguage.Styles[0];
        }

        public bool HasViewer
        {
            get
            {
                return Viewer != null;
            }
        }

        /// <summary>
        /// Creates new page
        /// </summary>
        /// <returns></returns>
        public MNPage CreateNewPage()
        {
            MNPage page = new MNPage(this);
            page.Title = string.Format("<Page Title>");
            page.Document = this;
            page.Id = Data.GetNextId();
            Data.Pages.Add(page);

            MNNotificationCenter.BroadcastMessage(this, "PageInserted", page);

            return page;
        }

        /// <summary>
        /// Creates new template
        /// </summary>
        /// <returns></returns>
        public MNPage CreateNewTemplate()
        {
            MNPage page = new MNPage(this);
            page.Title = string.Format("<Template Title>");
            page.Document = this;
            page.Id = Data.GetNextId();
            Data.Templates.Add(page);

            MNNotificationCenter.BroadcastMessage(this, "PageInserted", page);

            return page;
        }

        public MNReferencedText CreateNewText(bool isScript)
        {
            MNReferencedText rt = new MNReferencedText();
            rt.Name = "Untitled";

            if (isScript)
                Data.Scripts.Add(rt);
            else
                DefaultLanguage.Texts.Add(rt);

            MNNotificationCenter.BroadcastMessage(this, "TextInserted", rt);

            return rt;
        }

        public bool HasContent()
        {
            if (DefaultLanguage.Images.Count > 0)
                return true;

            int count = 0;
            foreach (MNPage page in Data.Pages)
            {
                count += page.Objects.Count;
            }

            if (count > 0)
                return true;

            return false;
        }




        public override GSCore GetPropertyValue(string s)
        {
            switch(s)
            {
                case "title":
                    return new GSString(Book.BookTitle);
                default:
                    return base.GetPropertyValue(s);
            }
        }

        public MNPage FindPage(string pageName)
        {
            foreach (MNPage page in Data.Pages)
            {
                if (page.Title.Equals(pageName))
                    return page;
            }
            return null;
        }

        public MNPage FindPageId(long pageId)
        {
            foreach (MNPage page in Data.Pages)
            {
                if (page.Id == pageId)
                    return page;
            }
            return null;
        }

        public MNReferencedImage FindImage(string p)
        {
            foreach (MNReferencedImage img in DefaultLanguage.Images)
            {
                if (img.Name.Equals(p))
                    return img;
            }
            return null;
        }

        public MNReferencedImage FindImage(long imageId)
        {
            foreach (MNReferencedImage img in DefaultLanguage.Images)
            {
                if (img.Id == imageId)
                    return img;
            }
            return null;
        }

        public MNPage GetTemplate(long templateId)
        {
            foreach (MNPage template in Data.Templates)
            {
                if (template.Id == templateId)
                    return template;
            }

            return null;
        }

        public object AcceptFile(string sFileName)
        {
            string extension = Path.GetExtension(sFileName);

            switch(extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                    {
                        foreach (MNReferencedImage mri in DefaultLanguage.Images)
                        {
                            if (mri.FilePath.Equals(sFileName))
                                return mri;
                        }

                        Image loadedImage = Image.FromFile(sFileName);
                        MNReferencedImage ri = CreateNewImage();
                        ri.ImageData = loadedImage;
                        ri.Name = Path.GetFileNameWithoutExtension(sFileName);
                        ri.FilePath = sFileName;
                        ri.Description = string.Format("{0}x{1}", loadedImage.Width, loadedImage.Height);
                        return ri;
                    }
                default:
                    return null;
            }
        }


        public MNPage GetPage(string p)
        {
            foreach (MNPage page in Data.Pages)
            {
                if (page.Title.Equals(p)) return page;
            }

            if (Data.Pages.Count > 0) return Data.Pages[0];

            return null;
        }

        public MNPage GetPage(int pageId)
        {
            foreach (MNPage page in Data.Pages)
            {
                if (page.Id == pageId) return page;
            }

            foreach (MNPage templ in Data.Templates)
            {
                if (templ.Id == pageId) return templ;
            }

            return null;
        }

        public SMControl FindControl(int controlId)
        {
            SMControl ctrl = null;
            foreach (MNPage page in Data.Pages)
            {
                ctrl = page.FindObject(controlId);
                if (ctrl != null)
                    return ctrl;
            }

            foreach (MNPage page in Data.Templates)
            {
                ctrl = page.FindObject(controlId);
                if (ctrl != null)
                    return ctrl;
            }

            return null;
        }

        public MNReferencedStyle FindStyle(string styleName)
        {
            MNReferencedStyle s = null;
            if (CurrentLanguage != null)
                s = CurrentLanguage.FindStyle(styleName);
            if (s != null)
                return s;
            if (DefaultLanguage != null)
                s = DefaultLanguage.FindStyle(styleName);
            return s;
        }


        public MNMenu FindMenu(string apiNameMenu)
        {
            foreach (MNMenu m in Data.Menus)
            {
                if (m.APIName.ToLower().Equals(apiNameMenu.ToLower()))
                    return m;
            }
            return null;
        }

        public string ObjectTypeToTag(Type a)
        {
            if (a == typeof(MNPage)) return "MNPage";
            if (a == typeof(MNReferencedImage)) return "MNReferencedImage";
            return "";
        }

        public object TagToObject(string tag)
        {
            switch (tag)
            {
                case "MNPage": return new MNPage(this);
                case "MNReferencedImage": return new MNReferencedImage();
                default: return null;
            }
        }

        public virtual bool HasImmediateEvaluation
        {
            get
            {
                switch (Book.Evaluation)
                {
                    case MNEvaluationType.Immediate: return true;
                    default: return false;
                }
            }
        }

        public virtual bool HasLazyEvaluation
        {
            get
            {
                switch (Book.Evaluation)
                {
                    case MNEvaluationType.Lazy: return true;
                    case MNEvaluationType.Inherited: return true;
                    default: return false;
                }
            }
        }



        public MNPage FindPageWithIndex(int pageIndex)
        {
            foreach (MNPage page in Data.Pages)
                if (page.Index == pageIndex)
                    return page;
            return null;
        }

        public MNReferencedImage CreateNewImage()
        {
            MNReferencedImage image = new MNReferencedImage();
            image.Id = Data.GetNextId();
            DefaultLanguage.Images.Add(image);
            return image;
        }

        public MNReferencedText FindText(string styleName)
        {
            MNReferencedText s = null;
            if (CurrentLanguage != null)
                s = CurrentLanguage.FindText(styleName);
            if (s != null)
                return s;
            if (DefaultLanguage != null)
                s = DefaultLanguage.FindText(styleName);
            return s;
        }

        public MNReferencedCore FindContentObject(SMContentType type, string contentId)
        {
            MNReferencedCore value = null;

            if (CurrentLanguage != null)
            {
                value = CurrentLanguage.FindObject(contentId);
            }

            if (value == null && type == SMContentType.Text)
            {
                MNReferencedText rt = FindText(contentId);
                if (rt != null)
                {
                    MNReferencedText str = new MNReferencedText();
                    str.Text = rt.Text;
                    value = str;
                }
            }

            return value;
        }


        public MNPage FindTemplateId(long p_template_lazy)
        {
            foreach (MNPage page in Data.Templates)
            {
                if (page.Id == p_template_lazy)
                    return page;
            }
            return null;
        }

        public void SaveBookStatus()
        {
            string fileName = Book.FilePath.Replace(".smb", ".sms");
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileName)))
            {
                RSFileWriter fw = new RSFileWriter(bw);
                Data.SaveStatus(fw);
            }
        }


        public MNPage InsertPage(MNPage refPage, string newPageName, bool bShouldInsertAfter)
        {
            MNPage newPage = new MNPage(this);
            newPage.Title = newPageName;
            int index = Data.Pages.IndexOf(refPage);
            int newIndex = Data.Pages.Count;
            if (index >= 0)
            {
                if (bShouldInsertAfter)
                    newIndex = index + 1;
                else
                    newIndex = index;
            }

            Data.Pages.Insert(newIndex, newPage);
            MNNotificationCenter.BroadcastMessage(this, "NewPageInserted", newPage);

            return newPage;
        }
    }

    public enum MNEvaluationType
    {
        None = 0,
        Inherited = 1,
        Lazy = 2,
        Immediate = 3
    }

    public enum MNEvaluationResult
    {
        NotEvaluated = 0,
        Correct = 1,
        Incorrect = 2,
        Focused = 3
    }
}


