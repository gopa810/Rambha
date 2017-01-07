using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Text;

namespace Rambha.Document
{
    public class SMControlGroup: SMControl
    {
        [Browsable(true)]
        public SMGroupListLabelsType GroupType { get; set; }

        private List<int> p_control_ids = new List<int>();

        public List<int> ControlIdentificators { get { return p_control_ids; } }

        public SMControlGroup(MNPage p)
            : base(p)
        {
            Text = "Label";
            GroupType = SMGroupListLabelsType.Labels;
            Autosize = false;
            GroupControl = true;
        }

        public override void Paint(MNPageContext context)
        {
            if (context.drawSelectionMarks)
            {
                SMRectangleArea area = context.CurrentPage.GetArea(Id);
                Rectangle bounds = area.GetBounds(context);

                string title = string.Format("[{0}] {1}", GroupType.ToString(), Text);

                context.g.DrawRectangle(Pens.DarkKhaki, bounds);
                context.g.DrawString(title, SystemFonts.CaptionFont, Brushes.DarkKhaki, bounds);

            }

            // draw selection marks
            base.Paint(context);
        }

        public bool ContainsControl(SMControl control)
        {
            SMRectangleArea cga = Page.GetArea(Id);
            SMRectangleArea ca = Page.GetArea(control.Id);

            bool pr = cga.GetRawRectangle(PageEditDisplaySize.LandscapeBig).Contains(ca.GetRawRectangle(PageEditDisplaySize.LandscapeBig));
            return pr ? ControlIsCompatible(control) : false;
        }

        public bool ControlIsCompatible(SMControl control)
        {
            switch (GroupType)
            {
                case SMGroupListLabelsType.Images:
                    return control is SMImage;
                case SMGroupListLabelsType.Labels:
                    return control is SMLabel;
                case SMGroupListLabelsType.CheckBoxes:
                    return control is SMCheckBox;
                default:
                    return false;
            }
        }

    }

    public enum SMGroupListLabelsType
    {
        Labels,
        Images,
        CheckBoxes
    }
}
