using netDxf;
using SmartBridgeBuilder.Extension;
using SmartBridgeBuilder.RoadDesign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using CoordinateSystem = Tekla.Structures.Geometry3d.CoordinateSystem;

namespace SmartBridgeBuilder.Structure
{
    class Node
    {
        public double X;
        public double Y;
        public double Z;
        public double ID;

        public Node(double x1, double y1, double z1)
        {
            X = x1;
            Y = y1;
            Z = z1;
            ID = GetHashCode();
        }

        double DistTo(Node j)
        {
            return Math.Sqrt(Math.Pow((X - j.X), 2) + Math.Pow((Y - j.Y), 2) + Math.Pow((Z - j.Z), 2));
        }
    }

    class Elem
    {
        public Node Ni;
        public Node Nj;
        public int SectNum;

        public Elem(Node kp1, Node kp2, int v)
        {
            Ni = kp1;
            Nj = kp2;
            SectNum = v;
        }
    }

    public class SteelBeam
    {

        DxfDocument dxfDoc;

        List<Node> kplist = new List<Node>();
        List<Elem> linelist = new List<Elem>();


        public SteelBeam(string dxfname)
        {
            dxfDoc = DxfDocument.Load(dxfname);

            foreach (var item in dxfDoc.Lines)
            {
                Node ni = new Node(item.StartPoint.X,item.StartPoint.Y,item.StartPoint.Z);
                Node nj = new Node(item.EndPoint.X, item.EndPoint.Y, item.EndPoint.Z);
                linelist.Add(new Elem(ni, nj, item.Layer.Color.Index));
            }
        }


        public void SteelBeam_ab(string dxffile)
        {

            string[] text = File.ReadAllLines(dxffile);

            bool isnew = false;
            bool newlayer = false;
            Dictionary<string, int> dict = new Dictionary<string, int>();
            string key = "";
            int value = 0;
            string layname = "";
            double x1 = 0, x2 = 0, y1 = 0, y2 = 0, z1 = 0, z2 = 0;

            for (int ii = 0; ii < text.Count(); ii++)
            {
                string line = text[ii];
                if (line.StartsWith("LAYER"))
                {
                    newlayer = true;
                }
                if (newlayer && line.Contains("  2"))
                {
                    key = text[ii + 1].TrimEnd('\r');
                }
                if (newlayer && line.Contains(" 62"))
                {
                    value = int.Parse(text[ii + 1]);
                    dict.Add(key, value);
                    newlayer = false;
                }
            }
                                 
            for (int ii = 0; ii < text.Count(); ii++)
            {
                string line = text[ii];

                if (line.StartsWith("LINE"))
                {
                    isnew = true;
                }
                if (isnew && line.StartsWith("  8"))
                {
                    layname = text[ii + 1].TrimEnd('\r');
                }
                if (isnew && line.Contains(" 10"))
                {
                    x1 = double.Parse(text[ii + 1]);
                }
                if (isnew && line.Contains(" 20"))
                {
                    y1 = double.Parse(text[ii + 1]);
                }
                if (isnew && line.Contains(" 30"))
                {
                    z1 = double.Parse(text[ii + 1]);
                }
                if (isnew && line.Contains(" 11"))
                {
                    x2 = double.Parse(text[ii + 1]);
                }
                if (isnew && line.Contains(" 21"))
                {
                    y2 = double.Parse(text[ii + 1]);
                }
                if (isnew && line.Contains(" 31"))
                {
                    z2 = double.Parse(text[ii + 1]);

                    Node kp1 = new Node(x1, y1, z1);
                    Node kp2 = new Node(x2, y2, z2);

                    kplist.Add(kp1);

                    kplist.Add(kp2);

                    linelist.Add(new Elem(kp1, kp2, dict[layname]));
                    isnew = false;
                }
            }
        }

        public void ToTekla(ref Model myModel, ref DesignLine cl, double pkstart, double pkend,double dpk)
        {
            double pk0 = pkstart;
            double pk1;
            Point A0, B0, A1, B1;
            Point C0, C1;
            Vector vc,dir;

            CoordinateSystem curCS = new CoordinateSystem();
            CoordinateSystem C0CS,C1CS;
            Matrix CurtoC0;

            while (pk0 < pkend)
            {
                pk1 = pk0 + dpk;
                if (pk1>pkend)
                {
                    pk1 = pkend;
                }

                C0 = new Point(cl.pQX.GetCoord(pk0)[0] * 1000, cl.pQX.GetCoord(pk0)[1] * 1000, cl.sQX.GetBG(pk0) * 1000);
                C1 = new Point(cl.pQX.GetCoord(pk1)[0] * 1000, cl.pQX.GetCoord(pk1)[1] * 1000, cl.sQX.GetBG(pk1) * 1000);
                vc = new Vector(C1.X - C0.X, C1.Y - C0.Y, 0);
                dir = new Vector(C1.X - C0.X, C1.Y - C0.Y, C1.Z-C0.Z);
                
                C0CS = new CoordinateSystem(C0, vc.RotByZ(-0.5 * Math.PI), new Vector(0, 0, 1));
                C1CS = new CoordinateSystem(C1, vc.RotByZ(-0.5 * Math.PI), new Vector(0, 0, 1));

                CurtoC0 = MatrixFactory.ByCoordinateSystems(C0CS, curCS);               
                
                double rad = vc.GetAngleBetween(new Vector(1, 0, 0));

                foreach (Elem ele in linelist)
                {

                    A0 = CurtoC0.Transform(new Point(ele.Ni.X, ele.Ni.Y, 0));
                    B0 = CurtoC0.Transform(new Point(ele.Nj.X, ele.Nj.Y, 0));
                    A1 = A0.MoveTo(dir.X, dir.Y, dir.Z);
                    B1 = B0.MoveTo(dir.X, dir.Y, dir.Z);

                    TeklaTools.CreatPlate(A0, B0, B1, A1, ele.SectNum);
                }
                pk0 = pk1;
            }
        }





        //public void ToTekla(ref Model myModel, ref DesignLine cl, double pkstart, double pkend)
        //{
        //    double pk0 = pkstart;
        //    double pk1;
        //    Point A0, B0, A1, B1;
        //    Point C0, C1;
        //    Vector vc;
        //    while (pk0 <= pkend)
        //    {
        //        pk1 = pk0 + 5;

        //        C0 = new Point(cl.pQX.GetCoord(pk0)[0] * 1000, cl.pQX.GetCoord(pk0)[1] * 1000, cl.sQX.GetBG(pk0) * 1000);
        //        C1 = new Point(cl.pQX.GetCoord(pk1)[0] * 1000, cl.pQX.GetCoord(pk1)[1] * 1000, cl.sQX.GetBG(pk1) * 1000);
        //        vc = new Vector(C1.X - C0.X, C1.Y - C0.Y,0);

                
        //        double rad = vc.GetAngleBetween(new Vector(1, 0, 0));

        //        foreach (Elem ele in linelist)
        //        {
        //            A0 = new Point(C0.MoveTo(0, ele.Ni.X, ele.Ni.Y));
        //            B0 = new Point(C0.MoveTo(0, ele.Nj.X, ele.Nj.Y));
        //            A1 = new Point(C1.MoveTo(0, ele.Ni.X, ele.Ni.Y));
        //            B1 = new Point(C1.MoveTo(0, ele.Nj.X, ele.Nj.Y));


        //            A0 = A0.MoveTo(-C0.X, -C0.Y, -C0.Z).RotByZ(rad).MoveTo(C0.X, C0.Y, C0.Z);

        //            B0 = B0.MoveTo(-C0.X, -C0.Y, -C0.Z).RotByZ(rad).MoveTo(C0.X, C0.Y, C0.Z);

        //            A1 = A1.MoveTo(-C1.X, -C1.Y, -C1.Z).RotByZ(rad).MoveTo(C1.X, C1.Y, C1.Z);

        //            B1 = B1.MoveTo(-C1.X, -C1.Y, -C1.Z).RotByZ(rad).MoveTo(C1.X, C1.Y, C1.Z);

        //            //A0 = A0.MoveTo(-C0.X, -C0.Y, -C0.Z);
        //            //A0 = A0.RotByZ(0);
        //            //A0 = A0.MoveTo(C0.X, C0.Y, C0.Z);

        //            //B0 = B0.MoveTo(-C0.X, -C0.Y, -C0.Z);
        //            //B0 = B0.RotByZ(0);
        //            //B0 = B0.MoveTo(C0.X, C0.Y, C0.Z);

        //            //A1 = A1.MoveTo(-C1.X, -C1.Y, -C1.Z);
        //            //A1 = A1.RotByZ(0);
        //            //A1 = A1.MoveTo(C1.X, C1.Y, C1.Z);

        //            //B1 = B1.MoveTo(-C1.X, -C1.Y, -C1.Z);
        //            //B1 = B1.RotByZ(0);
        //            //B1 = B1.MoveTo(C1.X, C1.Y, C1.Z);


        //            TeklaTools.CreatPlate(A0, B0, B1, A1, ele.SectNum);

        //        }

        //        pk0 = pk1;




        //    }






        //}








    }
}
