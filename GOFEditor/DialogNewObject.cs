using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.GOF;

namespace GOFEditor
{
    public partial class DialogNewObject : Form
    {
        public class ObjTypeProxy
        {
            public GOFObjectType objType = GOFObjectType.Directory;
            public string objTypeString = "";
            public override string ToString()
            {
                return objTypeString;
            }
        }

        public DialogNewObject()
        {
            InitializeComponent();

            comboBox1.Items.Clear();
            comboBox1.Items.Add(new ObjTypeProxy() { objType = GOFObjectType.Directory, objTypeString = "Directory" });
            comboBox1.Items.Add(new ObjTypeProxy() { objType = GOFObjectType.RunningText, objTypeString = "Running Text" });
            comboBox1.Items.Add(new ObjTypeProxy() { objType = GOFObjectType.String, objTypeString = "String" });
            comboBox1.Items.Add(new ObjTypeProxy() { objType = GOFObjectType.Image, objTypeString = "Image" });
            comboBox1.Items.Add(new ObjTypeProxy() { objType = GOFObjectType.SoundFile, objTypeString = "SoundFile" });
            comboBox1.SelectedIndex = 0;
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

        public GOFObjectType ObjectType
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
