using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Design;

using Rambha.Document.Views;
using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    [Serializable()]
    public class MNPage: GSCore, IRSObjectOrigin
    {
        [Browsable(true), ReadOnly(true)]
        public long Id { get; set; }

        [Browsable(false)]
        public MNDocument Document { get; set; }

        [Browsable(true),DisplayName("Page Title"),Category("Page")]
        public string Title { get { return p_title_obj.Value; } set { p_title_obj.Value = value; } }
        private GSString p_title_obj = new GSString("");

        [Browsable(true), DisplayName("Page Description"), Category("Page")]
        public string Description { get; set; }

        [Browsable(false)]
        public bool IsTemplate { get; set; }

        [Browsable(true), Category("Evaluation")]
        public MNEvaluationType Evaluation { get; set; }

        [Browsable(true), Category("Events")]
        public string UserEventOnClick { get; set; }
        public string UserEventOnDoubleClick { get; set; }
        public string UserEventOnLongClick { get; set; }


        public List<SMControl> Objects = new List<SMControl>();

        public Dictionary<long, SMRectangleArea> Areas = new Dictionary<long, SMRectangleArea>();

        public List<SMConnection> Connections = new List<SMConnection>();

        public SMRectangleArea Area = new SMRectangleArea();

        // not stored
        //

        [Browsable(true), Editor(typeof(TemplateSelectionEditor), typeof(UITypeEditor))]
        public MNPage Template { get { return p_template; } set { p_template = value; } 
        }
        private MNPage p_template = null;

        [Browsable(false)]
        public long TemplateId { get { MNPage t = Template;  return (t != null ? t.Id : -1); } }

        public int ItemHeight = 0;
        public int ItemTextHeight = 0;


        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "title":
                    return p_title_obj;
                default:
                    return base.GetPropertyValue(s);
            }
        }

        public void Save(RSFileWriter bw)
        {
            // basic info
            bw.WriteByte(10);
            bw.WriteInt64(Id);
            bw.WriteString(Title);
            bw.WriteString(Description);
            bw.WriteBool(IsTemplate);
            bw.WriteInt64(TemplateId);
            bw.WriteInt32((Int32)Evaluation);

            // objects
            bw.WriteByte(11);
            bw.WriteInt32(Objects.Count);
            for (int i = 0; i < Objects.Count; i++)
            {
                string sb = this.ObjectTypeToTag(Objects[i].GetType());
                if (sb.Length > 0)
                {
                    bw.WriteString(sb);
                    Objects[i].Save(bw);
                }
            }

            // areas
            bw.WriteByte(12);
            bw.WriteInt32(Areas.Count);
            foreach(KeyValuePair<long,SMRectangleArea> pp in Areas)
            {
                bw.WriteInt64(pp.Key);
                pp.Value.Save(bw);
            }

            // connections
            bw.WriteByte(13);
            bw.WriteInt32(Connections.Count);
            for (int i = 0; i < Connections.Count; i++)
            {
                Connections[i].Save(bw);
            }

            bw.WriteByte(14);
            Area.Save(bw);

            bw.WriteByte(15);
            bw.WriteString(UserEventOnClick);
            bw.WriteString(UserEventOnDoubleClick);
            bw.WriteString(UserEventOnLongClick);

            // end of object
            bw.WriteByte(0);
        }

        public bool Load(RSFileReader br)
        {
            byte tag;
            int c;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        Id = br.ReadInt64();
                        Title = br.ReadString();
                        Description = br.ReadString();
                        IsTemplate = br.ReadBool();
                        //Template = new MNPageLoadingPlaceholder() { pageId = br.ReadInt32() };
                        br.AddReference(Document, "MNPage", br.ReadInt64(), 10, this);
                        Evaluation = (MNEvaluationType)br.ReadInt32();
                        break;
                    case 11:
                        c = br.ReadInt32();
                        Objects.Clear();
                        for (int i = 0; i < c; i++)
                        {
                            string type = br.ReadString();
                            SMControl control = (SMControl)this.TagToObject(type);
                            if (control == null)
                                throw new Exception("Unknown control type: " + type);
                            control.Load(br);
                            Objects.Add(control);
                        }
                        break;
                    case 12:
                        c = br.ReadInt32();
                        Areas.Clear();
                        for (int i = 0; i < c; i++)
                        {
                            long key = br.ReadInt64();
                            SMRectangleArea sa = new SMRectangleArea();
                            sa.Load(br);
                            Areas.Add(key, sa);
                        }
                        break;
                    case 13:
                        c = br.ReadInt32();
                        Connections.Clear();
                        for (int i = 0; i < c; i++)
                        {
                            SMConnection conn = new SMConnection(Document);
                            conn.Load(br);
                            Connections.Add(conn);
                        }
                        break;
                    case 14:
                        Area.Load(br);
                        break;
                    case 15:
                        UserEventOnClick = br.ReadString();
                        UserEventOnDoubleClick = br.ReadString();
                        UserEventOnLongClick = br.ReadString();
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        public void setReference(int tag, object obj)
        {
            switch (tag)
            {
                case 10:
                    if (obj is MNPage)
                        Template = (MNPage)obj;
                    break;
            }
        }

        public MNPage(MNDocument doc): base()
        {
            Document = doc;
            IsTemplate = false;
            UserEventOnClick = "";
            UserEventOnDoubleClick = "";
            UserEventOnLongClick = "";
        }

        public override string ToString()
        {
            return Title;
        }

        public IEnumerable<SMControl> SortedObjects
        {
            get
            {
                return new EnumerableSortedObjects(this);
            }
        }

        /*public SMControlGroup FindControlGroup(SMControl ctrl)
        {
            if (ctrl == null) return null;
            foreach (SMControl obj in Objects)
            {
                if (obj is SMControlGroup)
                {
                    if ((obj as SMControlGroup).ContainsControl(ctrl))
                        return obj as SMControlGroup;
                }
            }
            return null;
        }*/

        public SMRectangleArea FindAreaGroup(long areaId, SMRectangleArea ctrl)
        {
            foreach (KeyValuePair<long,SMRectangleArea> obj in Areas)
            {
                if (obj.Key != areaId)
                {
                    if (obj.Value.GetRawRectangle(PageEditDisplaySize.LandscapeBig).Contains(ctrl.GetRawRectangle(PageEditDisplaySize.LandscapeBig)))
                        return obj.Value;
                }
            }
            return null;
        }

        public class EnumerableSortedObjects : IEnumerable<SMControl>
        {
            private MNPage _currentPage;
            public EnumerableSortedObjects(MNPage page)
            {
                _currentPage = page;
            }

            // Must implement GetEnumerator, which returns a new StreamReaderEnumerator.
            public IEnumerator<SMControl> GetEnumerator()
            {
                return new EnumeratorSortedObjects(_currentPage);
            }

            // Must also implement IEnumerable.GetEnumerator, but implement as a private method.
            private IEnumerator GetEnumerator1()
            {
                return this.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator1();
            }
        }

        public class EnumeratorSortedObjects: IEnumerator<SMControl>
        {
            public MNPage startPage = null;
            public MNPage currentPage = null;
            public int curIndex = -1;
            public int mode = 0;
            public SMControl currentObject = null;

            public EnumeratorSortedObjects(MNPage p)
            {
                startPage = p;
                currentPage = null;
                Reset();
            }

            public bool MoveNext()
            {
                // go through nongroup
                while (mode < 2)
                {
                    if (currentPage == null)
                        currentPage = startPage;
                    if (currentPage == null)
                        return false;

                    curIndex++;

                    if (curIndex >= currentPage.Objects.Count)
                    {
                        currentPage = currentPage.Document.GetTemplate(currentPage.TemplateId);
                        if (currentPage == null)
                        {
                            // go to mode 1
                            mode++;
                        }
                        curIndex = -1;
                        continue;
                    }

                    if (mode == 0)
                    {
                        if (!currentPage.Objects[curIndex].GroupControl)
                        {
                            currentObject = currentPage.Objects[curIndex];
                            return true;
                        }
                    }
                    // go through groups
                    else if (mode == 1)
                    {
                        if (currentPage.Objects[curIndex].GroupControl)
                        {
                            currentObject = currentPage.Objects[curIndex];
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }

            public void Reset() { curIndex = -1; currentPage = startPage; mode = 0; }

            void IDisposable.Dispose() { }

            public SMControl Current
            {
                get { return currentObject; }
            }


            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        public bool HasSelectedObjects()
        {
            foreach (KeyValuePair<long, SMRectangleArea> area in Areas)
            {
                if (area.Value.Selected) return true;
            }

            return false;
        }

        public List<SMControl> SelectedObjects
        {
            get
            {
                List<SMControl> lc = new List<SMControl>();
                foreach (SMControl item in Objects)
                {
                    SMRectangleArea area = GetArea(item.Id);
                    if (area.Selected)
                        lc.Add(item);
                }
                return lc;
            }
        }

        public void ClearSelection()
        {
            foreach (KeyValuePair<long, SMRectangleArea> area in Areas)
            {
                area.Value.Selected = false;
                area.Value.TrackedSelection = SMControlSelection.None;
            }
        }

        public void DeleteSelectedObjects()
        {
            List<SMControl> objectsForDelete = new List<SMControl>();
            HashSet<SMControl> selectedParentObjects = new HashSet<SMControl>();

            foreach (SMControl item in Objects)
            {
                SMRectangleArea area = GetArea(item.Id);
                if (area.Selected)
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

        public SMControl FindObject(long id)
        {
            foreach (SMControl s in Objects)
            {
                if (s.Id == id) return s;
            }

            return null;
        }

        public void Paint(MNPageContext context)
        {
            try
            {
                Graphics g = context.g;

                // draw connections
                foreach (SMConnection connection in this.Connections)
                {
                    connection.Paint(context);
                }

                // draw objects
                foreach (SMControl po in this.SortedObjects)
                {
                    po.Paint(context);
                }

            }
            catch (Exception ex)
            {
                Debugger.Log(0,"",ex.StackTrace);
                Debugger.Log(0, "", "\n\n");
            }
        }


        public SMRectangleArea CreateNewArea(long areaId)
        {
            SMRectangleArea area = new SMRectangleArea();
            Areas.Add(areaId, area);
            return area;
        }

        /// <summary>
        /// test for existence of the area in this page (or this template)
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public bool ContainsArea(long areaId)
        {
            return Areas.ContainsKey(areaId);
        }

        /// <summary>
        /// Retrieves area for drawing and editing. If area does not exist in current page, it looks for
        /// this area in templates, and if it exists in the templates, then copies it into current page.
        /// If area does not exist in the templates even, it creates new one just for this page.
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public SMRectangleArea GetArea(long areaId)
        {
            // lokk up in this page
            if (Areas.ContainsKey(areaId))
                return Areas[areaId];

            // look up in the templates
            long template = TemplateId;
            SMRectangleArea area = null;
            while (TemplateId > 0)
            {
                MNPage templatePage = Document.GetTemplate(template);
                if (templatePage == null)
                    break;
                if (templatePage.ContainsArea(areaId))
                {
                    area = templatePage.GetArea(areaId);
                    break;
                }
                template = templatePage.TemplateId;
            }

            // if area is null, then creates new fresh area
            area = new SMRectangleArea(area);
            Areas.Add(areaId, area);
            return area;
        }


        public void ClearConnections()
        {
            Connections.Clear();
        }

        public SMConnection FindConnection(SMControl from, SMControl to)
        {
            foreach (SMConnection sc in Connections)
            {
                if (sc.Source == from && sc.Target == to)
                {
                    return sc;
                }
            }
            return null;
        }

        public void AddConnection(SMControl from, SMControl to)
        {
            if (FindConnection(from, to) == null)
            {
                Connections.Add(new SMConnection(Document) { Source = from, Target = to });
            }
        }

        public void RemoveConnectionWithSource(SMControl source)
        {
            List<SMConnection> listToRemove = new List<SMConnection>();
            foreach (SMConnection conn in Connections)
            {
                if (conn.Source == source)
                    listToRemove.Add(conn);
            }
            foreach (SMConnection conn in listToRemove)
            {
                Connections.Remove(conn);
            }
        }

        public void RemoveConnectionWithTarget(SMControl target)
        {
            List<SMConnection> listToRemove = new List<SMConnection>();
            foreach (SMConnection conn in Connections)
            {
                if (conn.Target == target)
                    listToRemove.Add(conn);
            }
            foreach (SMConnection conn in listToRemove)
            {
                Connections.Remove(conn);
            }
        }

        public int CountConnectionsWithSource(SMControl source)
        {
            int count = 0;
            foreach (SMConnection conn in Connections)
            {
                if (conn.Source == source)
                    count++;
            }
            return count;
        }

        public int CountConnectionsWithTarget(SMControl target)
        {
            int count = 0;
            foreach (SMConnection conn in Connections)
            {
                if (conn.Target == target)
                    count++;
            }
            return count;
        }

        public virtual bool HasImmediateEvaluation
        {
            get
            {
                switch (Evaluation)
                {
                    case MNEvaluationType.Immediate: return true;
                    case MNEvaluationType.Inherited: return Document.HasImmediateEvaluation;
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
                    case MNEvaluationType.Inherited: return Document.HasLazyEvaluation;
                    default: return false;
                }
            }
        }

        public string ObjectTypeToTag(Type a)
        {
            if (a == typeof(SMCheckBox)) return "CheckBox";
            if (a == typeof(SMConnection)) return "Connection";
            if (a == typeof(SMDrawable)) return "Drawable";
            if (a == typeof(SMFreeDrawing)) return "FreeDrawing";
            if (a == typeof(SMImage)) return "Image";
            if (a == typeof(SMLabel)) return "Label";
            if (a == typeof(SMLetterInput)) return "LetterInput";
            if (a == typeof(SMTextContainer)) return "TextContainer";
            if (a == typeof(SMTextEdit)) return "TextEdit";
            if (a == typeof(SMTextPuzzle)) return "TextPuzzle";
            if (a == typeof(SMTextView)) return "TextView";
            if (a == typeof(SMKeyboard)) return "Keyboard";
            //if (a == typeof(SMControlGroup)) return "ControlGroup";

            return Document.ObjectTypeToTag(a);
        }

        public object TagToObject(string tag)
        {
            switch (tag)
            {
                case "CheckBox": return new SMCheckBox(this);
                case "Drawable": return new SMDrawable(this);
                case "FreeDrawing": return new SMFreeDrawing(this);
                case "Image": return new SMImage(this);
                case "Label": return new SMLabel(this);
                case "LetterInput": return new SMLetterInput(this);
                case "TextContainer": return new SMTextContainer(this);
                case "TextEdit": return new SMTextEdit(this);
                case "TextPuzzle": return new SMTextPuzzle(this);
                case "TextView": return new SMTextView(this);
                //case "ControlGroup": return new SMControlGroup(this);
                case "Keyboard": return new SMKeyboard(this);
                default: return Document.TagToObject(tag);
            }
        }

        public virtual void OnPageWillAppear()
        {
            if (Document.HasViewer)
                Document.Viewer.OnPageWillAppear(this);
        }

        public virtual void OnPageDidAppear()
        {
            if (Document.HasViewer)
                Document.Viewer.OnPageDidAppear(this);
        }

        public virtual void OnPageWillDisappear()
        {
            if (Document.HasViewer)
                Document.Viewer.OnPageWillDisappear(this);
        }

        public virtual void OnPageDidDisappear()
        {
            if (Document.HasViewer)
                Document.Viewer.OnPageDidDisappear(this);
        }
    }
}
