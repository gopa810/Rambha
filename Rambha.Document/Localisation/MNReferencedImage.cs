using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Diagnostics;

using Rambha.Serializer;


namespace Rambha.Document
{
    [Serializable()]
    public class MNReferencedImage: MNReferencedCore
    {
        public long Id { get; set; }
        public string FilePath { get; set; }
        public Image ImageData { get; set; }
        public string Description { get; set; }

        private List<MNReferencedSpot> Spots = null;

        public int ItemHeight = 0;
        public int ItemTextHeight = 0;

        public MNReferencedImage()
        {
            Name = "";
            FilePath = "";
            Description = "";
        }

        public override string ToString()
        {
            return Name;
        }


        public override void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt64(Id);

            bw.WriteByte(11);
            bw.WriteString(Name);
            bw.WriteString(FilePath);
            bw.WriteString(Description);

            bw.WriteByte(12);
            bw.WriteImage(ImageData);

            if (HasSpots())
            {
                foreach (MNReferencedSpot spot in Spots)
                {
                    bw.WriteByte(13);
                    spot.Save(bw);
                }
            }

            // end of object
            bw.WriteByte(0);
        }

        public override void Load(RSFileReader br)
        {
            byte tag;
            int a;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10: Id = br.ReadInt64(); break;
                    case 11:
                        Name = br.ReadString();
                        FilePath = br.ReadString();
                        Description = br.ReadString();
                        break;
                    case 12:
                        ImageData = br.ReadImage();
                        break;
                    case 13:
                        List<MNReferencedSpot> safe = SafeSpots;
                        MNReferencedSpot spot = new MNReferencedSpot();
                        spot.Load(br);
                        safe.Add(spot);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool HasSpots()
        {
            return (Spots != null && Spots.Count > 0);
        }

        public List<MNReferencedSpot> SafeSpots
        {
            get
            {
                if (Spots == null)
                    Spots = new List<MNReferencedSpot>();
                return Spots;
            }
        }

        /// <summary>
        /// Find Spot that is under given point (given in client coordinates)
        /// </summary>
        /// <param name="showRect"></param>
        /// <param name="clientPoint"></param>
        /// <returns></returns>
        public MNReferencedSpot FindSpot(Rectangle showRect, Rectangle sourceRect, Point clientPoint)
        {
            if (Spots == null) return null;
            Point rel = AbsToRel(showRect, clientPoint, sourceRect);
            Debugger.Log(0, "", string.Format(" >> Point clientRel: {0}\n", rel));
            foreach (MNReferencedSpot spot in Spots)
            {
                Debugger.Log(0, "", string.Format("  > Spot {0} :: {1} :: {2}\n", spot.Center, spot.AnchorA, spot.AnchorB));
                if (spot.Contains(rel))
                    return spot;
            }
            return null;
        }

        public Point AbsToRel(Rectangle showRect, Point p, Rectangle srcRect)
        {
            double xa = Convert.ToDouble(p.X - showRect.X) / showRect.Width;
            double ya = Convert.ToDouble(p.Y - showRect.Y) / showRect.Height;
            int xb = srcRect.X + Convert.ToInt32(xa*srcRect.Width);
            int yb = srcRect.Y + Convert.ToInt32(ya*srcRect.Height);
            return new Point(xb*100/ImageData.Width, yb*100/ImageData.Height);
        }


        public Point AbsToRel(Rectangle showRect, Point p)
        {
            return new Point(Convert.ToInt32(Convert.ToDouble(p.X - showRect.X) / showRect.Width * 100),
                Convert.ToInt32(Convert.ToDouble(p.Y - showRect.Y) / showRect.Height * 100));
        }

        public Point RelToAbs(Rectangle showRect, Point p)
        {
            return new Point(showRect.X + Convert.ToInt32(showRect.Width * (p.X / 100.0)),
                showRect.Y + Convert.ToInt32(showRect.Height * (p.Y / 100.0)));
        }


    }
}
