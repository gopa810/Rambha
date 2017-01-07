using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Rambha.GOF;

namespace GOFEditor.Views
{
    public partial class EditControlDirectory : UserControl
    {
        public MainForm ParentFrame { get; set; }

        public class LPVCommand
        {
            public const int GOUP = 1;
            public const int GOSUBDIR = 2;
            public const int ITEM = 3;
            public const int ADDITEM = 4;

            public int ItemType = 0;
            public string Text = "";
            public string Arg1 = "";

            public GOFCoreObject Item = null;
        }

        private GOFile data = null;

        private List<string> path = new List<string>();

        public EditControlDirectory()
        {
            InitializeComponent();
        }


        public void SetLanguageData(GOFile iData)
        {
            data = iData;
            label1.Text = ">";
            path.Clear();
            UpdateListView();
            
        }

        public void UpdateListView()
        {
            GOFNodes nodes = GetPathLeaf();
            LPVCommand lvi = null;

            // init path title
            StringBuilder sb = new StringBuilder();
            sb.Append(">");
            for (int j = 0; j < path.Count; j++)
            {
                sb.AppendFormat(" {0} >", path[j]);
            }
            label1.Text = sb.ToString();

            // clear list
            listBox1.Items.Clear();

            // if needed to go up
            if (path.Count > 0)
            {
                lvi = new LPVCommand() { ItemType = LPVCommand.GOUP, Text = "..." };
                listBox1.Items.Add(lvi);
            }

            foreach (KeyValuePair<string, GOFCoreObject> P in nodes.GetEnumerableNodes())
            {
                if (P.Value is GOFNodes)
                {
                    lvi = new LPVCommand() { ItemType = LPVCommand.GOSUBDIR, Text = P.Key, Arg1 = P.Key };
                }
                else
                {
                    lvi = new LPVCommand() { ItemType = LPVCommand.ITEM, Text = P.Key, Item = P.Value };
                }
                listBox1.Items.Add(lvi);
            }

            listBox1.Items.Add(new LPVCommand() { ItemType = LPVCommand.ADDITEM, Text = "Add New Item" });
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            DialogNewObject d = new DialogNewObject();
            if (d.ShowDialog() == DialogResult.OK)
            {
                GOFNodes nodes = GetPathLeaf();
                switch (d.ObjectType)
                {
                    case GOFObjectType.Directory:
                        {
                            if (nodes != null)
                            {
                                nodes.SetValue(d.ObjectName, new GOFNodes());
                                UpdateListView();
                            }
                        }
                        break;
                    case GOFObjectType.Image:
                        {
                            OpenFileDialog fd = new OpenFileDialog();
                            fd.Filter = "Images (*.png,*.jpg,*.bmp)|*.png;*.jpg;*.bmp|All Files (*.*)|*.*||";
                            if (fd.ShowDialog() == DialogResult.OK)
                            {
                                GOFImage img = new GOFImage();
                                img.SetData(File.ReadAllBytes(fd.FileName));
                                if (nodes != null)
                                {
                                    nodes.SetValue(d.ObjectName.Trim().Length > 0 ? d.ObjectName.Trim() : fd.FileName, img);
                                    UpdateListView();
                                }
                            }
                        }
                        break;
                    case GOFObjectType.RunningText:
                        {
                            if (nodes != null)
                            {
                                nodes.SetValue(d.ObjectName, new GOFRunningText());
                                UpdateListView();
                            }
                        }
                        break;
                    case GOFObjectType.SoundFile:
                        {
                            OpenFileDialog fd = new OpenFileDialog();
                            fd.Filter = "Sounds (*.wav,*.mp3,*.aiff)|*.wav;*.mp3;*.aiff|All Files (*.*)|*.*||";
                            if (fd.ShowDialog() == DialogResult.OK)
                            {
                                GOFSound img = new GOFSound();
                                img.InitializeWithFile(fd.FileName);
                                if (nodes != null)
                                {
                                    nodes.SetValue(d.ObjectName.Trim().Length > 0 ? d.ObjectName.Trim() : fd.FileName, img);
                                    UpdateListView();
                                }
                            }
                        }
                        break;
                    case GOFObjectType.String:
                        {
                            if (nodes != null)
                            {
                                nodes.SetValue(d.ObjectName, new GOFString());
                                UpdateListView();
                            }
                        }
                        break;
                }
            }
        }

        public GOFNodes GetPathLeaf()
        {
            GOFNodes nodes = data.GetNodes();


            for (int i = 0; i < path.Count; i++)
            {
                GOFCoreObject a = nodes.GetValue(path[i]);
                if (a is GOFNodes)
                {
                    nodes = a as GOFNodes;
                }
                else
                {
                    return nodes;
                }
            }

            return nodes;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(new Point(e.X, e.Y));
            LPVCommand lvi = GetItem(index);
            if (lvi != null)
            {
                if (lvi.ItemType == LPVCommand.GOUP)
                {
                    if (path.Count > 0)
                        path.RemoveAt(path.Count - 1);
                    UpdateListView();
                }
                else if (lvi.ItemType == LPVCommand.GOSUBDIR)
                {
                    path.Add(lvi.Arg1);
                    UpdateListView();
                }
                else if (lvi.ItemType == LPVCommand.ADDITEM)
                {
                    buttonNew_Click(this, e);
                }
                else if (lvi.ItemType == LPVCommand.ITEM && lvi.Item != null)
                {
                    if (ParentFrame != null)
                        ParentFrame.PresentData(lvi.Item, lvi.Text);
                }
            }
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                string s = (string)e.Data.GetData(DataFormats.UnicodeText);
                if (s.IndexOf('\n') >= 0)
                {
                    // here we can include text object or split to lines, but
                    // splitting to lines only when we have pairs of strings in each line
                    string[] lines = s.Split('\r', '\n');
                    GOFNodes current = GetPathLeaf();
                    foreach (string line in lines)
                    {
                        int index = line.IndexOf('\t');
                        if (index > 0)
                        {
                            string key = line.Substring(0, index).Trim();
                            if (key.Length > 0)
                            {
                                GOFString str = new GOFString() { Text = line.Substring(index + 1) };
                                current.SetValue(key, str);
                            }
                        }
                    }
                    UpdateListView();
                }
                else if (s.IndexOf('\t') >= 0)
                {
                    // if we have pair, insert it as string object
                    int splitterIndex = s.IndexOf('\t');
                    string key = s.Substring(0, splitterIndex);
                    GOFString str = new GOFString();
                    str.Text = s.Substring(splitterIndex + 1);
                    GOFNodes current = GetPathLeaf();
                    current.SetValue(key, str);
                    UpdateListView();
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                GOFNodes current = GetPathLeaf();
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".tif"))
                        {
                            string key = Path.GetFileNameWithoutExtension(file);
                            GOFImage img = new GOFImage();
                            img.SetData(File.ReadAllBytes(file));
                            current.SetValue(key, img);
                        }
                        else if (file.EndsWith(".mp3") || file.EndsWith(".wma") || file.EndsWith(".aiff"))
                        {
                            string key = Path.GetFileNameWithoutExtension(file);
                            GOFSound img = new GOFSound();
                            img.InitializeWithFile(file);
                            current.SetValue(key, img);
                        }
                    }
                    UpdateListView();
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private LPVCommand GetItem(int i)
        {
            if (i < 0) return null;
            if (i >= listBox1.Items.Count) return null;

            return (LPVCommand)listBox1.Items[i];
        }

        private bool ValidItemIndex(int i)
        {
            return (i >= 0 && i < listBox1.Items.Count);
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (!ValidItemIndex(e.Index)) return;
            LPVCommand item = GetItem(e.Index);

            if (item.ItemType == LPVCommand.GOUP || item.ItemType == LPVCommand.GOSUBDIR)
            {
                e.ItemHeight = 32;
                return;
            }
            else if (item.Item is GOFString)
            {
                e.ItemHeight = 32;
            }
            else if (item.Item is GOFImage)
            {
                e.ItemHeight = 64;
            }
            else
            {
                e.ItemHeight = 32;
            }
        }

        private SolidBrush p_lastBkg = null;
        private Brush p_selectedBkg = Brushes.LightBlue;
        private StringFormat p_sformat = null;

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!ValidItemIndex(e.Index)) return;
            LPVCommand item = GetItem(e.Index);

            if (p_sformat == null)
            {
                p_sformat = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near };
                p_sformat.Trimming = StringTrimming.EllipsisWord;
            }
            if (p_lastBkg == null || p_lastBkg.Color != this.BackColor)
            {
                p_lastBkg = new SolidBrush(listBox1.BackColor);
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(p_selectedBkg, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(p_lastBkg, e.Bounds);
            }


            if (item.ItemType == LPVCommand.GOUP)
            {
                e.Graphics.DrawImage(Properties.Resources.navigate_up_icon, e.Bounds.Left + 4, 
                    e.Bounds.Top + 4, e.Bounds.Height - 8, e.Bounds.Height - 8);
                int w = e.Bounds.Height + 4;
                e.Graphics.DrawString(item.Text, SystemFonts.DefaultFont, Brushes.Black,
                    new Rectangle(e.Bounds.Left + w + 8, e.Bounds.Top + 4, e.Bounds.Width - w - 12, e.Bounds.Height - 8), p_sformat);
            }
            else if (item.ItemType == LPVCommand.GOSUBDIR)
            {
                e.Graphics.DrawImage(Properties.Resources.icon_folder, e.Bounds.Left + 4,
                    e.Bounds.Top + 4, e.Bounds.Height - 8, e.Bounds.Height - 8);
                int w = e.Bounds.Height + 4;
                e.Graphics.DrawString(item.Text, SystemFonts.DefaultFont, Brushes.Black,
                    new Rectangle(e.Bounds.Left + w + 8, e.Bounds.Top + 4, e.Bounds.Width - w - 12, e.Bounds.Height - 8), p_sformat);
            }
            else if (item.ItemType == LPVCommand.ADDITEM)
            {
                e.Graphics.DrawImage(Properties.Resources.icon_add, e.Bounds.Left + 4,
                    e.Bounds.Top + 4, e.Bounds.Height - 8, e.Bounds.Height - 8);
                int w = e.Bounds.Height + 4;
                e.Graphics.DrawString(item.Text, SystemFonts.DefaultFont, Brushes.Black,
                    new Rectangle(e.Bounds.Left + w + 8, e.Bounds.Top + 4, e.Bounds.Width - w - 12, e.Bounds.Height - 8), p_sformat);
            }
            else if (item.Item is GOFString)
            {
                GOFString s = item.Item as GOFString;
                SizeF size = e.Graphics.MeasureString(item.Text, SystemFonts.CaptionFont);
                int w = 0;
                e.Graphics.DrawString(item.Text, SystemFonts.DefaultFont, Brushes.DarkBlue, 
                    new Rectangle(e.Bounds.Left + w + 8, e.Bounds.Top + 4, e.Bounds.Width - w - 12, e.Bounds.Height - 8), p_sformat);
                w = (int)size.Width + 8;
                e.Graphics.DrawString(s.Text, SystemFonts.DefaultFont, Brushes.Black, 
                    new Rectangle(e.Bounds.Left + w + 8, e.Bounds.Top + 4, e.Bounds.Width - w - 12, e.Bounds.Height - 8), p_sformat);

            }
            else if (item.Item is GOFImage)
            {
                Image i = (item.Item as GOFImage).Image;
                if (i.Height > 0)
                {
                    int w = (int)((e.Bounds.Height - 8) *  ((float)(i.Width)) / i.Height);
                    e.Graphics.DrawImage(i, e.Bounds.Left + 4, e.Bounds.Top + 4, w, e.Bounds.Height - 8);
                    e.Graphics.DrawString(item.Text, SystemFonts.DefaultFont, Brushes.Black, new Rectangle(e.Bounds.Left + w + 8, e.Bounds.Top + 4, e.Bounds.Width - w - 12, e.Bounds.Height - 8), p_sformat);
                }
            }
            else
            {
                int w = 0;
                e.Graphics.DrawString(item.Text, SystemFonts.DefaultFont, Brushes.Black, 
                    new Rectangle(e.Bounds.Left + w + 8, e.Bounds.Top + 4, e.Bounds.Width - w - 12, e.Bounds.Height - 8));
            }

        }

        public void ReplaceNode(string key, GOFCoreObject oldObject, GOFCoreObject newObject)
        {
            foreach (LPVCommand p in listBox1.Items)
            {
                if (p.Text.Equals(key) && p.ItemType == LPVCommand.ITEM && p.Item == oldObject)
                {
                    p.Item = newObject;
                    break;
                }
            }
        }
    }
}
