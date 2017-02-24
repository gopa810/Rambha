using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.Serialization;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    [Serializable()]
    public class SMControl: GSCore
    {
        [Browsable(true), ReadOnly(true)]
        public long Id { get; set; }

        /// <summary>
        /// Parent page
        /// </summary>
        [Browsable(false)]
        public MNPage Page { get; set; }

        [Browsable(false)]
        public MNReferencedStyle Style 
        {
            get
            {
                if (p_style == null)
                    p_style = Document.FindStyle(p_style_lazy);
                return p_style;
            }
            set
            {
                p_style = value;
            }
        }
        private MNReferencedStyle p_style = null;
        private string p_style_lazy = "Default";

        [Browsable(true), Category("Format")]
        public MNFontName FontName { get; set; }

        [Browsable(true), Category("Format")]
        public float FontSize { get; set; }

        [Browsable(true), Category("Format")]
        public SMHorizontalAlign Align { get; set; }

        [Browsable(true), Category("Format")]
        public SMVerticalAlign VertAlign { get; set; }

        [Browsable(true), Category("Layout")]
        public bool Autosize { get; set; }

        [Browsable(true), ReadOnly(true)]
        public bool GroupControl { get; set; }

        [Browsable(false)]
        public string Tag { get; set; }

        [Browsable(true), Category("API"), DisplayName("API Name"), Description("Name of control for the script API. If empty then this control is not referenced from script.")]
        public string UniqueName { get { return p_unique_name.Value; } set { p_unique_name.Value = value; } }

        private GSString p_unique_name = new GSString("");

        [Browsable(false)]
        public string Text { get; set; }

        [Browsable(true), Category("Content"), DisplayName("Content Type")]
        public SMContentType ContentType { get; set; }

        [Browsable(true), Category("Content"), DisplayName("Content ID")]
        public string ContentId { get; set; }

        [Browsable(true), Category("Evaluation")]
        public MNEvaluationType Evaluation { get; set; }

        [Browsable(true), Category("Evaluation")]
        public string Hints { get; set; }

        [Browsable(true), Category("Behavior")]
        public bool Clickable { get; set; }

        [Browsable(true), Category("Behavior")]
        public SMDragResponse Draggable { get; set; }

        [Browsable(true), Category("Behavior")]
        public SMDropResponse Droppable { get; set; }



        public List<MNReferencedText> Scripts = new List<MNReferencedText>();

        public bool UIStatePressed = false;
        public bool UIStateHover = false;
        public MNEvaluationResult UIStateError = MNEvaluationResult.NotEvaluated;
        public bool UIStateShowHint = false;
        public bool UIStateEnabled = true;
        public bool UIStateVisible = true;

        public Size AutosizeSize = Size.Empty;
        public List<SMTokenItem> DroppedItems = new List<SMTokenItem>();
        public MNReferencedCore Content { get; set; }


        protected SolidBrush tempForeBrush = null;
        protected SolidBrush tempBackBrush = null;
        protected Pen tempBorderPen = null;
        protected Pen tempForePen = null;


        public SMControl(MNPage p): base()
        {
            Page = p;
            Style = null;
            Autosize = false;
            Text = "";
            UniqueName = "";
            Tag = "";
            GroupControl = false;
            ContentId = "";
            Evaluation = MNEvaluationType.None;
            Draggable = SMDragResponse.None;
            Clickable = false;
            Hints = "";
            ContentType = SMContentType.Undefined;
            FontName = MNFontName.Default;
            FontSize = 0;
            Align = SMHorizontalAlign.Undefined;
            VertAlign = SMVerticalAlign.Undefined;
        }

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "title":
                    return p_unique_name;
                default:
                    return base.GetPropertyValue(s);
            }
        }

        public void CopyContentTo(SMControl label)
        {
            // copy SMControl
            label.Id = Document.Data.GetNextId();
            label.Style = this.Style;
            label.FontName = this.FontName;
            label.FontSize = this.FontSize;
            label.Autosize = this.Autosize;
            label.GroupControl = this.GroupControl;
            label.Tag = this.Tag;
            label.UniqueName = this.UniqueName;
            label.Text = this.Text;
            label.ContentType = this.ContentType;
            label.ContentId = this.ContentId;
            label.Content = this.Content;
            label.Evaluation = this.Evaluation;
            label.Hints = this.Hints;
            label.Clickable = this.Clickable;
            label.Draggable = this.Draggable;
            label.Droppable = this.Droppable;
            foreach (MNReferencedText scr in this.Scripts)
            {
                MNReferencedText ns = new MNReferencedText();
                ns.Text = scr.Text;
                ns.Name = scr.Name;
                ns.Modified = scr.Modified;
                label.Scripts.Add(ns);
            }

        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            switch (token)
            {
                case "setEnabled":
                    UIStateEnabled = args.getSafe(0).getBooleanValue();
                    break;
                case "getEnabled":
                    return new GSBoolean(UIStateEnabled);
                case "setVisible":
                    UIStateVisible = args.getSafe(0).getBooleanValue();
                    break;
                case "setHidden":
                    UIStateVisible = !args.getSafe(0).getBooleanValue();
                    break;
                case "getVisible":
                    return new GSBoolean(UIStateVisible);
                case "set":
                    return ExecuteMessageSet(args[0], args[1], args);
            }
            return base.ExecuteMessage(token, args);
        }


        protected virtual GSCore ExecuteMessageSet(GSCore a1, GSCore a2, GSCoreCollection args)
        {
            switch (a1.getStringValue())
            {
                case "text":
                    Text = a2.getStringValue();
                    break;
                case "style":
                    Style = Document.FindStyle(a2.getStringValue());
                    if (Style == null) Style = Document.FindStyle("Default");
                    break;
                case "clickable":
                    Clickable = a2.getBooleanValue();
                    break;
                case "hints":
                    Hints = a2.getStringValue();
                    break;
                case "script":
                    {
                        string[] ppa = a2.getStringValue().Split(':');
                        if (ppa.Length == 2)
                        {
                            MNReferencedText rt = new MNReferencedText();
                            rt.Name = ppa[0];
                            rt.Text = ppa[1];
                            Scripts.Add(rt);
                        }
                    }
                    break;
            }

            return a2;
        }

        public override GSCore[] GetChildren()
        {
            return null;
        }

        public override string ToString()
        {
            return Text;
        }

        public SMVerticalAlign GetVerticalAlign()
        {
            return VertAlign == SMVerticalAlign.Undefined ? Style.VertAlign : VertAlign;
        }

        public SMHorizontalAlign GetHorizontalAlign()
        {
            return Align == SMHorizontalAlign.Undefined ? Style.Align : Align;
        }

        public Font GetUsedFont()
        {
            float fontSize = FontSize > 5 ? FontSize : Style.FontSize;
            MNFontName fontName = FontName != MNFontName.Default ? FontName : Style.FontName;
            return SMGraphics.GetFontVariation(fontName, fontSize);
        }

        public virtual void RecalculateSize(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            bounds.Size = AutosizeSize;

            area.BottomRuler.SetValue(context.DisplaySize, bounds.Bottom);
            area.RightRuler.SetValue(context.DisplaySize, bounds.Right);
        }

        [Browsable(false)]
        public MNDocument Document
        {
            get
            {
                return Page.Document;
            }
        }

        public int IndexOfDroppedItem(string text, string tag)
        {
            for (int i = 0; i < DroppedItems.Count; i++)
            {
                SMTokenItem si = DroppedItems[i];
                if (si.Text.Equals(text) && si.Tag.Equals(tag))
                {
                    return i;
                }            
            }
            return -1;
        }

        /// <summary>
        /// Fired when user click in the control area
        /// </summary>
        /// <param name="dc"></param>
        public virtual void OnTapBegin(PVDragContext dc)
        {
            if (Document.HasViewer)
                Document.Viewer.OnTapBegin(this, dc);
        }

        /// <summary>
        /// Fired when user moves finger
        /// This is received by start control
        /// </summary>
        /// <param name="dc"></param>
        public virtual void OnTapMove(PVDragContext dc)
        {
        }

        public virtual void OnTapCancel(PVDragContext dc)
        {
            if (Document.HasViewer)
                Document.Viewer.OnTapCancel(this, dc);
        }

        /// <summary>
        /// Fired when user releass finger
        /// </summary>
        /// <param name="dc"></param>
        public virtual void OnTapEnd(PVDragContext dc)
        {
            if (Document.HasViewer)
                Document.Viewer.OnTapEnd(this, dc);
        }

        /// <summary>
        /// Fired when user taps and releases finger from screen
        /// </summary>
        /// <param name="dc"></param>
        public virtual void OnClick(PVDragContext dc)
        {
            if (Document.HasViewer)
                Document.Viewer.OnClick(this, dc);
        }

        /// <summary>
        /// Fired when user releases drag operation in other control
        /// This is fired in target control
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        public virtual bool OnDropFinished(PVDragContext dc)
        {
            if (!Document.HasViewer || Document.Viewer.OnDropWillFinish(this, dc))
            {
                SMTokenItem item = dc.draggedItem;
                if (item == null)
                {
                    Debugger.Log(0, "", "Dragged item is NULL\n");
                    return false;
                }
                foreach (SMTokenItem si in DroppedItems)
                {
                    if (si.Text.Equals(item.Text) && si.Tag.Equals(item.Tag))
                    {
                        return false;
                    }
                }
                DroppedItems.Add(item);

                SMDragResponse resp = dc.startControl.Draggable;
                if (resp == SMDragResponse.Drag || resp == SMDragResponse.Line)
                {
                    if (this.Droppable == SMDropResponse.One)
                        Page.RemoveConnectionWithTarget(this);
                    SMConnection conn = new SMConnection(this.Page);
                    conn.Source = dc.startControl;
                    conn.Target = this;
                    conn.ConnectionStyle = (resp == SMDragResponse.Line ? SMConnectionStyle.DirectLine : SMConnectionStyle.Invisible);
                    Page.AddConnection(conn);
                }

                if (Document.HasViewer)
                    Document.Viewer.OnDropDidFinish(this, dc);
            }
            return true;
        }

        /// <summary>
        /// Fired when user releases drag operation
        /// this is fired in source control
        /// </summary>
        /// <param name="item"></param>
        public virtual void OnDragFinished(PVDragContext context)
        {
            if (Document.HasViewer)
                Document.Viewer.OnDragFinished(this, context.draggedItem, context);
        }
        public virtual void OnDragStarted(PVDragContext context)
        {
            if (Document.HasViewer)
                Document.Viewer.OnDragStarted(this, context.draggedItem, context);
        }
        public virtual void OnDragMove(PVDragContext context)
        {
            if (Document.HasViewer)
                Document.Viewer.OnDragMove(this, context.draggedItem, context);
        }

        /// <summary>
        /// Fired when finger entered area of some control
        /// that control receives this message once at the beginning
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool OnDragHotTrackStarted(SMTokenItem item, PVDragContext context)
        {
            if (this.Droppable == SMDropResponse.One || this.Droppable == SMDropResponse.Many)
                UIStateHover = true;
            if (Document.HasViewer)
                Document.Viewer.OnDragHotTrackStarted(item, context);
            return UIStateHover;
        }

        /// <summary>
        /// Fired when finger leaves area of some control
        /// that control receives this message once
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool OnDragHotTrackEnded(SMTokenItem item, PVDragContext context)
        {
            UIStateHover = false;
            if (Document.HasViewer)
                Document.Viewer.OnDragHotTrackEnded(item, context);
            return UIStateHover;
        }

        /// <summary>
        /// Fired when finger is moved over some control
        /// recipient is control over which is finger dragged
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnDropMove(PVDragContext context)
        {
            if (Document.HasViewer)
                Document.Viewer.OnDropMove(this, context);
        }

        public virtual void Save(RSFileWriter bw)
        {
            bw.WriteByte(254);
            bw.WriteInt64(Id);
            bw.WriteString(Style != null ? Style.Name : "Default");
            bw.WriteString(Text != null ? Text : "");

            bw.WriteByte(253);
            bw.WriteBool(Autosize);
            bw.WriteBool(GroupControl);
            bw.WriteString(Tag);
            bw.WriteString(ContentId);
            bw.WriteString(UniqueName);
            bw.WriteInt32((Int32)Evaluation);
            bw.WriteString(Hints);

            bw.WriteByte(252);
            bw.WriteBool(Clickable);
            bw.WriteInt32((Int32)Draggable);
            bw.WriteInt32((Int32)Droppable);

            foreach (MNReferencedText rt in Scripts)
            {
                bw.WriteByte(251);
                rt.Save(bw);
            }

            bw.WriteByte(250);
            bw.WriteInt32((Int32)ContentType);

            bw.WriteByte(249);
            bw.WriteInt32((Int16)FontName);

            bw.WriteByte(248);
            bw.WriteFloat(FontSize);

            bw.WriteByte(247);
            bw.WriteInt32((Int32)Align);

            bw.WriteByte(246);
            bw.WriteInt32((Int32)VertAlign);

            // end-of-object
            bw.WriteByte(0);
        }

        public virtual bool Load(RSFileReader br)
        {
            byte tag;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 254:
                        Id = br.ReadInt64();
                        p_style_lazy = br.ReadString();
                        Text = br.ReadString();
                        break;
                    case 253:
                        Autosize = br.ReadBool();
                        GroupControl = br.ReadBool();
                        Tag = br.ReadString();
                        ContentId = br.ReadString();
                        UniqueName = br.ReadString();
                        Evaluation = (MNEvaluationType)br.ReadInt32();
                        Hints = br.ReadString();
                        break;
                    case 252:
                        Clickable = br.ReadBool();
                        Draggable = (SMDragResponse)br.ReadInt32();
                        Droppable = (SMDropResponse)br.ReadInt32();
                        break;
                    case 251:
                        MNReferencedText rt = new MNReferencedText();
                        rt.Load(br);
                        Scripts.Add(rt);
                        break;
                    case 250:
                        ContentType = (SMContentType)br.ReadInt32();
                        break;
                    case 249:
                        FontName = (MNFontName)br.ReadInt32();
                        break;
                    case 248:
                        FontSize = br.ReadFloat();
                        break;
                    case 247:
                        Align = (SMHorizontalAlign)br.ReadInt32();
                        break;
                    case 246:
                        VertAlign = (SMVerticalAlign)br.ReadInt32();
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public virtual void Paint(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(this.Id);
            if (area.Selected && context.drawSelectionMarks)
            {
                for (int i = 1; i < 9; i++)
                    context.g.FillRectangle(Brushes.DarkBlue, area.LastBounds[i]);
            }
            Brush bb = null;
            switch (UIStateError)
            {
                case MNEvaluationResult.Correct:
                    bb = Brushes.Green;
                    break;
                case MNEvaluationResult.Incorrect:
                    bb = Brushes.Red;
                    break;
                case MNEvaluationResult.Focused:
                    bb = Brushes.LightBlue;
                    break;
            }
            if (bb != null)
            {
                context.g.FillRectangle(bb, area.LastBounds[0].X - 3, area.LastBounds[0].Y - 3, 3, area.LastBounds[0].Height + 6);
                context.g.FillRectangle(bb, area.LastBounds[0].X, area.LastBounds[0].Y - 3, area.LastBounds[0].Width + 3, 3);
                context.g.FillRectangle(bb, area.LastBounds[0].X, area.LastBounds[0].Bottom, area.LastBounds[0].Width + 3, 3);
                context.g.FillRectangle(bb, area.LastBounds[0].Right, area.LastBounds[0].Y, 3, area.LastBounds[0].Height);
            }
        }


        public virtual Size GetDefaultSize()
        {
            return new Size(128,48);
        }

        /// <summary>
        /// Evaluates if current content is correct according evaluation criteria.
        /// This method is invoked in case of lazy evaluation.
        /// </summary>
        /// <returns></returns>
        public virtual MNEvaluationResult Evaluate()
        {
            return UIStateError;
        }

        protected void PrepareBrushesAndPens()
        {
            if (tempBackBrush == null || tempBackBrush.Color != (UIStatePressed | UIStateHover ? Style.HighBackColor : Style.BackColor))
            {
                tempBackBrush = new SolidBrush(UIStatePressed | UIStateHover ? Style.HighBackColor : Style.BackColor);
            }
            if (tempForeBrush == null || tempForeBrush.Color != (UIStatePressed | UIStateHover ? Style.HighForeColor : Style.ForeColor))
            {
                tempForeBrush = new SolidBrush(UIStatePressed | UIStateHover ? Style.HighForeColor : Style.ForeColor);
            }
            if (tempForePen == null || tempForePen.Color != (UIStatePressed | UIStateHover ? Style.HighForeColor : Style.ForeColor))
            {
                tempForePen = new Pen(UIStatePressed | UIStateHover ? Style.HighForeColor : Style.ForeColor);
            }
            if (tempBorderPen == null || tempBorderPen.Color != (UIStatePressed | UIStateHover ? Style.HighBorderColor : Style.BorderColor))
            {
                tempBorderPen = new Pen(UIStatePressed | UIStateHover ? Style.HighBorderColor : Style.BorderColor, UIStatePressed | UIStateHover ? Style.HighBorderWidth : Style.BorderWidth);
            }
        }

        protected void DrawStyledBorder(MNPageContext context, Rectangle bounds)
        {
            switch (UIStatePressed | UIStateHover ? Style.HighBorderStyle : Style.BorderStyle)
            {
                case SMBorderStyle.Rectangle:
                    context.g.FillRectangle(tempBackBrush, bounds);
                    context.g.DrawRectangle(tempBorderPen, bounds);
                    break;
                case SMBorderStyle.RoundRectangle:
                    context.g.DrawFillRoundedRectangle(tempBorderPen, tempBackBrush, bounds, Style.CornerRadius);
                    break;
                case SMBorderStyle.Elipse:
                    context.g.FillEllipse(tempBackBrush, bounds);
                    context.g.DrawEllipse(tempBorderPen, bounds);
                    break;
                default:
                    if (Style.BackColor != Color.Transparent)
                    {
                        context.g.FillRectangle(tempBackBrush, bounds);
                    }
                    break;
            }
        }

        public virtual SMTokenItem GetDraggableItem(Point point)
        {
            SMTokenItem item = new SMTokenItem();
            item.Tag = Tag;
            item.Text = Text;
            item.ContentSize = Size.Empty;
            return item;
        }

        public virtual bool HasImmediateEvaluation
        {
            get
            {
                switch (Evaluation)
                {
                    case MNEvaluationType.Immediate: return true;
                    case MNEvaluationType.Inherited: return Page.HasImmediateEvaluation;
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
                    case MNEvaluationType.Inherited: return Page.HasLazyEvaluation;
                    default: return false;
                }
            }
        }

        public virtual void DisplayAnswers()
        {
        }

        public bool HasEvaluation
        {
            get
            {
                return HasLazyEvaluation || HasImmediateEvaluation;
            }
        }

        public bool ContainsScript(string s)
        {
            foreach (MNReferencedText rt in Scripts)
                if (rt.Name.Equals(s))
                    return true;
            return false;
        }

        public MNReferencedText FindScript(string s)
        {
            foreach (MNReferencedText rt in Scripts)
                if (rt.Name.Equals(s))
                    return rt;
            return null;
        }

        public void PostExecuteEvent(string scriptName)
        {
            if (Document.Viewer != null && ContainsScript(scriptName))
                Document.Viewer.AddNextScript(FindScript(scriptName).Text);
        }

        public virtual void LoadStatus(RSFileReader br)
        {
            DroppedItems.Clear();
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        UIStateEnabled = br.ReadBool();
                        break;
                    case 13:
                        UIStateShowHint = br.ReadBool();
                        break;
                    case 14:
                        UIStateVisible = br.ReadBool();
                        break;
                    case 15:
                        SMTokenItem ti = new SMTokenItem();
                        ti.Load(br);
                        DroppedItems.Add(ti);
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void SaveStatus(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteBool(UIStateEnabled);

            bw.WriteByte(13);
            bw.WriteBool(UIStateShowHint);

            bw.WriteByte(14);
            bw.WriteBool(UIStateVisible);

            foreach (SMTokenItem wt in DroppedItems)
            {
                bw.WriteByte(15);
                wt.Save(bw);
            }

            bw.WriteByte(0);
        }

        public virtual SMControl Duplicate()
        {
            return null;
        }
    }

}
