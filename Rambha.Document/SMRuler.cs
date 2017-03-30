using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMRuler
    {
        private bool[] p_value_valid = new bool[4];
        private double[] p_value = new double[4];
        private static int[,] p_max = { { 1024, 768 }, { 800, 600 } };

        public bool Changed = true;

        [Browsable(true)]
        public SMAxis Axis { get; set; }

        public void Load(RSFileReader br)
        {
            byte tag;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        p_value_valid[0] = br.ReadBool();
                        p_value_valid[1] = br.ReadBool();
                        p_value_valid[2] = br.ReadBool();
                        p_value_valid[3] = br.ReadBool();
                        p_value[0] = br.ReadDouble();
                        p_value[1] = br.ReadDouble();
                        p_value[2] = br.ReadDouble();
                        p_value[3] = br.ReadDouble();
                        break;
                    case 11: Axis = (SMAxis)br.ReadInt32(); break;
                    case 12: br.ReadBool(); break;
                    case 13: Changed = br.ReadBool(); break;
                }
            }
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteBool(p_value_valid[0]);
            bw.WriteBool(p_value_valid[1]);
            bw.WriteBool(p_value_valid[2]);
            bw.WriteBool(p_value_valid[3]);
            bw.WriteDouble(p_value[0]);
            bw.WriteDouble(p_value[1]);
            bw.WriteDouble(p_value[2]);
            bw.WriteDouble(p_value[3]);
            bw.WriteByte(11); bw.WriteInt32((int)Axis);
            bw.WriteByte(13); bw.WriteBool(Changed);
            bw.WriteByte(0);
        }


        public void RemoveValue(PageEditDisplaySize disp)
        {
            p_value_valid[(int)disp] = false;
        }

        public int GetMax(PageEditDisplaySize disp)
        {
            if (Axis == SMAxis.X)
            {
                if (disp == PageEditDisplaySize.LandscapeBig) return p_max[0, 0];
                if (disp == PageEditDisplaySize.LandscapeSmall) return p_max[1, 0];
                if (disp == PageEditDisplaySize.PortaitBig) return p_max[0, 1];
                if (disp == PageEditDisplaySize.PortaitSmall) return p_max[1, 1];
            }
            else
            {
                if (disp == PageEditDisplaySize.LandscapeBig) return p_max[0, 1];
                if (disp == PageEditDisplaySize.LandscapeSmall) return p_max[1, 1];
                if (disp == PageEditDisplaySize.PortaitBig) return p_max[0, 0];
                if (disp == PageEditDisplaySize.PortaitSmall) return p_max[1, 0];
            }
            return 1024;
        }

        /// <summary>
        /// </summary>
        /// <param name="displayDiameter"></param>
        /// <param name="sidesRatio"></param>
        /// <returns></returns>
        public double GetValue(double displayDiameter, double sidesRatio)
        {
            double A = 0, B = 0, C = -1;

            // make all values valid
            for (int i = 0; i < 4; i++)
            {
                if (p_value_valid[i]) A = p_value[i];
                else p_value[i] = A;
            }

            if (displayDiameter <= 7)
            {
                A = p_value[3];
                B = p_value[2];
            }
            else if (displayDiameter >= 10)
            {
                A = p_value[1];
                B = p_value[0];
            }
            else
            {
                A = Interpol(7, p_value[2], 10, p_value[0], displayDiameter);
                B = Interpol(7, p_value[3], 10, p_value[1], displayDiameter);
            }

            if (sidesRatio <= 3 / 4)
            {
                C = A;
            }
            else if (sidesRatio >= 4 / 3)
            {
                C = B;
            }
            else
            {
                C = Interpol(3 / 4.0, A, 4 / 3.0, B, sidesRatio);
            }

            return C;
        }

        private double Interpol(double X1, double T1, double X2, double T2, double X)
        {
            double a = (T1 - T2) / (X1 - X2);
            double b = T2 - a*X2;
            return a * X + b;
        }

        public int RelativeToAbsolute(PageEditDisplaySize disp, double v)
        {
            return Convert.ToInt32(GetMax(disp) * v);
        }

        public double AbsoluteToRelative(PageEditDisplaySize disp, int v)
        {
            return ((double)v) / GetMax(disp);
        }

        public void SetValue(PageEditDisplaySize disp, int value)
        {
            RemoveValue(PageEditDisplaySize.LandscapeBig);
            int idx = (int)disp;
            p_value[idx] = AbsoluteToRelative(disp,value);
            p_value_valid[idx] = true;
            if (idx == 0)
            {
                for (int i = 1; i < 4; i++)
                {
                    if (p_value_valid[i]) p_value[i] = p_value[0];
                }
            }
            Changed = true;
        }

        /*public void SetValue(PageEditDisplaySize dispSize, int value, SMAxis axis)
        {
            Axis = axis;
            RemoveValue(PageEditDisplaySize.LandscapeBig);
            SetValue(dispSize, value);
        }*/

        public void SetRawValue(PageEditDisplaySize dispSize, double value)
        {
            p_value[(int)dispSize] = value;
            p_value_valid[(int)dispSize] = true;
            Changed = true;
        }

        public int GetValue(PageEditDisplaySize disp)
        {
            int idx = (int)disp;
            for (int i = 0; i < 4; i++)
            {
                if (p_value_valid[idx])
                    return RelativeToAbsolute(disp, p_value[idx]);
                idx = (idx + 3) % 4;
            }

            return 0;
        }

        public SMRuler(SMAxis axis)
        {
            Axis = axis;
            SetValue(PageEditDisplaySize.LandscapeBig, 0);
        }

        public SMRuler(SMRuler p)
        {
            Axis = p.Axis;
            SetValue(PageEditDisplaySize.LandscapeBig, p.GetValue(PageEditDisplaySize.LandscapeBig));
        }

        public void Paint(MNPageContext context)
        {
        }

        public void AddValue(MNPageContext context, Point offset)
        {
            int change;
            float pageDim;
            if (Axis == SMAxis.X)
            {
                change = offset.X;
                pageDim = context.PageWidth;
            }
            else
            {
                change = offset.Y;
                pageDim = context.PageHeight;
            }

            this.SetValue(context.DisplaySize, this.GetValue(context.DisplaySize) + change);
        }


        /// <summary>
        /// Making string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}]", GetValue(PageEditDisplaySize.LandscapeBig));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public double GetRawValue(PageEditDisplaySize ds)
        {
           return p_value[(int)ds];
        }

        public bool IsValid(int i)
        {
            return p_value_valid[i];
        }

        public double this[int i]
        {
            get
            {
                return p_value[i];
            }
            set
            {
                p_value[i] = value;
            }
        }

        public void AddRawValue(double ry)
        {
            for (int i = 0; i < 4; i++)
            {
                if (p_value_valid[i])
                    p_value[i] += ry;
            }
        }
    }

    public enum SMAxis
    {
        X,
        Y
    }
}
