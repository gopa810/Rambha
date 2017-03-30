﻿using System;
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

        [Browsable(true), Category("Editing")]
        public bool Selectable { get; set; }

        /// <summary>
        /// Parent page
        /// </summary>
        [Browsable(false)]
        public MNPage Page { get; set; }

        [Browsable(true), Category("Format")]
        public string StyleName { get { return p_style_name; } set { p_style_name = value;} }
        private string p_style_name = "Default";

        [Browsable(true), Category("Format")]
        public SMFont Font { get; set; }

        [Browsable(true), Category("Format")]
        public SMParaFormat Paragraph { get; set; }

        [Browsable(true), Category("Format")]
        public SMStatusLayout NormalState { get; set; }

        [Browsable(true), Category("Format")]
        public SMStatusLayout HighlightState { get; set; }

        [Browsable(true), DisplayName("Padding"), Category("Format")]
        public SMContentPadding ContentPadding { get; set; }


        [Browsable(true), Category("Layout")]
        public bool Autosize { get; set; }

        [Browsable(false)]
        public bool GroupControl { get; set; }

        [Browsable(true), Category("Management")]
        public string GroupName { get; set; }

        [Browsable(false)]
        public string Tag { get; set; }

        [Browsable(true), Category("API"), DisplayName("API Name"), Description("Name of control for the script API. If empty then this control is not referenced from script.")]
        public string UniqueName { get { return p_unique_name.Value; } set { p_unique_name.Value = value; } }

        private GSString p_unique_name = new GSString("");

        [Browsable(false)]
        public string Text { get { return p_text; } set { p_text = value; TextDidChange(); } }
        private string p_text = String.Empty;

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
        public SMConnectionCardinality Cardinality { get; set; }

        [Browsable(true), Category("Behavior")]
        public Bool3 ExpectedChecked { get; set; }

        [Browsable(true), Category("Behavior")]
        public Bool3 DefaultChecked { get; set; }

        [Browsable(true), Category("Scripts")]
        public string ScriptOnClick { get; set; }

        [Browsable(true), Category("Layout")]
        public SMControlSelection Dock { get; set; }

        [Browsable(false)]
        public SMRectangleArea Area
        {
            get { return _area; }
            set { _area = value; }
        }
        private SMRectangleArea _area = new SMRectangleArea();

        public List<MNReferencedText> Scripts = new List<MNReferencedText>();

        public bool UIStatePressed = false;
        public bool UIStateHover = false;
        public MNEvaluationResult UIStateError = MNEvaluationResult.NotEvaluated;
        public bool UIStateShowHint = false;
        public bool UIStateEnabled = true;
        public bool UIStateVisible = true;
        public bool UIStateChecked = false;

        public Size AutosizeSize = Size.Empty;
        public List<SMTokenItem> DroppedItems = new List<SMTokenItem>();

        [Browsable(false)]
        public MNReferencedCore Content { get; set; }


        protected Brush tempForeBrush = null;
        protected Brush tempBackBrush = null;
        protected Pen tempBorderPen = null;
        protected Pen tempForePen = null;


        public SMControl(MNPage p): base()
        {
            Page = p;
            
            Font = new SMFont();
            ContentPadding = new SMContentPadding();
            Paragraph = new SMParaFormat();
            HighlightState = new SMStatusLayout();
            NormalState = new SMStatusLayout();
            Dock = SMControlSelection.None;

            Autosize = false;
            Text = "";
            UniqueName = "";
            Tag = "";
            GroupControl = false;
            ContentId = "";
            Evaluation = MNEvaluationType.Inherited;
            Draggable = SMDragResponse.None;
            Clickable = false;
            Hints = "";
            ContentType = SMContentType.Undefined;
            AlwaysOnTop = false;
            GroupName = string.Empty;
            ExpectedChecked = Bool3.Undef;
            DefaultChecked = Bool3.Undef;
            Selectable = true;
            ScriptOnClick = "";
        }

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "title":
                    return p_unique_name;
                case "checked":
                    return new GSBoolean(UIStateChecked);
                default:
                    return base.GetPropertyValue(s);
            }
        }

        public virtual void StyleDidChange()
        {
        }


        public virtual void TextDidChange()
        {
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
                case "getText":
                    return new GSString(Text);
                case "toogleCheckZero":
                    SetCheckState(!UIStateChecked, 0, 1);
                    break;
                case "toogleCheckOne":
                    SetCheckState(!UIStateChecked, 1, 1);
                    break;
                case "toogleCheckMany":
                    SetCheckState(!UIStateChecked, 0, 100);
                    break;
                case "set":
                    return ExecuteMessageSet(args[0], args[1], args);
            }
            return base.ExecuteMessage(token, args);
        }

        public void SetCheckState(bool ns, int minCheck, int maxCheck)
        {
            if (ns)
            {
                // limit
                Page.LimitGroupChecked(GroupName, maxCheck - 1);
                UIStateChecked = ns;
            }
            else
            {
                UIStateChecked = ns;
                // check min
                if (GroupName.Length > 0)
                {
                    if (Page.CountGroupChecked(GroupName) < minCheck)
                    {
                        UIStateChecked = true;
                    }
                }
            }
        }

        public void ApplyStyle(MNReferencedStyle style)
        {
            if (style == null)
                return;

            ContentPadding.Set(style.ContentPadding);
            Font.Set(style.Font);
            Paragraph.Set(style.Paragraph);
            NormalState.Set(style.NormalState);
            HighlightState.Set(style.HighlightState);

            StyleDidChange();
        }

        protected virtual GSCore ExecuteMessageSet(GSCore a1, GSCore a2, GSCoreCollection args)
        {
            switch (a1.getStringValue())
            {
                case "text":
                    Text = a2.getStringValue();
                    break;
                case "style":
                    MNReferencedStyle s = Document.FindStyle(a2.getStringValue());
                    if (s != null)
                    {
                        ApplyStyle(s);
                        StyleName = s.Name;
                    }
                    break;
                case "clickable":
                    Clickable = a2.getBooleanValue();
                    break;
                case "hints":
                    Hints = a2.getStringValue();
                    break;
                case "onClick":
                    ScriptOnClick = a2.getStringValue();
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
            return Paragraph.VertAlign;
        }

        public SMHorizontalAlign GetHorizontalAlign()
        {
            return Paragraph.Align;
        }

        public Font GetUsedFont()
        {
            return Font.Font;
        }

        public virtual void RecalculateSize(MNPageContext context)
        {
            Rectangle bounds = Area.GetBounds(context);

            bounds.Size = AutosizeSize;

            Area.Height = AutosizeSize.Height;
            Area.Width = AutosizeSize.Width;
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
        /// <returns>Returns True, if drag operation and drag object was accepted by target control.
        /// Returns false if drag object was not accepted.</returns>
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

                SMDragResponse resp = dc.startControl.Draggable;
                if (resp == SMDragResponse.Line)
                {
                    if (this.Cardinality == SMConnectionCardinality.One)
                        Page.RemoveConnectionsForControl(this);
                    SMConnection conn = new SMConnection(this.Page);
                    conn.Source = dc.startControl;
                    conn.Target = this;
                    conn.ConnectionStyle = (resp == SMDragResponse.Line ? SMConnectionStyle.DirectLine : SMConnectionStyle.Invisible);
                    Page.AddConnection(conn);
                }
                else if (resp == SMDragResponse.Drag)
                {
                    bool isAdded = false;
                    foreach (SMTokenItem si in DroppedItems)
                    {
                        if (si.Tag.Equals(item.Tag))
                        {
                            isAdded = true;
                            break;
                        }
                    }
                    if (!isAdded)
                    {
                        DroppedItems.Add(item);
                    }
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
            if (this.Cardinality == SMConnectionCardinality.One || this.Cardinality == SMConnectionCardinality.Many)
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
            bw.Log("* * * CONTROL * * *\n");
            bw.WriteByte(254);
            bw.WriteInt64(Id);
            bw.WriteString(StyleName);
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
            bw.WriteInt32((Int32)Cardinality);

            foreach (MNReferencedText rt in Scripts)
            {
                bw.WriteByte(251);
                rt.Save(bw);
            }

            bw.WriteByte(250);
            bw.WriteInt32((Int32)ContentType);

            bw.WriteByte(244);
            bw.WriteBool(AlwaysOnTop);

            bw.WriteByte(243);
            bw.WriteString(GroupName);

            bw.WriteByte(242);
            bw.WriteByte((byte)ExpectedChecked);

            bw.WriteByte(241);
            bw.WriteByte((byte)DefaultChecked);

            bw.WriteByte(240);
            bw.WriteBool(Selectable);

            bw.WriteByte(239);
            Area.Save(bw);

            bw.WriteByte(238);
            bw.WriteString(ScriptOnClick);

            bw.WriteByte(237);
            ContentPadding.Save(bw);

            bw.WriteByte(236);
            NormalState.Save(bw);

            bw.WriteByte(235);
            HighlightState.Save(bw);

            bw.WriteByte(234);
            Paragraph.Save(bw);

            bw.WriteByte(233);
            Font.Save(bw);

            bw.WriteByte(232);
            bw.WriteInt32((int)Dock);

            // end-of-object
            bw.WriteByte(0);
        }

        [Browsable(true),Category("Layout")]
        public bool AlwaysOnTop { get; set; }

        public virtual bool Load(RSFileReader br)
        {
            br.Log("* * * CONTROL * * *\n");
            byte tag;
            Scripts.Clear();
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 254:
                        Id = br.ReadInt64();
                        p_style_name = br.ReadString();
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
                        Cardinality = (SMConnectionCardinality)br.ReadInt32();
                        break;
                    case 251:
                        MNReferencedText rt = new MNReferencedText();
                        rt.Load(br);
                        if (rt.Name.Equals("OnClick"))
                            ScriptOnClick = rt.Text;
                        else
                            Scripts.Add(rt);
                        break;
                    case 250:
                        ContentType = (SMContentType)br.ReadInt32();
                        break;
                    case 249:
                        Font.Name = (MNFontName)br.ReadInt32();
                        break;
                    case 248:
                        Font.Size = br.ReadFloat();
                        break;
                    case 247:
                        Paragraph.Align = (SMHorizontalAlign)br.ReadInt32();
                        break;
                    case 246:
                        Paragraph.VertAlign = (SMVerticalAlign)br.ReadInt32();
                        break;
                    case 245:
                        MNReferencedStyle pStylePrivate = new MNReferencedStyle();
                        pStylePrivate.Load(br);
                        ApplyStyle(pStylePrivate);
                        StyleName = "";
                        break;
                    case 244:
                        AlwaysOnTop = br.ReadBool();
                        break;
                    case 243:
                        GroupName = br.ReadString();
                        break;
                    case 242:
                        ExpectedChecked = (Bool3)br.ReadByte();
                        break;
                    case 241:
                        DefaultChecked = (Bool3)br.ReadByte();
                        break;
                    case 240:
                        Selectable = br.ReadBool();
                        break;
                    case 239:
                        Area = new SMRectangleArea();
                        Area.Load(br);
                        break;
                    case 238:
                        ScriptOnClick = br.ReadString();
                        break;
                    case 237:
                        ContentPadding.Load(br);
                        break;
                    case 236:
                        NormalState.Load(br);
                        break;
                    case 235:
                        HighlightState.Load(br);
                        break;
                    case 234:
                        Paragraph.Load(br);
                        break;
                    case 233:
                        Font.Load(br);
                        break;
                    case 232:
                        Dock = (SMControlSelection)br.ReadInt32();
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public virtual void Paint(MNPageContext context)
        {
            this.Paint(context, true);
        }

        public virtual void Paint(MNPageContext context, bool bEvalBorder)
        {
            if (bEvalBorder)
            {
                PaintEvaluationBorder(context, Area);
            }
        }

        private void PaintEvaluationBorder(MNPageContext context, SMRectangleArea area)
        {
            Brush bb = null;
            switch (UIStateError)
            {
                /*case MNEvaluationResult.Correct:
                    bb = Brushes.Green;
                    break;*/
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
            bool highState = UIStatePressed | UIStateHover | UIStateChecked;

            SMStatusLayout layout = highState ? HighlightState : NormalState;

            tempBackBrush = SMGraphics.GetBrush(layout.BackColor);
            tempForeBrush = SMGraphics.GetBrush(layout.ForeColor);
            if (tempForePen == null || tempForePen.Color != layout.ForeColor)
            {
                tempForePen = SMGraphics.GetPen(layout.ForeColor, 1);
            }
            if (tempBorderPen == null || tempBorderPen.Color != layout.BorderColor)
            {
                tempBorderPen = SMGraphics.GetPen(layout.BorderColor, layout.BorderWidth);
            }
        }

        protected void DrawStyledBorder(MNPageContext context, Rectangle bounds)
        {
            bool highState = UIStatePressed || UIStateHover;
            SMStatusLayout layout = (highState ? HighlightState : NormalState);
            //Debugger.Log(0, "", "Drawing " + highState + " border for control " + Id + ", but conn is found: " + (conn != null) + "\n");
            switch (layout.BorderStyle)
            {
                case SMBorderStyle.Rectangle:
                    context.g.FillRectangle(tempBackBrush, bounds);
                    context.g.DrawRectangle(tempBorderPen, bounds);
                    break;
                case SMBorderStyle.RoundRectangle:
                    context.g.DrawFillRoundedRectangle(tempBorderPen, tempBackBrush, bounds, layout.CornerRadius);
                    break;
                case SMBorderStyle.Elipse:
                    context.g.FillEllipse(tempBackBrush, bounds);
                    context.g.DrawEllipse(tempBorderPen, bounds);
                    break;
                default:
                    if (layout.BackColor != Color.Transparent)
                    {
                        context.g.FillRectangle(tempBackBrush, bounds);
                    }
                    break;
            }
        }

        [Browsable(false)]
        public string SafeTag
        {
            get
            {
                return Tag.Length == 0 ? Text.ToLower() : Tag.ToLower();
            }
        }

        public virtual SMTokenItem GetDraggableItem(Point point)
        {
            SMTokenItem item = new SMTokenItem();
            item.Tag = SafeTag;
            item.Text = Text;
            item.ContentSize = Size.Empty;
            return item;
        }

        [Browsable(false)]
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

        [Browsable(false)]
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

        [Browsable(false)]
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
            byte[] bts = GetBytes();
            SMControl newc = SMControl.FromBytes(Page, bts);
            newc.Id = Document.Data.GetNextId();
            return newc;
        }

                /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bounds">Rectangle where image is to be placed</param>
        /// <param name="image"></param>
        /// <param name="scaling"></param>
        public Rectangle DrawImage(MNPageContext context, Rectangle bounds, Image image, SMContentScaling scaling)
        {
            return DrawImage(context, bounds, image, scaling, 50, 50);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bounds">Rectangle where image is to be placed</param>
        /// <param name="image"></param>
        /// <param name="scaling"></param>
        /// <param name="offsetRelX">Value from range 0..100, it is relative
        /// placement of X location source rectnagle in source image in case of Fill</param>
        /// <param name="offsetRelY">Value from range 0..100, it is relative
        /// placement of Y location source rectnagle in source image in case of Fill</param>
        public Rectangle DrawImage(MNPageContext context, Rectangle bounds, Image image, SMContentScaling scaling, int offsetRelX, int offsetRelY)
        {
            if (scaling == SMContentScaling.Normal)
                scaling = SMContentScaling.Fit;
            if (scaling == SMContentScaling.Fit)
            {
                Size imageSize = GetImageDrawSize(bounds, image);

                switch (GetVerticalAlign())
                {
                    case SMVerticalAlign.Top:
                        break;
                    case SMVerticalAlign.Center:
                        bounds.Y += (bounds.Height - imageSize.Height) / 2;
                        break;
                    case SMVerticalAlign.Bottom:
                        bounds.Y += (bounds.Height - imageSize.Height);
                        break;
                }
                switch (GetHorizontalAlign())
                {
                    case SMHorizontalAlign.Left:
                        break;
                    case SMHorizontalAlign.Center:
                    case SMHorizontalAlign.Justify:
                        bounds.X += (bounds.Width - imageSize.Width) / 2;
                        break;
                    case SMHorizontalAlign.Right:
                        bounds.X += (bounds.Width - imageSize.Width);
                        break;
                }
                bounds.Width = imageSize.Width;
                bounds.Height = imageSize.Height;

                Rectangle currBounds = bounds;
                currBounds.Inflate(4, 4);
                DrawStyledBorder(context, currBounds);
                context.g.DrawImage(image, bounds);
            }
            else if (scaling == SMContentScaling.Stretch)
            {
                Rectangle currBounds = bounds;
                currBounds.Inflate(4, 4);
                DrawStyledBorder(context, currBounds);
                context.g.DrawImage(image, bounds);
            }
            else if (scaling == SMContentScaling.Fill)
            {
                Rectangle srcRect = Rectangle.Empty;
                srcRect.Size = GetImageSourceSize(bounds, image);
                // these are relative placements of source rectangle within image bounds
                srcRect.X = (image.Size.Width - srcRect.Size.Width) * offsetRelX / 100;
                srcRect.Y = (image.Size.Height - srcRect.Size.Height) * offsetRelY / 100;
                context.g.DrawImage(image, bounds, srcRect, GraphicsUnit.Pixel);
                return srcRect;
            }

            return bounds;
        }


        public static Size GetImageDrawSize(Rectangle bounds, Image image)
        {
            Size imageSize = image.Size;
            if (imageSize.Width > 0 && bounds.Width > 0 && imageSize.Height > 0 && bounds.Height > 0)
            {
                double ratio = Math.Max(imageSize.Width / Convert.ToDouble(bounds.Width),
                    imageSize.Height / Convert.ToDouble(bounds.Height));
                imageSize = new Size(Convert.ToInt32(imageSize.Width / ratio), Convert.ToInt32(imageSize.Height / ratio));
            }
            return imageSize;
        }

        public static Size GetImageSourceSize(Rectangle bounds, Image image)
        {
            Size imageSize = image.Size;
            if (imageSize.Width > 0 && bounds.Width > 0 && imageSize.Height > 0 && bounds.Height > 0)
            {
                double ratio = Math.Min(imageSize.Width / Convert.ToDouble(bounds.Width),
                    imageSize.Height / Convert.ToDouble(bounds.Height));
                imageSize = new Size(Convert.ToInt32(bounds.Width * ratio), Convert.ToInt32(bounds.Height * ratio));
            }
            return imageSize;
        }

        public byte[] GetBytes()
        {
            byte[] buffer = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    RSFileWriter fw = new RSFileWriter(bw);
                    fw.WriteString(Page.ObjectTypeToTag(GetType()));
                    Save(fw);
                    buffer = ms.GetBuffer();
                }
            }
            return buffer;
        }

        public static SMControl FromBytes(MNPage page, byte[] buffer)
        {
            object area = null;
            SMControl ct = null;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    RSFileReader fr = new RSFileReader(br);
                    string tag = fr.ReadString();
                    area = page.TagToObject(tag);
                    if (area is SMControl)
                        ct = (SMControl)area;
                    if (ct != null)
                        ct.Load(fr);
                }
            }
            return ct;
        }
    }

}
