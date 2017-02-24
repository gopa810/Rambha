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
    public class MNPage: GSCore
    {
        [Browsable(true), ReadOnly(true)]
        public long Id { get; set; }

        [Browsable(false)]
        public MNDocument Document { get; set; }

        [Browsable(true),DisplayName("Page Title"),Category("Page")]
        public string Title { get { return p_title_obj.Value; } set { p_title_obj.Value = value; } }
        private GSString p_title_obj = new GSString("");

        [Browsable(true), DisplayName("API Name"), Category("API")]
        public string APIName { get { return p_title_obj.Value; } set { p_title_obj.Value = value; } }

        [Browsable(true), DisplayName("Page Description"), Category("Page")]
        public string Description { get; set; }

        [Browsable(false)]
        public bool IsTemplate { get; set; }

        [Browsable(true), Category("Evaluation")]
        public MNEvaluationType Evaluation { get; set; }

        private Color backgroundColor = Color.White;
        private Brush nontransparentBackgroundBrush = null;
        private Brush semitransparentBackgroundBrush = null;

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                nontransparentBackgroundBrush = new SolidBrush(backgroundColor);
                semitransparentBackgroundBrush = new SolidBrush(Color.FromArgb(128, backgroundColor));
            }
        }

        /// <summary>
        /// These are non-permanent properties for storing values during
        /// runtime. They are not saved.
        /// </summary>
        public Dictionary<string, GSCore> Properties = new Dictionary<string, GSCore>();

        public List<SMControl> Objects = new List<SMControl>();

        public List<MNReferencedText> Scripts = new List<MNReferencedText>();

        public Dictionary<long, SMRectangleArea> Areas = new Dictionary<long, SMRectangleArea>();

        public List<SMConnection> Connections = new List<SMConnection>();

        public SMRectangleArea Area = new SMRectangleArea();

        // not stored
        //

        [Browsable(true), Editor(typeof(TemplateSelectionEditor), typeof(UITypeEditor))]
        public MNPage Template 
        {
            get 
            {
                if (p_template == null && p_template_lazy > 0)
                {
                    p_template = Document.FindTemplateId(p_template_lazy);
                    p_template_lazy = -1;
                }
                return p_template;
            }
            set
            {
                p_template = value;
            }
        }
        [Browsable(false)]
        public long TemplateId
        {
            get { MNPage t = Template; return (t != null ? t.Id : p_template_lazy); }
            set { p_template_lazy = value; }
        }
        private MNPage p_template = null;
        private long p_template_lazy = -1;


        public int ItemHeight = 0;
        public int ItemTextHeight = 0;
        public int Index = 0;


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

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            string key;
            GSCore val;
            switch (token)
            {
                case "getValue":
                    key = args.getSafe(0).getStringValue();
                    if (Properties.ContainsKey(key))
                        return Properties[key];
                    return GSVoid.Void;
                case "setValue":
                    key = args.getSafe(0).getStringValue();
                    val = args.getSafe(1);
                    Properties[key] = val;
                    return val;
                case "checkAnswers":
                    return new GSBoolean(CheckAnswers(false));
                case "displayValidation":
                    CheckAnswers(false);
                    return GSVoid.Void;
                case "showHints":
                    ShowHints(true);
                    return GSVoid.Void;
                case "hideHints":
                    ShowHints(false);
                    return GSVoid.Void;
                case "displayAnswers":
                    DisplayAnswers();
                    return GSVoid.Void;
                case "test":
                    return new GSBoolean(Test());
                case "findControl":
                    key = args.getSafe(0).getStringValue();
                    foreach (SMControl ctrl in Objects)
                        if (ctrl.UniqueName.Equals(key))
                            return ctrl;
                    return GSVoid.Void;
                default:
                    return base.ExecuteMessage(token, args);
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
            bw.WriteColor(BackgroundColor);

            foreach (MNReferencedText rt in Scripts)
            {
                bw.WriteByte(16);
                rt.Save(bw);
            }

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
                        TemplateId = br.ReadInt64();
                        Evaluation = (MNEvaluationType)br.ReadInt32();
                        break;
                    case 11:
                        c = br.ReadInt32();
                        Objects.Clear();
                        for (int i = 0; i < c; i++)
                        {
                            string type = br.ReadString();
                            SMControl control = (SMControl)this.TagToObject(type);
                            control.Page = this;
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
                            SMConnection conn = new SMConnection(this);
                            conn.Load(br);
                            Connections.Add(conn);
                        }
                        break;
                    case 14:
                        Area.Load(br);
                        break;
                    case 15:
                        BackgroundColor = br.ReadColor();
                        break;
                    case 16:
                        MNReferencedText rt = new MNReferencedText();
                        rt.Load(br);
                        Scripts.Add(rt);
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        public MNPage(MNDocument doc): base()
        {
            Document = doc;
            IsTemplate = false;
            BackgroundColor = Color.White;
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
                    objectsForDelete.Add(item);
                }
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

        public void DuplicateSelectedObjects()
        {
            int i, m = Objects.Count;
            for (i = 0; i < m; i++)
            {
                SMControl item = Objects[i];
                SMRectangleArea area = GetArea(item.Id);
                if (area.Selected)
                {
                    area.Selected = false;
                    SMControl duplicated = item.Duplicate();
                    if (duplicated != null)
                    {
                        SMRectangleArea area2 = GetArea(duplicated.Id);
                        area2.Set(area, 30, 30);
                        area2.Selected = true;

                        Objects.Add(duplicated);
                    }
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

                g.FillRectangle(nontransparentBackgroundBrush, 0, 0, context.PageWidth, context.PageHeight);

                // draw connections
                foreach (SMConnection connection in this.Connections)
                {
                    connection.Paint(context);
                }

                // draw objects
                foreach (SMControl po in this.SortedObjects)
                {
                    if (!po.UIStateVisible)
                        continue;

                    SMRectangleArea area = GetArea(po.Id);
                    Rectangle rect = context.isTracking ? area.GetBoundsRecalc(context) : area.GetBounds(context);

                    po.Paint(context);

                    if (po.UIStateShowHint && po.Hints.Length > 0)
                    {
                        SizeF hintSize = g.MeasureString(po.Hints, context.HintFont);
                        Rectangle hintRect = new Rectangle(rect.Right - (int)hintSize.Width,
                            rect.Top - (int)hintSize.Height, (int)hintSize.Width, (int)hintSize.Height);
                        g.FillRectangle(Brushes.LightYellow, hintRect);
                        g.DrawString(po.Hints, context.HintFont, Brushes.Black, hintRect);
                    }

                    if (context.drawSelectionMarks)
                    {
                        g.DrawRectangle(Pens.LightGray, rect);
                    }

                    if (!po.UIStateEnabled)
                    {
                        g.FillRectangle(semitransparentBackgroundBrush, rect);
                    }
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

        public void AddConnection(SMConnection conn)
        {
            Connections.Add(conn);
        }

        public void AddConnection(SMControl from, SMControl to)
        {
            if (FindConnection(from, to) == null)
            {
                Connections.Add(new SMConnection(this) { Source = from, Target = to });
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
            if (a == typeof(SMMemoryGame)) return "MemoryGame";

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
                case "MemoryGame": return new SMMemoryGame(this);
                case "Keyboard": return new SMKeyboard(this);
                default: return Document.TagToObject(tag);
            }
        }

        public virtual void OnPageEvent(string eventName)
        {
            if (Document.HasViewer)
                Document.Viewer.OnEvent(eventName, this);
        }

        /// <summary>
        /// Checks if answers given by user are correct for each control.
        /// </summary>
        /// <param name="saveResults">If you want to save results of validation into control,
        /// so that control will reflet it in its appearance, then use value TRUE for this argument.
        /// If you dont want to change control's appearance, then use FALSE for this argument.</param>
        /// <returns></returns>
        public bool CheckAnswers(bool saveResults)
        {
            int errors = 0;
            foreach (SMControl ctrl in Objects)
            {
                MNEvaluationResult previous = ctrl.UIStateError;
                ctrl.Evaluate();
                if (ctrl.Evaluate() == MNEvaluationResult.Incorrect)
                    errors++;
                if (!saveResults)
                    ctrl.UIStateError = previous;
            }

            return errors > 0;
        }

        /// <summary>
        /// Shows expected values in all controls.
        /// </summary>
        public void DisplayAnswers()
        {
            foreach (SMControl ctrl in Objects)
            {
                if (ctrl.HasEvaluation)
                {
                    ctrl.DisplayAnswers();
                }
            }
        }

        public void ShowHints(bool state)
        {
            foreach (SMControl ctrl in Objects)
            {
                ctrl.UIStateShowHint = state;
            }
        }

        public bool Test()
        {
            long counter = 1;
            if (Properties.ContainsKey("testCounter"))
                counter = Properties["testCounter"].getIntegerValue();

            bool res = CheckAnswers(false);
            // res == false means, there are all answers valid (no incorrect ones)
            if (res == false)
                return true;

            Properties["testCounter"] = new GSInt32(counter + 1);
            if (counter == 1)
            {
                CheckAnswers(true);
                ShowHints(false);
                return false;
            }
            else if (counter == 2)
            {
                CheckAnswers(true);
                ShowHints(true);
                return false;
            }
            else
            {
                ShowHints(false);
                DisplayAnswers();
                CheckAnswers(true);
                return true;
            }
        }


        public void LoadStatus(RSFileReader br)
        {
            Connections.Clear();
            byte b;
            long mid;
            SMControl ct;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        SMConnection conn = new SMConnection(this);
                        conn.Load(br);
                        Connections.Add(conn);
                        break;
                    case 20:
                        mid = br.ReadInt64();
                        ct = FindObject(mid);
                        if (ct != null) ct.LoadStatus(br);
                        break;
                }
            }
        }

        public void SaveStatus(RSFileWriter bw)
        {
            foreach (SMConnection conn in Connections)
            {
                bw.WriteByte(10);
                conn.Save(bw);
            }

            foreach (SMControl ct in Objects)
            {
                bw.WriteByte(20);
                bw.WriteInt64(ct.Id);
                ct.SaveStatus(bw);
            }

            bw.WriteByte(0);
        }

        public int GetSelectedCount()
        {
            int count = 0;
            foreach(SMRectangleArea area in Areas.Values)
                if (area.Selected) count++;
            return count;
        }

        public void SaveSelection(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt64(this.Id);

            foreach (SMControl c in Objects)
            {
                SMRectangleArea area = GetArea(c.Id);
                if (area.Selected)
                {
                    bw.WriteByte(20);
                    bw.WriteString(ObjectTypeToTag(c.GetType()));
                    c.Save(bw);

                    bw.WriteByte(30);
                    bw.WriteInt64(c.Id);
                    area.Save(bw);
                }
            }

            bw.WriteByte(0);
        }

        public SMControl PasteSelection(RSFileReader br)
        {
            byte b;
            long areaId = -1;
            long pageId = -1;
            Dictionary<long, SMRectangleArea> tempAreas = new Dictionary<long, SMRectangleArea>();
            List<SMControl> tempControls = new List<SMControl>();
            SMControl returnedValue = null;


            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        pageId = br.ReadInt64();
                        break;
                    case 20:
                        object osc = TagToObject(br.ReadString());
                        if (osc is SMControl)
                        {
                            SMControl sc = (SMControl)osc;
                            sc.Load(br);
                            tempControls.Add(sc);
                        }
                        break;
                    case 30:
                        areaId = br.ReadInt64();
                        SMRectangleArea ra = new SMRectangleArea();
                        ra.Load(br);
                        tempAreas.Add(areaId, ra);
                        break;
                }
            }

            ClearSelection();

            double offsetX = (pageId == this.Id ? 32/1024.0 : 0);
            double offsetY = (pageId == this.Id ? 32 / 768.0 : 0);

            foreach (SMControl c in tempControls)
            {
                if (tempAreas.ContainsKey(c.Id))
                {
                    SMRectangleArea a = tempAreas[c.Id];
                    c.Id = Document.Data.GetNextId();
                    a.Selected = true;
                    a.TrackedSelection = SMControlSelection.All;

                    if (returnedValue == null)
                        returnedValue = c;

                    Objects.Add(c);
                    Areas.Add(c.Id, a);

                    if (pageId == this.Id)
                    {
                        a.MoveRaw(32 / 1024.0, 32 / 768.0);
                    }
                }
            }

            return returnedValue;
        }

        public void RecalcAreasForSelection(MNPageContext context)
        {
            foreach (SMRectangleArea area in Areas.Values)
            {
                if (area.Selected)
                {
                    area.RecalcAllBounds(context);
                }
            }
        }
    }
}
