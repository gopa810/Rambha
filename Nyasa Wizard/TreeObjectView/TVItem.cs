using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Document;
using Rambha.Script;

namespace SlideMaker.Views
{
    public abstract class TVItem
    {
        /// <summary>
        /// Value meaning: 0 - flow, 1 - list, 2 - table with two columns, 3 - table with three columns, etc
        /// This can be initialized in handler for event OnItemInitialized for TreeObjectView
        /// </summary>
        public int ChildrenRows = 1;
        public Image ItemIcon { get; set; }

        public TreeObjectView View { get; set; }
        public bool Expanded { get; set; }
        public string Name { get; set; }
        public List<TVItem> Children = new List<TVItem>();

        private bool p_expandalble_init = false;
        private bool p_expandable = false;

        public float Height
        {
            get
            {
                if (View != null) return View.ItemHeight;
                return 24;
            }
        }

        public Rectangle PaintRect = new Rectangle();

        public TVItem(TreeObjectView view)
        {
            View = view;
            Name = null;
        }

        public abstract bool IdenticalData(object data);

        public abstract object GetContentData();

        public virtual string GetName()
        {
            return Name == null ? "?" : Name;
        }

        public virtual TVItem[] GetChildren()
        {
            return null;
        }

        public virtual NABase[] GetActions()
        {
            return null;
        }

        public void Expand(bool newState)
        {
            if (newState) RefreshChildren();
            Expanded = newState;
        }

        public float GetTotalHeight()
        {
            if (!Expanded) return Height;

            float sum = Height;
            if (Children != null)
            {
                if (Children.Count > 0)
                {
                    foreach (TVItem to in Children)
                    {
                        sum += to.GetTotalHeight();
                    }
                }
                else
                {
                    sum += View.ItemHeight;
                }
                sum += 8;
            }

            return sum;
        }

        public void RefreshChildren()
        {
            TVItem[] plist = GetChildren();

            if (Children == null)
            {
                if (plist != null)
                {
                    Children = new List<TVItem>();
                    foreach (TVItem obj in plist)
                    {
                        TreeObjectViewEventArgs e = new TreeObjectViewEventArgs();
                        e.Item = obj;
                        View.OnInitializeItemDelegate(e);
                        obj.View = this.View;
                        Children.Add(obj);
                    }
                }
            }
            else
            {
                if (plist != null)
                {
                    List<TVItem> gc = new List<TVItem>();
                    foreach (TVItem obj in plist)
                    {
                        int idxto = Children.IndexOf(obj);
                        if (idxto < 0)
                        {
                            obj.View = this.View;
                            TreeObjectViewEventArgs e = new TreeObjectViewEventArgs();
                            e.Item = obj;
                            View.OnInitializeItemDelegate(e);
                            gc.Add(obj);
                        }
                        else
                        {
                            gc.Add(Children[idxto]);
                            Children[idxto].RefreshChildren();
                        }
                    }
                    Children = gc;
                }
                else
                {
                    Children = null;
                }
            }
        }

        public bool IsExpandable()
        {
            if (p_expandalble_init) return p_expandable;

            RefreshChildren();
            p_expandable = (Children != null);
            return p_expandable;
        }

    }

}
