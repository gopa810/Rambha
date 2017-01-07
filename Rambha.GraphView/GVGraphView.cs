using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Rambha.GraphView
{
    public partial class GVGraphView : UserControl
    {
        private GVGraph p_graph = null;

        private GVGraphViewContext p_context = new GVGraphViewContext();

        private TextBox p_textEdit = null;
        private ComboBox p_comboEdit = null;


        public event GeneralEventHandler OnItemHover;

        public GVGraph Graph
        {
            set
            {
                p_graph = value;
                p_context.Graph = value;
                p_context.SetScale(1/10f);
                p_context.Offset = new PointF(64, 64);
                Invalidate();
            }
            get
            {
                return p_graph;
            }
        }

        public IGraphDelegate GraphDelegate
        {
            get { return p_context.Delegate; }
            set { p_context.Delegate = value; }
        }

        public GVGraphView()
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(GVGraphView_MouseWheel);
        }



        private void GVGraphView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            p_context.Graphics = e.Graphics;
            p_context.BackColor = this.BackColor;

            if (p_graph != null)
            {
                foreach (GVGraphObject obj in p_graph.Objects)
                {
                    obj.Paint(p_context, 0, 0);
                }

                foreach (GVGraphConnection conn in p_graph.Connections)
                {
                    conn.Paint(p_context);
                }
            }
            else
            {
                g.DrawString("GVGraphView", SystemFonts.MenuFont, Brushes.Black, 4, 4);
            }
        }

        private void GVGraphView_MouseWheel(object sender, MouseEventArgs e)
        {
            StopTextEdit();

            PointF LM, P1, P2;
            P1 = new PointF(e.X, e.Y);
            LM = p_context.ConvertClientToLogical(e.X, e.Y);

            if (e.Delta < 0)
            {
                p_context.DescreaseScale();
                Invalidate();
            }
            else if (e.Delta > 0)
            {
                p_context.IncreaseScale();
                Invalidate();
            }

           P2 = p_context.ConvertLogicalToClient(LM.X, LM.Y);

            p_context.Offset.X += P1.X - P2.X;
            p_context.Offset.Y += P1.Y - P2.Y;
        }

        private void GVGraphView_MouseDown(object sender, MouseEventArgs e)
        {
            StopTextEdit();

            p_context.mouseDownPoint.X = e.X;
            p_context.mouseDownPoint.Y = e.Y;
            p_context.mouseMovePoint = p_context.mouseDownPoint;

            p_context.logDownPoint = p_context.ConvertClientToLogical(e.X, e.Y);
            p_context.logMovePoint = p_context.logDownPoint;

            p_context.mouseState = GVGraphViewMouseState.Down;

            // find if some object is clicked
            p_context.MovedObject = null;
            p_context.ActiveTracker = FindActiveTracker(p_context.mouseDownPoint);
            if (p_context.ActiveTracker != null)
            {
                p_context.ActiveTracker.OnTouchBegin(p_context);
            }
            else
            {
                p_context.downObject = Graph.Objects.FindObjectContainingClientPoint(p_context.mouseDownPoint);
                if (p_context.downObject != null)
                {
                    GVGraphObject go = p_context.downObject;
                    p_context.downObjectHotPointOffset.X = e.X - go.X;
                    p_context.downObjectHotPointOffset.Y = e.Y - go.Y;
                }
            }

            Invalidate();
        }

        private GVTrackerBase FindActiveTracker(PointF p)
        {
            if (p_context.SelectedObject == null) return null;
            foreach (GVTrackerBase ot in p_context.SelectedObject.getTrackers())
            {
                if (ot.drawRect.Contains(p))
                    return ot;
            }
            return null;
        }

        private void Log(string format, params object[] args)
        {
            Debugger.Log(0, "", string.Format(format, args));
            Debugger.Log(0, "", "\n");
        }

        private void GVGraphView_MouseUp(object sender, MouseEventArgs e)
        {
            if (p_context.mouseState != GVGraphViewMouseState.None)
            {
                p_context.mouseUpPoint.X = e.X;
                p_context.mouseUpPoint.Y = e.Y;

                p_context.logUpPoint = p_context.ConvertClientToLogical(e.X, e.Y);

                // find if some object is clicked
                p_context.upObject = Graph.Objects.FindObjectContainingClientPoint(p_context.mouseUpPoint);

                if (p_context.mouseState == GVGraphViewMouseState.Down)
                {
                    p_context.SelectedObject = p_context.downObject;
                    if (p_context.SelectedObject != null)
                    {
                        p_context.SelectedObject.ResetTrackers();
                    }
                    if (GraphDelegate != null)
                    {
                        GraphDelegate.OnObjectSelected(p_context.SelectedObject);
                    }
                }
                else if (p_context.mouseState == GVGraphViewMouseState.Moved)
                {
                    if (p_context.MovedObject == null)
                    {
                        p_context.Offset.X += p_context.DiagramTempOffset.X;
                        p_context.Offset.Y += p_context.DiagramTempOffset.Y;
                    }
                }

                if (p_context.ActiveTracker != null && GraphDelegate != null)
                {
                    p_context.ActiveTracker.StatusHighlighted = false;
                    if (GraphDelegate.OnTouchWillEndTracker(p_context, p_context.ActiveTracker))
                    {
                        p_context.ActiveTracker.OnTouchEnd(p_context);
                        GraphDelegate.OnTouchDidEndTracker(p_context, p_context.ActiveTracker);
                    }
                }


                // last action is to clear mouse state
                p_context.ResetPoints();
                p_context.ResetObjects();
                Invalidate();
            }
        }

        private void GVGraphView_MouseMove(object sender, MouseEventArgs e)
        {
            if (p_context.mouseState != GVGraphViewMouseState.None)
            {
                p_context.mouseMovePoint.X = e.X;
                p_context.mouseMovePoint.Y = e.Y;

                p_context.logMovePoint = p_context.ConvertClientToLogical(e.X, e.Y);

                p_context.TrackedObject = p_graph.Objects.FindObjectContainingClientPoint(p_context.mouseMovePoint);

                // test if we should transfer to MOVING status
                if (p_context.mouseState == GVGraphViewMouseState.Down)
                {
                    float diff = Math.Abs(e.X - p_context.mouseDownPoint.X) + Math.Abs(e.Y + p_context.mouseDownPoint.Y);
                    if (diff >= 6)
                    {
                        p_context.mouseState = GVGraphViewMouseState.Moved;
                    }
                }

                if (p_context.mouseState == GVGraphViewMouseState.Moved)
                {
                    if (p_context.ActiveTracker != null)
                    {
                        p_context.ActiveTracker.OnTrack(p_context);
                    }
                    else if (p_context.MovedObject == null)
                    {
                        // move whole diagram
                        p_context.DiagramTempOffset.X = e.X - p_context.mouseDownPoint.X;
                        p_context.DiagramTempOffset.Y = e.Y - p_context.mouseDownPoint.Y;
                    }
                }


                Invalidate();
            }

            if (p_context.mouseState == GVGraphViewMouseState.None)
            {
                if (OnItemHover != null)
                {
                    p_context.mouseMovePoint.X = e.X;
                    p_context.mouseMovePoint.Y = e.Y;

                    GeneralEventArgs args = new GeneralEventArgs();
                    GVGraphObject go =  Graph.Objects.FindObjectContainingClientPoint(p_context.mouseMovePoint);
                    if (go != null)
                    {
                        args.UserInfo["object"] = go;
                    }
                    else
                    {
                        GVTrackerBase tb = FindActiveTracker(p_context.mouseMovePoint);
                        if (tb != null)
                        {
                            args.UserInfo["tracker"] = tb;
                        }
                    }
                    OnItemHover(this, args);
                }
            }
        }

        private void GVGraphView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Graph != null && GraphDelegate != null)
            {
                GVGraphObject go = Graph.Objects.FindObjectContainingClientPoint(new PointF(e.X, e.Y));
                GraphDelegate.OnObjectDoubleClick(p_context, go);
            }
        }

        private void GVGraphView_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void GVGraphView_DragEnter(object sender, DragEventArgs e)
        {
            if (GraphDelegate != null)
                e.Effect = GraphDelegate.DragEnterEventCallback(this, e);
        }

        private void GVGraphView_DragDrop(object sender, DragEventArgs e)
        {
            if (GraphDelegate != null)
                GraphDelegate.DragDropEventCallback(this, e);
        }


        public PointF GetLogicalPoint(Point pt)
        {
            return p_context.ConvertClientToLogical(pt.X, pt.Y);
        }

        private GeneralDictionary p_textEdit_Tag = null;

        public ComboBox StartComboEdit(RectangleF rc, GeneralDictionary gd)
        {
            StopTextEdit();

            if (p_comboEdit == null)
            {
                p_comboEdit = new ComboBox();
                p_comboEdit.Parent = this;
                this.Controls.Add(p_comboEdit);
            }

            p_comboEdit.Visible = true;
            p_comboEdit.DropDownStyle = ComboBoxStyle.DropDownList;
            p_comboEdit.Location = new Point((int)rc.X, (int)rc.Y);
            p_comboEdit.Size = new Size((int)Math.Max(rc.Width, 64), (int)Math.Min(rc.Height, 120));
            p_comboEdit.KeyDown += new KeyEventHandler(p_comboEdit_KeyDown);
            p_textEdit_Tag = gd;
            p_comboEdit.Focus();

            return p_comboEdit;
        }

        void p_comboEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StopComboEdit();
            }
        }

        public void StopComboEdit()
        {
            if (p_comboEdit != null && p_comboEdit.Visible)
            {
                p_comboEdit.Visible = false;

                if (p_textEdit_Tag != null)
                {
                    if (GraphDelegate != null)
                    {
                        p_textEdit_Tag["text"] = p_comboEdit.Items[p_comboEdit.SelectedIndex].ToString();
                        p_textEdit_Tag["selectedIndex"] = p_comboEdit.SelectedIndex.ToString();
                        GraphDelegate.OnObjectTextEdited(p_textEdit_Tag);
                    }
                }
                p_textEdit_Tag = null;
            }
        }

        public TextBox StartTextEdit(RectangleF rc, GeneralDictionary gd)
        {
            StopTextEdit();

            if (p_textEdit == null)
            {
                p_textEdit = new TextBox();
                p_textEdit.Parent = this;
                this.Controls.Add(p_textEdit);
            }

            p_textEdit.Visible = true;
            p_textEdit.Location = new Point((int)rc.X, (int)rc.Y);
            p_textEdit.Size = new Size((int)Math.Max(rc.Width, 64), (int)Math.Min(rc.Height, 20));
            p_textEdit.KeyDown += new KeyEventHandler(p_textEdit_KeyDown);
            p_textEdit_Tag = gd;
            p_textEdit.Focus();

            return p_textEdit;
        }

        void p_textEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StopTextEdit();
            }
        }

        public void StopTextEdit()
        {
            StopComboEdit();

            if (p_textEdit != null && p_textEdit.Visible)
            {
                p_textEdit.Visible = false;

                if (p_textEdit_Tag != null)
                {
                    if (GraphDelegate != null)
                    {
                        p_textEdit_Tag["text"] = p_textEdit.Text;
                        GraphDelegate.OnObjectTextEdited(p_textEdit_Tag);
                    }
                }
                p_textEdit_Tag = null;
            }
        }

    }

    public delegate void GeneralEventHandler(object sender, GeneralEventArgs e);

    public class GeneralDictionary : Dictionary<string, object>
    {
    }

    public class GeneralEventArgs : EventArgs
    {
        public Dictionary<string, object> UserInfo = new Dictionary<string, object>();
    }
}
