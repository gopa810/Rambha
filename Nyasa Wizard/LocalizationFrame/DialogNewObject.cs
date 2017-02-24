using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker
{
    public partial class DialogNewObject : Form
    {
        public class ObjTypeProxy
        {
            public SMContentType objType = SMContentType.Undefined;
            public string objTypeString = "";
            public override string ToString()
            {
                return objTypeString;
            }
        }

        public DialogNewObject(SMContentType preffered)
        {
            InitializeComponent();

            comboBox1.Items.Clear();
            comboBox1.Items.Add(new ObjTypeProxy() { objType = SMContentType.AudioText, objTypeString = "AudioText" });
            comboBox1.Items.Add(new ObjTypeProxy() { objType = SMContentType.Text, objTypeString = "Text" });
            comboBox1.Items.Add(new ObjTypeProxy() { objType = SMContentType.Image, objTypeString = "Image" });
            comboBox1.Items.Add(new ObjTypeProxy() { objType = SMContentType.Audio, objTypeString = "Audio" });

            switch (preffered)
            {
                case SMContentType.AudioText: comboBox1.SelectedIndex = 0; break;
                case SMContentType.Text: comboBox1.SelectedIndex = 1; break;
                case SMContentType.Image: comboBox1.SelectedIndex = 2; break;
                case SMContentType.Audio: comboBox1.SelectedIndex = 3; break;
                default: comboBox1.SelectedIndex = 0; break;
            }
        }

        public string ObjectName
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        public SMContentType ObjectType
        {
            get
            {
                return (comboBox1.SelectedItem as ObjTypeProxy).objType;
            }
            set
            {
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    ObjTypeProxy ot = comboBox1.Items[i] as ObjTypeProxy;
                    if (ot.objType == value)
                    {
                        comboBox1.SelectedIndex = i;
                        return;
                    }
                }
            }
        }
    }
}
