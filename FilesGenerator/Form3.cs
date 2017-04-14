using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FilesGenerator
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Paint(object sender, PaintEventArgs e)
        {
            RectangleStatistic rs = new RectangleStatistic();

            rs.Rects.Add(new Rectangle(-40, -40, 100, 100));
            rs.Rects.Add(new Rectangle(-40, 0, 300, 20));

            ShadowPainter sp = new ShadowPainter(rs.TotalRectangle, Color.White, 16, 16);
            rs.DrawInto(sp);

            e.Graphics.DrawLine(Pens.Black, 0, 0, 300, 300);

            Image img = sp.GetPNGImage();
            e.Graphics.DrawImage(img, 0, 0);
//            e.Graphics.FillEllipse(pgb, 0, 0, 140, 70);
        }

    }
}
