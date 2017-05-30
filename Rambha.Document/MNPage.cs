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

        [Browsable(true), DisplayName("Next Page Title"), Category("API")]
        public string NextPage { get; set; }

        [Browsable(false)]
        public bool ShowMessageAlways { get; set; }

        [Browsable(false)]
        public string MessageTitle { get; set; }

        [Browsable(false)]
        public string MessageText { get; set; }

        [Browsable(false)]
        public string TextB { get; set; }

        [Browsable(false)]
        public string TextC { get; set; }

        [Browsable(true)]
        public bool ShowBackNavigation { get; set; }

        [Browsable(true)]
        public bool ShowForwardNavigation { get; set; }

        [Browsable(true)]
        public bool ShowHome { get; set; }
        
        [Browsable(true)]
        public bool ShowHelp { get; set; }

        [Browsable(true)]
        public bool ShowTitle { get; set; }

        [Browsable(true)]
        public bool DefaultAudioState { get; set; }

        [Browsable(true)]
        public bool ShowAudio { get; set; }

        private Color backgroundColor = Color.White;
        private Brush nontransparentBackgroundBrush = null;
        private Brush semitransparentBackgroundBrush = null;

        public byte[] InitialStatus = null;

        public const int HEADER_HEIGHT = 64;

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

        public List<SMConnection> Connections = new List<SMConnection>();

        public SMRectangleArea Area = new SMRectangleArea();

        // not stored
        //

        public MNPage Template 
        {
            get 
            {
                if (p_template == null)
                {
                    if (p_templateName_lazy.Length > 0)
                    {
                        p_template = Document.FindTemplateName(p_templateName_lazy);
                        p_templateName_lazy = "";
                    }
                    else if (p_template_lazy > 0)
                    {
                        p_template = Document.FindTemplateId(p_template_lazy);
                        p_template_lazy = -1;
                    }
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
        [Browsable(false)]
        public string TemplateName
        {
            get { MNPage t = Template; return (t != null ? t.Title : p_templateName_lazy); }
            set { p_templateName_lazy = value; }
        }
        private MNPage p_template = null;
        private long p_template_lazy = -1;
        private string p_templateName_lazy = "";


        public int ItemHeight = 0;
        public int ItemTextHeight = 0;
        public int Index = 0;

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "title":
                    return p_title_obj;
                case "messageTitle":
                    return new GSString(MessageTitle);
                case "messageText":
                case "textA":
                    return new GSString(MessageText);
                case "textB":
                    return new GSString(TextB);
                case "textC":
                    return new GSString(TextC);
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
                case "messageText":
                    return new GSString(MessageText);
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

            bw.WriteByte(17);
            bw.WriteString(NextPage);

            bw.WriteByte(18);
            bw.WriteString(MessageText);

            bw.WriteByte(19);
            bw.WriteString(TextB);

            bw.WriteByte(20);
            bw.WriteString(TextC);

            bw.WriteByte(21);
            bw.WriteString(TemplateName);

            bw.WriteByte(22);
            bw.WriteBool(ShowTitle);

            bw.WriteByte(23);
            bw.WriteBool(ShowBackNavigation);

            bw.WriteByte(24);
            bw.WriteBool(ShowHelp);

            bw.WriteByte(25);
            bw.WriteBool(ShowHome);

            bw.WriteByte(26);
            bw.WriteBool(ShowForwardNavigation);

            bw.WriteByte(27);
            bw.WriteString(MessageTitle);

            bw.WriteByte(28);
            bw.WriteBool(ShowMessageAlways);

            bw.WriteByte(29);
            bw.WriteBool(DefaultAudioState);

            bw.WriteByte(30);
            bw.WriteBool(ShowAudio);

            // end of object
            bw.WriteByte(0);
        }

        public bool Load(RSFileReader br)
        {
            byte tag;
            int c;
            ShowTitle = true;
            ShowHome = true;
            ShowHelp = false;
            ShowBackNavigation = true;
            ShowForwardNavigation = true;
            bool showAudioLoaded = false;

            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        Id = br.ReadInt64();
                        if (Id == 0)
                        {
                            Id = Document.Data.GetNextId();
                        }
                        Title = br.ReadString();
                        Description = br.ReadString();
                        IsTemplate = br.ReadBool();
                        TemplateId = br.ReadInt64();
                        TemplateId = -1;
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
                        for (int i = 0; i < c; i++)
                        {
                            long key = br.ReadInt64();
                            SMControl sa = FindObject(key);
                            if (sa != null)
                            {
                                sa.Area.Load(br);
                            }
                            else
                            {
                                // throw away area with unknown ID
                                SMRectangleArea ar = new SMRectangleArea();
                                ar.Load(br);
                                ar = null;
                            }
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
                    case 17:
                        NextPage = br.ReadString();
                        break;
                    case 18:
                        MessageText = br.ReadString();
                        if (MessageText.Length > 0)
                            ShowHelp = true;
                        break;
                    case 19:
                        TextB = br.ReadString();
                        break;
                    case 20:
                        TextC = br.ReadString();
                        break;
                    case 21:
                        TemplateName = br.ReadString();
                        TemplateName = "";
                        break;
                    case 22: ShowTitle = br.ReadBool(); break;
                    case 23: ShowBackNavigation = br.ReadBool(); break;
                    case 24: ShowHelp = br.ReadBool(); break;
                    case 25: ShowHome = br.ReadBool(); break;
                    case 26: ShowForwardNavigation = br.ReadBool(); break;
                    case 27:
                        MessageTitle = br.ReadString();
                        break;
                    case 28: ShowMessageAlways = br.ReadBool(); break;
                    case 29:
                        DefaultAudioState = br.ReadBool();
                        break;
                    case 30:
                        ShowAudio = br.ReadBool();
                        showAudioLoaded = true;
                        break;
                    default:
                        break;
                }
            }


            if (!showAudioLoaded)
            {
                foreach (SMControl s in Objects)
                {
                    if (s.ContentId.Length > 0 && s.ContentType == SMContentType.Audio)
                    {
                        ShowAudio = !(s.ContentId.Equals("Title"));
                        DefaultAudioState = (s.ContentId.Equals("Title"));
                        showAudioLoaded = true;
                        break;
                    }
                }

                if (!showAudioLoaded)
                {
                    ShowAudio = false;
                }
            }


            return true;
        }

        public MNPage(MNDocument doc): base()
        {
            Document = doc;
            IsTemplate = false;
            BackgroundColor = Color.White;
            NextPage = string.Empty;
            MessageText = "";
            MessageTitle = "Notes for parents and teachers";
            TextB = "";
            TextC = "";
            Evaluation = MNEvaluationType.Inherited;
            ShowTitle = true;
            ShowHome = true;
            ShowHelp = false;
            ShowBackNavigation = true;
            ShowForwardNavigation = true;
            ShowMessageAlways = false;
            DefaultAudioState = false;
            ShowAudio = false;
        }

        public override string ToString()
        {
            return Title;
        }

        public bool HasSelectedObjects()
        {
            foreach (SMControl s in Objects)
            {
                if (s.Area.Selected) return true;
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
                    if (item.Area.Selected)
                        lc.Add(item);
                }
                return lc;
            }
        }

        public void ClearSelection()
        {
            foreach (SMControl s in Objects)
            {
                s.Area.Selected = false;
            }

            if (Template != null)
                Template.ClearSelection();
        }

        public void DeleteSelectedObjects()
        {
            List<SMControl> objectsForDelete = new List<SMControl>();
            HashSet<SMControl> selectedParentObjects = new HashSet<SMControl>();

            foreach (SMControl item in Objects)
            {
                if (item.Area.Selected)
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



        public void DuplicateSelectedObjects(int rows, int columns, bool hz, bool vt, int spacing)
        {
            int i, t;
            if (hz || vt)
                spacing = Math.Max(spacing, 10);

            List<SMControl> selectedObjects = SelectedObjects;

            Rectangle totalRect = Rectangle.Empty;

            // get relative coordinates of selection
            for (i = 0; i < selectedObjects.Count; i++)
            {
                SMControl item = selectedObjects[i];
                Rectangle rcArea = item.Area.GetRawRectangle(PageEditDisplaySize.LandscapeBig);
                if (totalRect.IsEmpty)
                {
                    totalRect = rcArea;
                }
                else
                {
                    MergeRectangles(ref totalRect, rcArea);
                }
            }

            if (totalRect.IsEmpty)
                return;

            Point offset = Point.Empty;

            if (hz)
            {
                for (int r = 1; r < rows; r++)
                {
                    Rectangle newRect = new Rectangle(totalRect.Left, totalRect.Top + r * (totalRect.Height + spacing) - spacing,
                        columns * (totalRect.Width + spacing) - spacing, spacing);

                    SMDrawable d = new SMDrawable(this);
                    d.Drawings = "line 0 50 100 50";
                    d.Id = Document.Data.GetNextId();
                    d.Area.SetRawRectangle(PageEditDisplaySize.LandscapeBig, newRect);
                    d.Area.Selected = true;
                    Objects.Add(d);
                }
            }

            if (vt)
            {
                for (int c = 1; c < columns; c++)
                {
                    Rectangle newRect = new Rectangle(totalRect.Left + c * (totalRect.Width + spacing) - spacing, totalRect.Top,
                        spacing, rows * (totalRect.Height + spacing ) - spacing);

                    SMDrawable d = new SMDrawable(this);
                    d.Drawings = "line 50 0 50 100";
                    d.Id = Document.Data.GetNextId();
                    d.Area.SetRawRectangle(PageEditDisplaySize.LandscapeBig, newRect);
                    d.Area.Selected = true;
                    Objects.Add(d);
                }
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (r == 0 && c == 0)
                        continue;
                    offset.X = c * (totalRect.Width + spacing);
                    offset.Y = r * (totalRect.Height + spacing);
                    foreach (SMControl item in selectedObjects)
                    {
                        SMControl duplicated = item.Duplicate();
                        if (duplicated != null)
                        {
                            duplicated.Area.MoveRaw(offset.X, offset.Y);
                            duplicated.Area.Selected = true;

                            Objects.Add(duplicated);
                        }
                    }
                }
            }
        }

        public static void MergeRectangles(ref Rectangle totalRect, Rectangle rcArea)
        {
            if (totalRect.IsEmpty)
            {
                totalRect = rcArea;
            }
            else
            {
                Rectangle r = new Rectangle();
                r.X = Math.Min(totalRect.X, rcArea.X);
                r.Y = Math.Min(totalRect.Y, rcArea.Y);
                r.Width = Math.Max(totalRect.Right, rcArea.Right);
                r.Height = Math.Max(totalRect.Bottom, rcArea.Bottom);
                r.Width = r.Width - r.X;
                r.Height = r.Height - r.Y;
                totalRect = r;
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

        public void PaintBackground(MNPageContext context)
        {
            context.g.FillRectangle(nontransparentBackgroundBrush, 0, 0, context.PageWidth, context.PageHeight);
        }

        public void Paint(MNPageContext context, bool topControls)
        {
            //Debugger.Log(0, "", "\n\n\n");
            try
            {
                if (Template != null && Template != this)
                    Template.Paint(context, topControls);

                Graphics g = context.g;

                // draw connections
                if (!topControls)
                {
                    foreach (SMConnection connection in this.Connections)
                    {
                        connection.Paint(context);
                    }
                }

                // draw objects
                foreach (SMControl po in this.Objects)
                {
                    if (!po.UIStateVisible || po.AlwaysOnTop != topControls)
                        continue;

                    SMRectangleArea area = po.Area;
                    Rectangle rect = area.RelativeArea;

                    /*if (area.Selected && context.drawSelectionMarks)
                        area.PaintSelectionMarks(context);*/

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
                        g.DrawRectangle((area.Selected ? Pens.Turquoise : Pens.LightGray), rect);
                    }

                    if (!po.UIStateEnabled)
                    {
                        g.FillRectangle(semitransparentBackgroundBrush, rect);
                    }
                }

                if (topControls)
                {
                    int height = HEADER_HEIGHT;
                    int leftX = 0;
                    int rightX = context.PageWidth;
                    if (ShowBackNavigation)
                    {
                        g.FillRectangle((context.hitHeaderButton == 1 ? Brushes.Blue : Brushes.Black), leftX, 0, height, height);
                        if (context.navigIconBack != null)
                            g.DrawImage(context.navigIconBack, leftX, 0, height, height);
                        if (context.navigArrowBack != null)
                            g.DrawImage(context.navigArrowBack, 0, context.PageHeight / 2 - 100, 80, 200); 
                        leftX += height;
                    }

                    if (ShowForwardNavigation)
                    {
                        rightX -= height;
                        g.FillRectangle((context.hitHeaderButton == 4 ? Brushes.Blue : Brushes.Black), rightX, 0, height, height);
                        if (context.navigIconFwd != null)
                            g.DrawImage(context.navigIconFwd, rightX, 0, height, height);
                        if (context.navigArrowFwd != null)
                            g.DrawImage(context.navigArrowFwd, context.PageWidth - 80, context.PageHeight / 2 - 100, 80, 200);
                    }

                    if (ShowHome)
                    {
                        g.FillRectangle((context.hitHeaderButton == 2 ? Brushes.Blue : Brushes.Black), leftX, 0, height, height);
                        if (context.navigIconMenu != null)
                            g.DrawImage(context.navigIconMenu, leftX, 0, height, height);
                        leftX += height;
                    }

                    if (ShowHelp && !ShowMessageAlways)
                    {
                        rightX -= height;
                        g.FillRectangle((context.hitHeaderButton == 3 ? Brushes.Blue : Brushes.Black), rightX, 0, height, height);
                        if (context.navigIconHelp != null)
                            g.DrawImage(context.navigIconHelp, rightX, 0, height, height);
                    }

                    if (ShowAudio && context.navigSpeakerOn != null && context.navigSpeakerOff != null)
                    {
                        rightX -= height;
                        g.FillRectangle((context.hitHeaderButton == 5 ? Brushes.Blue : Brushes.Black), rightX, 0, height, height);
                        g.DrawImage(MNNotificationCenter.AudioOn ? context.navigSpeakerOn : context.navigSpeakerOff, rightX, 0, height, height);
                    }

                    if (ShowTitle)
                    {
                        g.FillRectangle(Brushes.Black, leftX, 0, rightX - leftX, height);
                        g.DrawString(TextB, context.PageTitleFont, Brushes.White, leftX + 16, 16);
                    }

                }
            }
            catch (Exception ex)
            {
                Debugger.Log(0,"",ex.StackTrace);
                Debugger.Log(0, "", "\n\n");
            }
        }

        public void ClearConnections()
        {
            Connections.Clear();
        }

        public SMConnection FindConnection(SMControl control)
        {
            foreach (SMConnection sc in Connections)
            {
                if (sc.Source.Id == control.Id || sc.Target.Id == control.Id)
                {
                    return sc;
                }
            }
            return null;
        }

        public SMConnection FindConnection(SMControl from, SMControl to)
        {
            foreach (SMConnection sc in Connections)
            {
                if ((sc.Source.Id == from.Id && sc.Target.Id == to.Id)
                    || (sc.Source == to && sc.Target == from))
                {
                    return sc;
                }
            }
            return null;
        }

        public void AddConnection(SMConnection conn)
        {
            if (FindConnection(conn.Source, conn.Target) == null)
                Connections.Add(conn);
        }

        public void AddConnection(SMControl from, SMControl to)
        {
            if (FindConnection(from, to) == null)
            {
                Connections.Add(new SMConnection(this) { Source = from, Target = to });
            }
        }

        public void RemoveConnectionsForControl(SMControl target)
        {
            List<SMConnection> listToRemove = new List<SMConnection>();
            foreach (SMConnection conn in Connections)
            {
                if (conn.Target == target || conn.Source == target)
                    listToRemove.Add(conn);
            }
            foreach (SMConnection conn in listToRemove)
            {
                Connections.Remove(conn);
            }
        }

        public int CountConnectionsWithControl(SMControl source)
        {
            int count = 0;
            foreach (SMConnection conn in Connections)
            {
                if (conn.Source == source || conn.Target == source)
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
            if (a == typeof(SMImageButton)) return "ImageButton";
            if (a == typeof(SMLabel)) return "Label";
            if (a == typeof(SMLetterInput)) return "LetterInput";
            if (a == typeof(SMTextContainer)) return "TextContainer";
            if (a == typeof(SMTextEdit)) return "TextEdit";
            if (a == typeof(SMTextPuzzle)) return "TextPuzzle";
            if (a == typeof(SMTextView)) return "TextView";
            if (a == typeof(SMKeyboard)) return "Keyboard";
            if (a == typeof(SMMemoryGame)) return "MemoryGame";
            if (a == typeof(SMOrderedList)) return "OrderedList";
            if (a == typeof(SMSelection)) return "Selection";

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
                case "ImageButton": return new SMImageButton(this);
                case "Label": return new SMLabel(this);
                case "LetterInput": return new SMLetterInput(this);
                case "TextContainer": return new SMTextContainer(this);
                case "TextEdit": return new SMTextEdit(this);
                case "TextPuzzle": return new SMTextPuzzle(this);
                case "TextView": return new SMTextView(this);
                case "MemoryGame": return new SMMemoryGame(this);
                case "Keyboard": return new SMKeyboard(this);
                case "Matrix": return new SMOrderedList(this);
                case "OrderedList": return new SMOrderedList(this);
                case "Selection": return new SMSelection(this);
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


        public void StoreStatus()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    RSFileWriter fw = new RSFileWriter(bw);

                    SaveStatus(fw);

                    InitialStatus = ms.GetBuffer();
                }
            }
        }

        public void RestoreStatus()
        {
            if (InitialStatus == null)
                return;

            using (MemoryStream ms = new MemoryStream(InitialStatus))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    RSFileReader fr = new RSFileReader(br);
                    LoadStatus(fr);
                }
            }
        }

        public int GetSelectedCount()
        {
            int count = 0;
            foreach(SMControl c in Objects)
                if (c.Area.Selected) count++;
            return count;
        }

        public void SaveSelection(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt64(this.Id);

            foreach (SMControl c in Objects)
            {
                if (c.Area.Selected)
                {
                    bw.WriteByte(20);
                    bw.WriteString(ObjectTypeToTag(c.GetType()));
                    c.Save(bw);
                }
            }

            bw.WriteByte(0);
        }

        public SMControl PasteSelection(RSFileReader br)
        {
            byte b;
            long areaId = -1;
            long pageId = -1;
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
                }
            }

            ClearSelection();

            double offsetX = (pageId == this.Id ? 32 / 1024.0 : 0);
            double offsetY = (pageId == this.Id ? 32 / 768.0 : 0);

            foreach (SMControl c in tempControls)
            {
                c.Id = Document.Data.GetNextId();
                c.Area.Selected = true;

                if (returnedValue == null)
                    returnedValue = c;

                Objects.Add(c);

                if (pageId == this.Id)
                {
                    c.Area.MoveRaw(32, 32);
                }
            }

            return returnedValue;
        }

        public void RecalcAreasForSelection(MNPageContext context)
        {
            foreach (SMControl c in Objects)
            {
                if (c.Area.Selected)
                {
                    c.Area.RecalcAllBounds(context);
                }
            }
        }

        public Rectangle GetTotalSelectionRect()
        {
            Rectangle tr = Rectangle.Empty;

            foreach (SMControl po in SelectedObjects)
            {
                MergeRectangles(ref tr, po.Area.GetRawRectangle(PageEditDisplaySize.LandscapeBig));
            }

            return tr;
        }

        public bool HasSelectedObject()
        {
            foreach (SMControl c in Objects)
            {
                SMRectangleArea ar = c.Area;
                if (ar.Selected)
                    return true;
            }

            return false;
        }

        public SMControl FindObjectContainingPoint(MNPageContext context, Point logPoint)
        {
            SMControl c = FindObjectContainingPoint(context, logPoint, true);
            if (c == null)
                c = FindObjectContainingPoint(context, logPoint, false);
            return c;
        }

        public SMControl FindObjectContainingPoint(MNPageContext context, Point logPoint, bool topControls)
        {
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                SMControl po = Objects[i];
                if (po.AlwaysOnTop == topControls)
                {
                    if (po.Area.TestHitLogical(context, logPoint))
                    {
                        return po;
                    }
                }
            }

            /*if (Template != null)
            {
                return Template.FindObjectContainingPoint(context, logPoint, topControls);
            }*/

            return null;
        }

        public void LimitGroupChecked(string GroupName, int p)
        {
            if (GroupName.Length == 0)
                return;

            int count = 0;
            foreach (SMControl sc in Objects)
            {
                if (sc.GroupName.Equals(GroupName))
                    if (sc.UIStateChecked)
                        count++;
                if (count > p)
                {
                    Debugger.Log(0, "", "C\n");
                    sc.UIStateChecked = false;
                }
            }
        }

        public List<SMControl> GetGroupControls(string groupName)
        {
            List<SMControl> cl = new List<SMControl>();
            foreach (SMControl sc in Objects)
            {
                if (sc.GroupName.Equals(groupName))
                    cl.Add(sc);
            }
            return cl;
        }

        public List<SMControl> GetGroupControlsChecked(string groupName)
        {
            List<SMControl> cl = new List<SMControl>();
            foreach (SMControl sc in Objects)
            {
                if (sc.GroupName.Equals(groupName) && sc.UIStateChecked)
                    cl.Add(sc);
            }
            return cl;
        }

        public List<SMControl> GetGroupControlsExcept(string groupName, SMControl s)
        {
            List<SMControl> cl = new List<SMControl>();
            foreach (SMControl sc in Objects)
            {
                if (sc.GroupName.Equals(groupName) && sc != s)
                    cl.Add(sc);
            }
            return cl;
        }

        public List<SMControl> GetGroupControlsCheckedExcept(string groupName, SMControl s)
        {
            List<SMControl> cl = new List<SMControl>();
            foreach (SMControl sc in Objects)
            {
                if (sc.GroupName.Equals(groupName) && sc != s && sc.UIStateChecked)
                    cl.Add(sc);
            }
            return cl;
        }

        public int CountGroupChecked(string GroupName)
        {
            int count = 0;
            foreach (SMControl sc in Objects)
            {
                if (sc.GroupName.Equals(GroupName))
                    if (sc.UIStateChecked)
                        count++;
            }
            return count;
        }

        public static void CopyControlsFrom(MNPage sourcePage, MNPage destinationPage)
        {
            byte[] buffer;
            foreach (SMControl ctrl in sourcePage.Objects)
            {
                buffer = ctrl.GetBytes();
                SMControl new_control = SMControl.FromBytes(destinationPage, buffer);
                new_control.Id = destinationPage.Document.Data.GetNextId();
                destinationPage.Objects.Add(new_control);
            }
        }


        public SMControl FindObjectWithAPIName(string controlApiName)
        {
            foreach (SMControl s in Objects)
            {
                if (s.UniqueName.Equals(controlApiName))
                    return s;
            }

            return null;
        }


        public bool HasControlsWithHints 
        {
            get
            {
                foreach (SMControl s in Objects)
                {
                    if (s.Hints.Length > 0)
                        return true;
                }
                return false;
            }
        }

        public bool HasControlsWithSpots
        {
            get
            {
                foreach (SMControl s in Objects)
                {
                    if (s is SMImage)
                    {
                        SMImage si = s as SMImage;
                        MNReferencedImage ri = (si != null && si.Img != null) ? si.Img.Image : null;
                        if (ri != null)
                        {
                            if (ri.HasSpots())
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool HasGroup(string groupName)
        {
            foreach (SMControl c in Objects)
            {
                if (c.GroupName.Equals(groupName))
                {
                    return true;
                }
            }

            return false;
        }

        public void ResetStatus()
        {
            Connections.Clear();
            foreach (SMControl c in Objects)
            {
                c.ResetStatus();
            }
        }
    }
}
