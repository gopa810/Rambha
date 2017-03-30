using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Document;
using Rambha.Serializer;

namespace SlideMaker
{
    public class MNSharedObjects
    {
        public static MNDocument internalDocument = new MNDocument();

        public static string FilePath = @"e:\Dropbox\Books for Software\00 General\Library.sml";

        public static void AddImage(MNReferencedImage img)
        {
            AddImageToArray(internalDocument, img, true);
        }

        public static void AddTemplate(MNPage page)
        {
            AddTemplateToArray(internalDocument, page, true);
        }

        public static void AddStyle(MNReferencedStyle style)
        {
            AddStyleToArray(internalDocument, style, true);
        }


        private static bool AddImageToArray(MNDocument doc, MNReferencedImage img, bool overwrite)
        {
            bool added = false;
            if (img == null || img.Name.Length == 0)
                return false;

            int index = -1;
            List<MNReferencedImage> array = doc.DefaultLanguage.Images;
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Name == img.Name)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                if (overwrite)
                {
                    array.RemoveAt(index);
                    array.Insert(index, img);
                    added = true;
                }
            }
            else
            {
                array.Add(img);
                added = true;
            }

            return added;
        }

        private static bool AddTemplateToArray(MNDocument doc, MNPage page, bool overwrite)
        {
            bool added = false;
            int index = -1;
            List<MNPage> array = doc.Data.Templates;

            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].APIName == page.APIName)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                if (overwrite)
                {
                    array.RemoveAt(index);
                    array.Insert(index, page);
                    added = true;
                }
            }
            else
            {
                array.Add(page);
                added = true;
            }

            if (added)
            {
                foreach (SMControl sc in page.Objects)
                {
                    if (sc is SMImage)
                    {
                        AddImageToArray(doc, (sc as SMImage).Img.Image, overwrite);
                    }
                    else if (sc is SMImageButton)
                    {
                        AddImageToArray(doc, (sc as SMImageButton).ImgA.Image, overwrite);
                        AddImageToArray(doc, (sc as SMImageButton).ImgB.Image, overwrite);
                    }
                }
            }

            return added;
        }

        private static bool AddStyleToArray(MNDocument doc, MNReferencedStyle style, bool overwrite)
        {
            bool added = false;
            if (style == null || style.Name.Length == 0)
                return false;
            List<MNReferencedStyle> array = doc.DefaultLanguage.Styles;

            int index = -1;
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Name == style.Name)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                if (overwrite)
                {
                    array.RemoveAt(index);
                    array.Insert(index, style);
                    added = true;
                }
            }
            else
            {
                array.Add(style);
                added = true;
            }

            return added;
        }

        public static void CopyToDocument(MNDocument doc)
        {
            foreach (MNPage page in internalDocument.Data.Templates)
            {
                if (AddTemplateToArray(doc, page, false))
                {
                    page.Document = doc;
                    page.Id = doc.Data.GetNextId();
                    foreach (SMControl sc in page.Objects)
                    {
                        long oldId = sc.Id;
                        sc.Id = doc.Data.GetNextId();
                        sc.Page = page;
                    }
                }
            }
            foreach (MNReferencedImage img in internalDocument.DefaultLanguage.Images)
            {
                if (AddImageToArray(doc, img, false))
                {
                    img.Id = doc.Data.GetNextId();
                }
            }
            foreach (MNReferencedStyle st in internalDocument.DefaultLanguage.Styles)
            {
                AddStyleToArray(doc, st, false);
            }

            // load again, since we have most probably destroyed structure of document
            Load();
        }

        public static void Load()
        {
            if (File.Exists(FilePath))
            {
                using (Stream sr = File.OpenRead(FilePath))
                {
                    using (BinaryReader br = new BinaryReader(sr))
                    {
                        RSFileReader fr = new RSFileReader(br);

                        byte b = 0;
                        internalDocument = new MNDocument();
                        while((b = fr.ReadByte()) != 0)
                        {
                            switch(b)
                            {
                                case 10:
                                    MNPage p = new MNPage(internalDocument);
                                    p.Load(fr);
                                    p.TemplateName = "";
                                    p.TemplateId = -1;
                                    internalDocument.Data.Templates.Add(p);
                                    break;
                                case 11:
                                    MNReferencedImage img = new MNReferencedImage();
                                    img.Load(fr);
                                    internalDocument.DefaultLanguage.Images.Add(img);
                                    break;
                                case 12:
                                    MNReferencedStyle st = new MNReferencedStyle();
                                    st.Load(fr);
                                    internalDocument.DefaultLanguage.Styles.Add(st);
                                    break;
                            }
                        }
                    }
                }

                //internalDocument.ReapplyStyles();
            }
        }

        public static void Save()
        {
            using (Stream stream = File.Create(FilePath))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    RSFileWriter fw = new RSFileWriter(bw);
                    foreach (MNPage page in internalDocument.Data.Templates)
                    {
                        fw.WriteByte(10);
                        page.Save(fw);
                    }
                    foreach (MNReferencedImage img in internalDocument.DefaultLanguage.Images)
                    {
                        fw.WriteByte(11);
                        img.Save(fw);
                    }
                    foreach (MNReferencedStyle st in internalDocument.DefaultLanguage.Styles)
                    {
                        fw.WriteByte(12);
                        st.Save(fw);
                    }

                    fw.WriteByte(0);
                }
            }
        }

    }
}
