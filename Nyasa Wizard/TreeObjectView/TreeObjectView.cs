using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using Rambha.Script;
using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class TreeObjectView : UserControl, INotificationTarget
    {
        public TreeObjectView()
        {
            InitializeComponent();
            ItemHeight = 24;

            ItemFont = new Font(FontFamily.GenericSansSerif, 10);
            ItemEmptyFont = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Italic);
            CellBorderColor = Color.LightGray;
            CellSelectionBackColor = Color.LightGreen;
            SelectedNode = null;
        }

        private TVItem p_tree = null;

        public int ItemHeight { get; set; }

        public Font ItemFont { get; set; }

        public Font ItemEmptyFont { get; set; }

        public Color CellBorderColor { get; set; }

        public Color CellSelectionBackColor { get; set; }

        public TVItem SelectedNode { get; set; }

        public event TreeObjectViewEventDelegate OnInitializeItem;

        public event TreeObjectViewEventDelegate OnInitializeActionMenu;


        private Pen CellBorderPen = Pens.Gray;
        private SolidBrush CellSelBackColor = new SolidBrush(Color.LightGreen);

        public TVItem Tree 
        {
            get
            {
                return p_tree;
            }
            set
            {
                p_tree = value;
                Invalidate();
            }
        }

        void INotificationTarget.OnNotificationReceived(object sender, string msg, params object[] args)
        {
            if (sender == this) return;

            switch (msg)
            {
                case "PageInserted":
                    if (p_tree != null)
                    {
                        p_tree.RefreshChildren();
                        panel1.Height = (int)GetTreeHeight();
                        panel1.Invalidate();
                    }
                    break;
                case "ControlAdded":
                    if (p_tree != null)
                    {
                        p_tree.RefreshChildren();
                        panel1.Height = (int)GetTreeHeight();
                        panel1.Invalidate();
                    }
                    break;
                case "TextInserted":
                    if (p_tree != null)
                    {
                        p_tree.RefreshChildren();
                        panel1.Height = (int)GetTreeHeight();
                        panel1.Invalidate();
                    }
                    break;
                case "ObjectSelected":
                    if (args != null && args.Length > 0)
                    {
                        if (FindAndSelectNode(args[0], Tree))
                            Invalidate();
                    }
                    break;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (p_tree == null)
                return;

            int h = (int)GetTreeHeight();
            if (panel1.Height != h)
            {
                panel1.Height = h;
                Invalidate();
                return;
            }

            if (CellBorderPen.Color != CellBorderColor)
                CellBorderPen = new Pen(CellBorderColor);
            if (CellSelBackColor.Color != CellSelectionBackColor)
                CellSelBackColor = new SolidBrush(CellSelectionBackColor);
            if (p_futureSelection != null)
                SelectedNode = null;
            PaintItem(p_tree, 0, 0, panel1.Width, e.Graphics);

            if (p_futureSelection != null)
            {
                if (SelectedNode != null)
                {
                    panel1.AutoScrollOffset = new Point(0, Math.Min(SelectedNode.PaintRect.Top, panel1.Height - this.Height));
                }
                p_futureSelection = null;
            }
        }

        public new void Invalidate()
        {
            panel1.Invalidate();
            base.Invalidate();
        }

        private GSCore p_futureSelection = null;

        public void Invalidate(GSCore futureSelection)
        {
            p_futureSelection = futureSelection;
            panel1.Invalidate();
            base.Invalidate();
        }

        private float GetTreeHeight()
        {
            return p_tree != null ? p_tree.GetTotalHeight() : 0;
        }

        private int PaintItem(TVItem item, int y, int level, int width, Graphics g)
        {
            string itemName = item.GetName();
            SizeF textSize = g.MeasureString(itemName, ItemFont);
            int lastY = y + ItemHeight;

            item.PaintRect.X = level * ItemHeight;
            item.PaintRect.Y = y;
            item.PaintRect.Height = ItemHeight;
            item.PaintRect.Width = width - level * ItemHeight;
            //Debugger.Log(0, "", "Draw Item: " + item.Name + ", rect:" + item.PaintRect.ToString() + "\n");

            if (item.IdenticalData(p_futureSelection))
            {
                SelectedNode = item;
            }

            if (SelectedNode == item)
            {
                g.FillRectangle(CellSelBackColor, item.PaintRect);
            }

            if (item.IsExpandable())
            {
                int pp = ItemHeight/5;
                g.DrawImage((item.Expanded ? Properties.Resources.iconCollapse : Properties.Resources.iconExpand), 
                    new Rectangle(item.PaintRect.X, item.PaintRect.Y, ItemHeight, ItemHeight));
            }

            g.DrawString(itemName, ItemFont, Brushes.Black, item.PaintRect.X + ItemHeight, item.PaintRect.Y + (int)(ItemHeight - textSize.Height)/2);

            if (item == SelectedNode && item.GetActions() != null)
            {
                g.DrawImage(Properties.Resources.IconActions, width - ItemHeight, y, ItemHeight, ItemHeight);
            }

            if (item.Expanded)
            {
                // padding before
                lastY += 4;
                int topY = lastY;

                if (item.Children != null && item.Children.Count > 0)
                {
                    // higher level of children
                    level++;


                    g.DrawLine(CellBorderPen, level * ItemHeight, lastY, level * ItemHeight + 8, lastY);
                    for (int i = 0; i < item.Children.Count; i++)
                    {
                        lastY = PaintItem(item.Children[i], lastY, level, width - 2, g);
                        g.DrawLine(CellBorderPen, level * ItemHeight, lastY, level * ItemHeight + 8, lastY);
                    }

                    // vertical line
                    //g.DrawLine(CellBorderPen, width - 1, topY, width - 1, lastY);
                    g.DrawLine(CellBorderPen, level * ItemHeight, topY, level * ItemHeight, lastY);

                }
                else
                {
                    textSize = g.MeasureString("(empty)", ItemEmptyFont);
                    g.DrawString("(empty)", ItemEmptyFont, Brushes.Black, item.PaintRect.X + ItemHeight, lastY + (int)(ItemHeight - textSize.Height) / 2);
                    lastY += ItemHeight;
                }

                // padding after
                lastY += 4;
            }

            return lastY;
        }

        public void SetObject(MNDocument data)
        {
            this.Tree = new TItemDocument(this) { Document = data, Name = "Document" };
            this.Tree.Expand(true);
        }

        public void OnInitializeItemDelegate(TreeObjectViewEventArgs e)
        {
            if (OnInitializeItem != null)
                OnInitializeItem(this, e);
        }

        /// <summary>
        /// Mouse variables
        /// </summary>
        private bool p_mouse_down = false;
        private HitObject p_md_hit = null;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            p_mouse_down = true;
            p_md_hit = GetHitPart(e.X, e.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            p_mouse_down = false;

            HitObject ho = GetHitPart(e.X, e.Y);

            if (HitObject.CompareHitPart(ho, p_md_hit))
            {
                if (ho.Part == HitObjectPart.ExpandButton)
                {
                    ho.Item.Expand(!ho.Item.Expanded);
                    panel1.Invalidate();
                }
                else if (ho.Part == HitObjectPart.Title)
                {
                    SelectedNode = ho.Item;
                    MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", ho.Item.GetContentData());
                    panel1.Invalidate();
                }
                else if (ho.Part == HitObjectPart.ActionButton)
                {
                    if (SelectedNode == ho.Item)
                        ShowActionsForObject(ho.Item, new Point(e.X, e.Y - VerticalScroll.Value));
                    else
                    {
                        SelectedNode = ho.Item;
                        ho.Part = HitObjectPart.Title;
                        MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", ho.Item.GetContentData());
                        panel1.Invalidate();
                    }
                }
            }

            p_md_hit = null;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (p_mouse_down)
            {
            }
            else
            {
            }
        }

        private void ShowActionsForObject(TVItem to, Point clientPoint)
        {
            if (to == null) return;

            if (to.GetContentData() == null) return;

            if (OnInitializeActionMenu == null) return;

            TreeObjectViewEventArgs e = new TreeObjectViewEventArgs();
            e.Item = to;
            e.ScreenPoint = PointToScreen(clientPoint);
            OnInitializeActionMenu(this, e);
        }

        private HitObject GetHitPart(int x, int y)
        {
            if (p_tree == null)
                return null;
            return GetHitPart(x, y, p_tree);
        }

        private HitObject GetHitPart(int x, int y, TVItem item)
        {
            HitObject hit = null;
            //Debugger.Log(0, "", "Item: " + item.Name + " " + item.PaintRect.ToString() + "\n");
            if (item.PaintRect.Contains(x, y))
            {
                hit = new HitObject();
                hit.Item = item;
                hit.Part = HitObjectPart.Title;
                if (x < item.PaintRect.X + item.PaintRect.Height && item.IsExpandable())
                    hit.Part = HitObjectPart.ExpandButton;
                else if (x > item.PaintRect.Right - ItemHeight)
                    hit.Part = HitObjectPart.ActionButton;
            }
            else if (item.IsExpandable() && item.Expanded)
            {
                foreach (TVItem sub in item.Children)
                {
                    hit = GetHitPart(x, y, sub);
                    if (hit != null) break;
                }
            }

            return hit;
        }

        public enum HitObjectPart
        {
            ExpandButton, Title, None, ActionButton
        }

        public class HitObject
        {
            public TVItem Item = null;
            public HitObjectPart Part = HitObjectPart.None;

            public static bool CompareHitPart(HitObject ho, HitObject ho2)
            {
                return ho != null
                    && ho2 != null
                    && ho2.Item == ho.Item 
                    && ho2.Part == ho.Part;
            }

        }


        public void SelectItemWithData(object p)
        {
            FindAndSelectNode(p, Tree);
        }

        private bool FindAndSelectNode(object p, TVItem item)
        {
            if (item != null)
            {
                if (item.IdenticalData(p))
                {
                    SelectedNode = item;
                    return true;
                }

                if (item.Expanded)
                {
                    foreach (TVItem ch in item.Children)
                    {
                        if (FindAndSelectNode(p, ch))
                            return true;
                    }
                }
            }

            return false;
        }
    }

    public class TreeObjectViewEventArgs : EventArgs
    {
        public TVItem Item = null;
        public Point ScreenPoint = Point.Empty;
    }

    public delegate void TreeObjectViewEventDelegate(object sender, TreeObjectViewEventArgs e);
}
