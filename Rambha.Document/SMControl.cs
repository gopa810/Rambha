using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.ComponentModel;
using System.Runtime;
using System.Runtime.Serialization;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    [Serializable()]
    public class SMControl: GSCore, IRSObjectOrigin
    {
        [Browsable(true), ReadOnly(true)]
        public long Id { get; set; }

        /// <summary>
        /// Parent page
        /// </summary>
        [Browsable(false)]
        public MNPage Page { get; set; }

        [Browsable(false)]
        public SMControl ParentObject { get; set; }

        [Browsable(false)]
        public SMStyle Style { get; set; }

        [Browsable(true), Category("Layout")]
        public bool Autosize { get; set; }

        [Browsable(true), ReadOnly(true)]
        public bool GroupControl { get; set; }

        [Browsable(true), Category("Evaluation"),Description("Content identificator for draggable and droppable")]
        public string Tag { get; set; }

        [Browsable(true), Category("API"), DisplayName("API Identificator"), Description("Name of control for the script API. If empty then this control is not referenced from script.")]
        public string UniqueName { get { return p_unique_name.Value; } set { p_unique_name.Value = value; } }

        private GSString p_unique_name = new GSString("");

        [Browsable(true),Category("Content")]
        public string Text { get; set; }

        [Browsable(true), Category("Content"), DisplayName("")]
        public string ContentId { get; set; }

        [Browsable(true), Category("Evaluation")]
        public MNEvaluationType Evaluation { get; set; }

        [Browsable(true), Category("Events")]
        public string UserEventOnClick { get; set; }
        public string UserEventOnDoubleClick { get; set; }
        public string UserEventOnLongClick { get; set; }


        public bool UIStatePressed = false;
        public bool UIStateHover = false;
        public MNEvaluationResult UIStateError = MNEvaluationResult.NotEvaluated;

        public Size AutosizeSize = Size.Empty;
        public List<SMTokenItem> DroppedItems = new List<SMTokenItem>();


        protected SolidBrush tempForeBrush = null;
        protected SolidBrush tempBackBrush = null;
        protected Pen tempBorderPen = null;
        protected Pen tempForePen = null;


        public SMControl(MNPage p): base()
        {
            Page = p;
            Autosize = false;
            Text = "";
            UniqueName = "";
            Tag = "";
            GroupControl = false;
            ContentId = "";
            Evaluation = MNEvaluationType.None;
            UserEventOnClick = "";
            UserEventOnDoubleClick = "";
            UserEventOnLongClick = "";
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

        public override GSCore[] GetChildren()
        {
            return null;
        }

        public override string ToString()
        {
            return Text;
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
                if (item == null) return false;
                foreach (SMTokenItem si in DroppedItems)
                {
                    if (si.Text.Equals(item.Text) && si.Tag.Equals(item.Tag))
                    {
                        return false;
                    }
                }
                DroppedItems.Add(item);

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
            if (Style.Droppable == SMDropResponse.One || Style.Droppable == SMDropResponse.Many)
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
            bw.WriteInt64(Page.Id);
            bw.WriteInt64(ParentObject != null ? ParentObject.Id : 0);
            bw.WriteInt64(Style != null ? Style.Id : 0);

            bw.WriteByte(253);
            bw.WriteBool(Autosize);
            bw.WriteBool(GroupControl);
            bw.WriteString(Tag);
            bw.WriteString(ContentId);
            bw.WriteString(UniqueName);
            bw.WriteInt32((Int32)Evaluation);

            bw.WriteByte(252);
            bw.WriteString(UserEventOnClick);
            bw.WriteString(UserEventOnDoubleClick);
            bw.WriteString(UserEventOnLongClick);

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
                        /*Page = new MNPageLoadingPlaceholder() { pageId = br.ReadInt32() };
                        ParentObject = new SMControlLoadingPlaceholder() { controlId = br.ReadInt32() };
                        Style = new SMStyleLoadingPlaceholder() { styleId = br.ReadInt32() };*/
                        br.AddReference(Document, "MNPage", br.ReadInt64(), 254, this);
                        br.AddReference(Document, "SMControl", br.ReadInt64(), 254, this);
                        br.AddReference(Document, "SMStyle", br.ReadInt64(), 254, this);
                        break;
                    case 253:
                        Autosize = br.ReadBool();
                        GroupControl = br.ReadBool();
                        Tag = br.ReadString();
                        ContentId = br.ReadString();
                        UniqueName = br.ReadString();
                        Evaluation = (MNEvaluationType)br.ReadInt32();
                        break;
                    case 252:
                        UserEventOnClick = br.ReadString();
                        UserEventOnDoubleClick = br.ReadString();
                        UserEventOnLongClick = br.ReadString();
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public virtual void setReference(int tag, object obj)
        {
            switch (tag)
            {
                case 254:
                    if (obj is MNPage) Page = (MNPage)obj;
                    else if (obj is SMControl) ParentObject = (SMControl)obj;
                    else if (obj is SMStyle) Style = (SMStyle)obj;
                    break;
            }
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
            return new Size(128,16);
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
            item.TextFont = Style.Font;
            item.TextBrush = new SolidBrush(Style.ForeColor);
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

    }

}
