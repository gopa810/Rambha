using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.ComponentModel;
using System.Runtime;
using System.Runtime.Serialization;

namespace SlideMaker.Document
{
    [Serializable()]
    public class SMControl
    {
        /// <summary>
        /// Parent page
        /// </summary>
        [Browsable(false)]
        public MNPage Page { get; set; }

        [Browsable(false)]
        public SMControl ParentObject { get; set; }


        [Browsable(true)]
        public string Text { get; set; }

        /// <summary>
        /// This is relative point (as if page or image has 1000 x 1000 pt)
        /// </summary>
        [Browsable(true)]
        public MNPageRuler LeftRuler = new MNPageRuler();
        [Browsable(true)]
        public MNPageRuler TopRuler = new MNPageRuler();
        [Browsable(true)]
        public MNPageRuler RightRuler = new MNPageRuler();
        [Browsable(true)]
        public MNPageRuler BottomRuler = new MNPageRuler();

        [Browsable(false)]
        public virtual SMControlSelection Selected { get; set; }

        [Browsable(false)]
        public SMControlSelection TrackedSelection { get; set; }

        private Rectangle[] p_lastBounds = new Rectangle[9];


        public SMControl(MNPage p)
        {
            Page = p;
            Selected = SMControlSelection.None;
        }

        public override string ToString()
        {
            return Text;
        }

        public virtual SMControl CreateCopy()
        {
            SMControl to = (SMControl)Activator.CreateInstance(this.GetType(), Page);
            SMControl from = this;
            to.Text = from.Text;
            to.LeftRuler = new MNPageRuler(from.LeftRuler);
            to.RightRuler = new MNPageRuler(from.RightRuler);
            to.TopRuler = new MNPageRuler(from.TopRuler);
            to.BottomRuler = new MNPageRuler(from.BottomRuler);
            to.ParentObject = from.ParentObject;
            return to;
        }

        [Browsable(false)]
        public MNDocument Document
        {
            get
            {
                return Page.Document;
            }
        }

        public virtual XmlElement Save(XmlDocument doc)
        {
            XmlElement e1 = doc.CreateElement(this.GetType().Name);
            e1.SetAttribute("selected", Selected.ToString());
            //e1.AppendChild(SavePoint(doc, AnchorPoint, "RelativeCenter"));

            return e1;
        }

        public XmlElement SavePoint(XmlDocument doc, Point p, string nodeName)
        {
            XmlElement e2 = doc.CreateElement(nodeName);
            e2.SetAttribute("x", p.X.ToString());
            e2.SetAttribute("y", p.Y.ToString());
            return e2;
        }

        public Point LoadPoint(XmlElement e)
        {
            return new Point(int.Parse(e.GetAttribute("x")), int.Parse(e.GetAttribute("y")));
        }

        public XmlElement SaveColor(XmlDocument doc, Color p, string nodeName)
        {
            XmlElement e2 = doc.CreateElement(nodeName);
            e2.SetAttribute("red", p.R.ToString());
            e2.SetAttribute("green", p.G.ToString());
            e2.SetAttribute("blue", p.B.ToString());
            return e2;
        }

        public Color LoadColor(XmlElement e)
        {
            return Color.FromArgb(int.Parse(e.GetAttribute("red")), 
                int.Parse(e.GetAttribute("green")), int.Parse(e.GetAttribute("blue")));
        }

        public XmlElement SaveSize(XmlDocument doc, Size p, string nodeName)
        {
            XmlElement e2 = doc.CreateElement(nodeName);
            e2.SetAttribute("cx", p.Width.ToString());
            e2.SetAttribute("cy", p.Height.ToString());
            return e2;
        }

        public Size LoadSize(XmlElement e)
        {
            return new Size(int.Parse(e.GetAttribute("cx")), int.Parse(e.GetAttribute("cy")));
        }

        public XmlElement SaveString(XmlDocument doc, string s, string nodeName)
        {
            XmlElement e = doc.CreateElement(nodeName);
            e.InnerText = s;
            return e;
        }

        public string LoadString(XmlElement e)
        {
            return e.InnerText;
        }

        public XmlElement SaveInteger(XmlDocument doc, int i, string nodeName)
        {
            return SaveString(doc, i.ToString(), nodeName);
        }

        public int LoadInteger(XmlElement e)
        {
            return int.Parse(LoadString(e));
        }

        public virtual List<SMControl> Load(XmlElement e)
        {
            //if (e.HasAttribute("selected"))
            //    Selected = bool.Parse(e.GetAttribute("selected"));
            foreach (XmlElement e1 in e.ChildNodes)
            {
                if (e1.Name.Equals("RelativeCenter"))
                {
                    //AnchorPoint = LoadPoint(e1);
                }
            }

            List<SMControl> result = new List<SMControl>();
            result.Add(this);
            return result;
        }

        public Point LogicalToRelative(MNPageContext context, Point p)
        {
            return new Point(p.X * 1000 / context.PageWidth, p.Y * 1000 / context.PageHeight);
        }

        public Point RelativeToLogical(MNPageContext context, Point relPoint)
        {
            return new Point(relPoint.X * context.PageWidth / 1000, relPoint.Y * context.PageHeight / 1000);
        }

        public virtual SMControlSelection TestHitLogical(MNPageContext context, Point logicalPoint)
        {
            if (Selected == SMControlSelection.All)
            {
                if (p_lastBounds[1].Contains(logicalPoint)) return SMControlSelection.TopLeft;
                if (p_lastBounds[2].Contains(logicalPoint)) return SMControlSelection.TopCenter;
                if (p_lastBounds[3].Contains(logicalPoint)) return SMControlSelection.TopRight;
                if (p_lastBounds[4].Contains(logicalPoint)) return SMControlSelection.CenterLeft;
                if (p_lastBounds[5].Contains(logicalPoint)) return SMControlSelection.CenterRight;
                if (p_lastBounds[6].Contains(logicalPoint)) return SMControlSelection.BottomLeft;
                if (p_lastBounds[7].Contains(logicalPoint)) return SMControlSelection.BottomCenter;
                if (p_lastBounds[8].Contains(logicalPoint)) return SMControlSelection.BottomRight;
            }
            return p_lastBounds[0].Contains(logicalPoint) ? SMControlSelection.All : SMControlSelection.None;
        }

        /// <summary>
        /// Move object by specified offset defined in logical coordinate system
        /// </summary>
        /// <param name="offset">Logical offset (in logical coordinate system)</param>
        public virtual void Move(MNPageContext context, System.Drawing.Point offset)
        {
            //Point rp = LogicalToRelative(context, offset);
            if ((TrackedSelection & SMControlSelection.TopCenter) != SMControlSelection.None)
                TopRuler.AddValue(context.PageHeight, offset.Y /* context.zoom*/);
            if ((TrackedSelection & SMControlSelection.BottomCenter) != SMControlSelection.None)
                BottomRuler.AddValue(context.PageHeight, offset.Y /* context.zoom*/);
            if ((TrackedSelection & SMControlSelection.CenterLeft) != SMControlSelection.None)
                LeftRuler.AddValue(context.PageWidth, offset.X /* context.zoom*/);
            if ((TrackedSelection & SMControlSelection.CenterRight) != SMControlSelection.None)
                RightRuler.AddValue(context.PageWidth, offset.X /* context.zoom*/);
            //AnchorPoint = new Point(AnchorPoint.X + rp.X, AnchorPoint.Y + rp.Y);
        }

        public virtual void Paint(MNPageContext context)
        {
            if (Selected != SMControlSelection.None && context.drawSelectionMarks)
            {
                for (int i = 1; i < 9; i++)
                    context.g.FillRectangle(Brushes.DarkBlue, p_lastBounds[i]);
            }

        }


        public virtual Size GetDefaultSize()
        {
            return new Size(256,64);
        }

        /// <summary>
        /// returns bounds of control in logical coordinate system
        /// if control is tracked, then returns modified bounds according tracking values
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public Rectangle GetBounds(MNPageContext ctx)
        {
            int x = LeftRuler.GetAbsoluteValue(ctx.PageWidth);
            int y = TopRuler.GetAbsoluteValue(ctx.PageHeight);
            int r = RightRuler.GetAbsoluteValue(ctx.PageWidth);
            int b = BottomRuler.GetAbsoluteValue(ctx.PageHeight);


            if (Selected != SMControlSelection.None && ctx.isTracking)
            {
                if ((TrackedSelection & SMControlSelection.TopCenter) != SMControlSelection.None)
                    y += ctx.TrackedDrawOffset.Y;
                if ((TrackedSelection & SMControlSelection.BottomCenter) != SMControlSelection.None)
                    b += ctx.TrackedDrawOffset.Y;
                if ((TrackedSelection & SMControlSelection.CenterLeft) != SMControlSelection.None)
                    x += ctx.TrackedDrawOffset.X;
                if ((TrackedSelection & SMControlSelection.CenterRight) != SMControlSelection.None)
                    r += ctx.TrackedDrawOffset.X;

                int markWidth = ctx.PhysicalToLogical(3);
                p_lastBounds[1] = new Rectangle(x - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
                p_lastBounds[2] = new Rectangle((x + r) / 2 - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
                p_lastBounds[3] = new Rectangle(r - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
                p_lastBounds[4] = new Rectangle(x - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);
                p_lastBounds[5] = new Rectangle(r - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);
                p_lastBounds[6] = new Rectangle(x - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
                p_lastBounds[7] = new Rectangle((x + r) / 2 - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
                p_lastBounds[8] = new Rectangle(r - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
            }
            p_lastBounds[0] = new Rectangle(x, y, r - x, b - y);
            return p_lastBounds[0];
        }

        public void SetCenterSize(Point center, Size defSize)
        {
            LeftRuler.SetRelativeX(center.X - defSize.Width / 2);
            TopRuler.SetRelativeY(center.Y - defSize.Height / 2);
            RightRuler.SetRelativeX(center.X + defSize.Width / 2);
            BottomRuler.SetRelativeY(center.Y + defSize.Height / 2);
        }
    }

}
