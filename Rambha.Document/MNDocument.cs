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
    public class MNDocument: GSCore, IRSObjectResolver, IRSObjectOrigin, IRSUniqueIdProvider
    {
        private static byte[] p_fileHeader = new byte[] {
            1, 1, 2, 3, 
            5, 8, 13, 21,
            34, 55, 89, 144,
            0, 7, 7, 0
        };

        private static int p_id = -1;

        public SMRectangleArea Area = new SMRectangleArea();

        [Browsable(true), DisplayName("Book Title"), Category("Document")]
        public string BookTitle { get; set; }

        [Browsable(true), DisplayName("Book Code"), Category("Document")]
        public string BookCode { get; set; }

        [Browsable(true), DisplayName("Book Author"), Category("Document")]
        public string BookAuthor { get; set; }

        [Browsable(true), DisplayName("Publisher"), Category("Document")]
        public string BookPublisher { get; set; }

        [Browsable(true), DisplayName("Copyright"), Category("Document")]
        public string BookCopyright { get; set; }

        [Browsable(true), DisplayName("Collection"), Category("Document")]
        public string BookCollection { get; set; }


        [Editor(typeof(ImageSelectionPropertyEditor), typeof(UITypeEditor))]
        [Browsable(true), DisplayName("Book Icon"), Category("Document")]
        public MNReferencedImage BookImageRef { get; set; }

        [Browsable(false)]
        public Image BookImage { get; set; }

        [Browsable(true), Category("Evaluation")]
        public MNEvaluationType Evaluation { get; set; }


        [Browsable(true), DisplayName("Page Size"), Category("Page"), Description("Size of page in points. For simplicity purpose, you can take 1 point = 1 pixel")]
        public Size PageSize { get; set; }

        [Browsable(true), Category("Document")]
        public String StartPage { get; set; }

        public List<MNReferencedImage> Images = new List<MNReferencedImage>();
        public List<MNPage> Pages = new List<MNPage>(); 
        public List<MNPage> Templates = new List<MNPage>();

        public List<SMStyle> Styles = new List<SMStyle>();

        /// <summary>
        /// Message from loaded of document
        /// if loading was OK, then message is empty
        /// </summary>
        public string LoadMessage = string.Empty;
        private int sizeOfInt = sizeof(int);

        // constants
        public static readonly int PO_PORTAIT = 0;
        public static readonly int PO_LANDSCAPE = 1;
        public static readonly int PS_A4 = 0;
        public static readonly int PS_LETTER = 1;
        public static int DotPerMM = 12;

        public IDocumentViewDelegate Viewer = null;

        public MNDocument()
        {
            // initialize global ID provider
            InitializeID();

            // default page size
            PageSize = new Size(1024, 768);

            BookImage = null;
            BookImageRef = null;

            // initialize default style
            SMStyle ds = new SMStyle();
            ds.Name = "Default";
            ds.Font = new Font(FontFamily.GenericSerif, 20f, FontStyle.Regular);
            ds.ForeColor = Color.Black;
            ds.BackColor = Color.Transparent;
            ds.BorderStyle = SMBorderStyle.None;
            ds.ContentAlign = SMContentAlign.Center;
            ds.Id = GetNextId();
            Styles.Add(ds);

            // create new default page
            CreateNewPage();

        }

        ~MNDocument()
        {
            SaveID();
        }

        public void InitializeID()
        {
            if (p_id < 0)
                p_id = Properties.Settings.Default.UniqueID;
        }

        public void SaveID()
        {
            Properties.Settings.Default.UniqueID = p_id;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Generating unique ID
        /// </summary>
        /// <returns></returns>
        public long GetNextId()
        {
            return p_id++;
        }

        public SMStyle GetDefaultStyle()
        {
            return Styles[0];
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
            page.Id = GetNextId();
            Pages.Add(page);

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
            page.Id = GetNextId();
            Templates.Add(page);

            MNNotificationCenter.BroadcastMessage(this, "PageInserted", page);

            return page;
        }

        public bool HasContent()
        {
            if (Images.Count > 0)
                return true;

            int count = 0;
            foreach (MNPage page in Pages)
            {
                count += page.Objects.Count;
            }

            if (count > 0)
                return true;

            return false;
        }

        public int PageWidth
        {
            get
            {
                return 200 * DotPerMM;
            }
        }

        public int PageHeight
        {
            get
            {
                return 200 * DotPerMM;
            }
        }


        public void Save(RSFileWriter bw)
        {
            bw.Log("===DOCUMENT START===\n");

            bw.WriteBytes(p_fileHeader);

            bw.WriteByte(20);
            bw.WriteByte(sizeof(int));

            bw.WriteByte(21);
            bw.WriteInt32(p_id);

            bw.WriteTagString(30, BookTitle);
            bw.WriteTagString(31, BookCode);
            bw.WriteTagString(32, BookAuthor);
            bw.WriteTagString(33, BookPublisher);
            bw.WriteTagString(34, BookCopyright);
            bw.WriteTagString(35, BookCollection);

            bw.WriteByte(23);
            bw.WriteInt32(PageSize.Width);
            bw.WriteInt32(PageSize.Height);

            bw.WriteTagString(24, StartPage);

            bw.WriteByte(25);
            Area.Save(bw);

            if (BookImageRef != null)
            {
                bw.WriteByte(26);
                bw.WriteImage(BookImageRef.ImageData);
                bw.WriteByte(27);
                bw.WriteInt64(BookImageRef.Id);
            }
            else if (BookImage != null)
            {
                bw.WriteByte(26);
                bw.WriteImage(BookImage);
            }

            bw.WriteByte(28);
            bw.WriteInt32((Int32)Evaluation);

            // saving big data
            // before each data piece we write its length
            // so when reading only header info, we skip reading all detailed data
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bms = new BinaryWriter(ms))
                {
                    // tempw is used as temporary memory stream
                    // for determining size of data to be writen
                    RSFileWriter tempw = new RSFileWriter(bms);

                    // writing pages
                    bw.WriteByte(51);
                    ms.SetLength(0);
                    foreach (MNReferencedImage ri in Images) ri.Save(tempw);
                    bw.WriteInt64(ms.Length + sizeof(int));
                    bw.WriteInt32(Images.Count);
                    foreach (MNReferencedImage ri in Images)
                    {
                        ri.Save(bw);
                    }

                    bw.WriteByte(52);
                    ms.SetLength(0);
                    foreach (MNPage ri in Pages) ri.Save(tempw);
                    bw.WriteInt64(ms.Length + sizeof(int));
                    bw.WriteInt32(Pages.Count);
                    foreach (MNPage ri in Pages)
                    {
                        ri.Save(bw);
                    }

                    bw.WriteByte(53);
                    ms.SetLength(0);
                    foreach (MNPage ri in Templates) ri.Save(tempw);
                    bw.WriteInt64(ms.Length + sizeof(int));
                    bw.WriteInt32(Templates.Count);
                    foreach (MNPage ri in Templates)
                    {
                        ri.Save(bw);
                    }

                    bw.WriteByte(54);
                    ms.SetLength(0);
                    tempw.WriteInt32(Styles.Count);
                    foreach (SMStyle ri in Styles) ri.Save(tempw);
                    bw.WriteInt64(ms.Length);
                    bw.WriteInt32(Styles.Count);
                    foreach (SMStyle ri in Styles)
                    {
                        ri.Save(bw);
                    }
                }
            }

            // posledny byte suboru
            bw.WriteByte(0);

            bw.Log("===DOCUMENT END===\n");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <param name="fullRead">TRUE if whole content is loaded to memory, FALSE if only header info are read</param>
        /// <returns></returns>
        public bool Load(RSFileReader br, bool fullRead)
        {
            Images.Clear();
            Pages.Clear();
            Templates.Clear();
            Styles.Clear();

            br.Log("===DOCUMENT START===\n");

            byte[] ha = br.ReadBytes(16);
            if (!ByteArrayCompare(ha, p_fileHeader))
            {
                LoadMessage = "File Header does not equal to expected value.";
                return false;
            }

            byte tag;
            int a, b;
            long size;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 20:
                        sizeOfInt = br.ReadByte();
                        break;
                    case 21: p_id = br.ReadInt32(); break;
                    case 30: BookTitle = br.ReadString(); break;
                    case 31: BookCode = br.ReadString(); break;
                    case 32: BookAuthor = br.ReadString(); break;
                    case 33: BookPublisher = br.ReadString(); break;
                    case 34: BookCopyright = br.ReadString(); break;
                    case 35: BookCollection = br.ReadString(); break;
                    case 23:
                        a = br.ReadInt32();
                        b = br.ReadInt32();
                        PageSize = new Size(a, b);
                        break;
                    case 24:
                        StartPage = br.ReadString();
                        break;
                    case 25: Area.Load(br); break;
                    case 26: BookImage = br.ReadImage(); break;
                    case 27:
                        //BookImageRef = new MNReferencedImageLoadingPlaceholder() { imageId = br.ReadInt32() };
                        br.AddReference(this, "Image", br.ReadInt64(), 27, this);
                        break;
                    case 28:
                        Evaluation = (MNEvaluationType)br.ReadInt32(); break;
                    case 51:
                        size = br.ReadInt64();
                        if (fullRead)
                        {
                            a = br.ReadInt32();
                            for (b = 0; b < a; b++)
                            {
                                MNReferencedImage m = new MNReferencedImage(this);
                                m.Load(br);
                                Images.Add(m);
                            }
                        }
                        else
                        {
                            br.Offset(size);
                        }
                        break;
                    case 52:
                        size = br.ReadInt64();
                        if (fullRead)
                        {
                            a = br.ReadInt32();
                            for (b = 0; b < a; b++)
                            {
                                MNPage m = new MNPage(this);
                                m.Load(br);
                                Pages.Add(m);
                            }
                        }
                        else
                        {
                            br.Offset(size);
                        }
                        break;
                    case 53:
                        size = br.ReadInt64();
                        if (fullRead)
                        {
                            a = br.ReadInt32();
                            for (b = 0; b < a; b++)
                            {
                                MNPage m = new MNPage(this);
                                m.Load(br);
                                Templates.Add(m);
                            }
                        }
                        else
                        {
                            br.Offset(size);
                        }
                        break;
                    case 54:
                        size = br.ReadInt64();
                        if (fullRead)
                        {
                            a = br.ReadInt32();
                            for (b = 0; b < a; b++)
                            {
                                SMStyle m = new SMStyle();
                                m.Load(br);
                                Styles.Add(m);
                            }
                        }
                        else
                        {
                            br.Offset(size);
                        }
                        break;
                }
            }

            br.Log("===DOCUMENT END===\n");


            //
            // RESTORE all references
            br.ResolveReferences(this);

            return true;
        }


        public override GSCore GetPropertyValue(string s)
        {
            switch(s)
            {
                case "title":
                    return new GSString(BookTitle);
                default:
                    return base.GetPropertyValue(s);
            }
        }

        void IRSObjectOrigin.setReference(int tag, object obj)
        {
            switch (tag)
            {
                case 27:
                    BookImageRef = (MNReferencedImage)obj;
                    break;
            }
        }

        object IRSObjectResolver.IRSResolver_FindObject(string objType, long objId)
        {
            if (objType == "MNPage")
            {
                return GetPage((int)objId);
            }
            else if (objType == "SMControl")
            {
                return FindControl((int)objId);
            }
            else if (objType == "Image")
            {
                return FindImage((int)objId);
            }
            else if (objType == "SMStyle")
            {
                return FindStyle((int)objId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Comparing two byte arrays
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        public MNReferencedImage FindImage(string p)
        {
            foreach (MNReferencedImage img in Images)
            {
                if (img.Title.Equals(p))
                    return img;
            }
            return null;
        }

        public MNReferencedImage FindImage(long imageId)
        {
            foreach (MNReferencedImage img in Images)
            {
                if (img.Id == imageId)
                    return img;
            }
            return null;
        }

        public MNPage GetTemplate(long templateId)
        {
            foreach (MNPage template in Templates)
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
                        foreach (MNReferencedImage mri in Images)
                        {
                            if (mri.FilePath.Equals(sFileName))
                                return mri;
                        }

                        Image loadedImage = Image.FromFile(sFileName);
                        MNReferencedImage ri = new MNReferencedImage(this);
                        ri.Id = GetNextId();
                        ri.ImageData = loadedImage;
                        ri.Title = Path.GetFileNameWithoutExtension(sFileName);
                        ri.FilePath = sFileName;
                        ri.Description = string.Format("{0}x{1}", loadedImage.Width, loadedImage.Height);
                        this.Images.Add(ri);
                        return ri;
                    }
                default:
                    return null;
            }
        }


        public MNPage GetPage(string p)
        {
            foreach (MNPage page in Pages)
            {
                if (page.Title.Equals(p)) return page;
            }

            if (Pages.Count > 0) return Pages[0];

            return null;
        }

        public MNPage GetPage(int pageId)
        {
            foreach (MNPage page in Pages)
            {
                if (page.Id == pageId) return page;
            }

            foreach (MNPage templ in Templates)
            {
                if (templ.Id == pageId) return templ;
            }

            return null;
        }

        public SMControl FindControl(int controlId)
        {
            SMControl ctrl = null;
            foreach (MNPage page in Pages)
            {
                ctrl = page.FindObject(controlId);
                if (ctrl != null)
                    return ctrl;
            }

            foreach (MNPage page in Templates)
            {
                ctrl = page.FindObject(controlId);
                if (ctrl != null)
                    return ctrl;
            }

            return null;
        }

        public SMStyle FindStyle(string styleName)
        {
            foreach (SMStyle s in Styles)
            {
                if (s.Name.Equals(styleName))
                    return s;
            }
            return null;
        }

        public SMStyle FindStyle(int styleId)
        {
            foreach (SMStyle s in Styles)
            {
                if (s.Id == styleId) return s;
            }
            return null;
        }

        public string ObjectTypeToTag(Type a)
        {
            if (a == typeof(MNPage)) return "MNPage";
            if (a == typeof(MNReferencedImage)) return "ReferencedImage";
            return "";
        }

        public object TagToObject(string tag)
        {
            switch (tag)
            {
                case "MNPage": return new MNPage(this);
                case "ReferencedImage": return new MNReferencedImage(this);
                default: return null;
            }
        }

        public virtual bool HasImmediateEvaluation
        {
            get
            {
                switch (Evaluation)
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
                switch (Evaluation)
                {
                    case MNEvaluationType.Lazy: return true;
                    case MNEvaluationType.Inherited: return true;
                    default: return false;
                }
            }
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


