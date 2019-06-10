using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartBridgeBuilder.RoadDesign;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using SmartBridgeBuilder.Extension;
using CADImport.DWG;
using CADImport;
using Autodesk.AutoCAD.Geometry;

namespace SmartBridgeBuilder.Structure
{
    public class Cables
    {
        public double PkB1, PkT1, PkT2, PkB2;
        public double HB1, HT1, H0, HT2, HB2;
        public int NumMid, NumSide;
        public double StartMid, StartSide;
        public double D1, D2, W;


        public Cables(string subfile)
        {
            string[] altext = File.ReadAllLines(subfile);

            foreach (string item in altext)
            {
                if (item.StartsWith("//"))
                {
                    continue;
                }
                try
                {
                    string line = item.TrimEnd('\r');
                    line = line.TrimEnd('\t');
                    var xx = Regex.Split(line, @"\s+");
                    PkB1 = double.Parse(xx[0]);
                    PkT1 = double.Parse(xx[1]);
                    PkT2 = double.Parse(xx[2]);
                    PkB2 = double.Parse(xx[3]);
                    HB1 = double.Parse(xx[4]);
                    HT1 = double.Parse(xx[5]);
                    H0 = double.Parse(xx[6]);
                    HT2 = double.Parse(xx[7]);
                    HB2 = double.Parse(xx[8]);
                    NumMid = int.Parse(xx[9]);
                    NumSide = int.Parse(xx[10]);
                    StartMid = double.Parse(xx[11]);
                    StartSide = double.Parse(xx[12]);
                    D1 = double.Parse(xx[13]);
                    D2 = double.Parse(xx[14]);
                    W = double.Parse(xx[15]);
                }
                catch (Exception)
                {
                    throw;
                }
                break;

            }
        }

        public void ToTekla(ref Model myModel, ref DesignLine maggie)
        {
            var x0y0 = maggie.pQX.GetCoord((PkT1 + PkT2) * 0.5);
            double x0 = x0y0[0] * 1000;
            double y0 = x0y0[1] * 1000;
            double z0 = maggie.sQX.GetBG((PkT1 + PkT2) * 0.5) * 1000;

            var x1y1 = maggie.pQX.GetCoord((PkT1 + PkT2) * 0.5 + 1);
            double x1 = x1y1[0] * 1000;
            double y1 = x1y1[1] * 1000;

            Vector dir = new Vector(x1 - x0, y1 - y0, 0);

            CoordinateSystem global = new CoordinateSystem();
            CoordinateSystem loc = new CoordinateSystem(new Point(x0, y0, 0), dir, dir.RotByZ(0.5 * Math.PI));
            Matrix mat = MatrixFactory.ByCoordinateSystems(loc, global);

            string mCab = string.Format("D{0}", D1 * 1000);
            string sCab = string.Format("D{0}", D2 * 1000);

            // 主缆
            double ll = PkT2 - PkT1;
            double s1 = PkT1 - PkB1;
            double s2 = PkB2 - PkT2;
            double xi = -(0.5 * ll + s1);

            var xlist = new List<double>() { xi };
            xlist.AddRange(GetList(xi, xi + s1 - StartSide, NumSide - 1));
            xlist.Add(-0.5 * ll);
            xlist.Add(-0.5 * ll + StartMid);
            xlist.AddRange(GetList(xi + s1 + StartMid, 0.5 * ll - StartMid, NumMid));
            xlist.Add(0.5 * ll);
            if (NumSide == 1)
            {
                xlist.Add(0.5 * ll + s2);
            }
            else
            {
                xlist.Add(0.5 * ll + StartSide);
                xlist.AddRange(GetList(0.5 * ll + StartSide, 0.5 * ll + s2, NumSide - 1));
            }



            foreach (var yy in new[] { 0.5 * W * 1000, -0.5 * W * 1000 })
            {
                for (int i = 0; i < xlist.Count - 1; i++)
                {
                    double xx0 = xlist[i] * 1000;
                    double zz0 = GetZinM(xlist[i]) * 1000;
                    double xx1 = xlist[i + 1] * 1000;
                    double zz1 = GetZinM(xlist[i + 1]) * 1000;
                    TeklaTools.CreatBeam(mat.Transform(new Point(xx0, yy, zz0)), mat.Transform(new Point(xx1, yy, zz1)), mCab, 0);

                }

                for (int i = 0; i < xlist.Count ; i++)
                {                    
                    double xx0 = xlist[i] * 1000;
                    if (xx0==-1000*(0.5*ll+s1)|| xx0 == -1000 * (0.5 * ll)|| xx0 == 1000 * (0.5 * ll )|| xx0 == 1000 * (0.5 * ll + s2))
                    {
                        continue;
                    }
                    double zz0 = GetZinM(xlist[i]) * 1000;
                    double pk0 = (PkT1 + PkT2) * 0.5 + (xx0/1000);
                    double zz1 = maggie.sQX.GetBG(pk0) * 1000-1250;
                    
                    TeklaTools.CreatBeam(mat.Transform(new Point(xx0, yy, zz0)), mat.Transform(new Point(xx0, yy, zz1)), sCab, 0);
                }

            }
        }

        public void ToDXF(ref DWGImage vDrawing, ref DesignLine maggie)
        {

            var x0y0 = maggie.pQX.GetCoord((PkT1 + PkT2) * 0.5);
            double x0 = x0y0[0] * 1000;
            double y0 = x0y0[1] * 1000;
            double z0 = maggie.sQX.GetBG((PkT1 + PkT2) * 0.5) * 1000;

            

            CoordinateSystem global = new CoordinateSystem();
            CoordinateSystem loc = new CoordinateSystem(new Point(0.5*(PkT1+PkT2)*1000, 0, 0), new Vector(1,0,0),new Vector(0,1,0));
            Matrix mat = MatrixFactory.ByCoordinateSystems(loc, global);
                  

            CADLWPolyLine maincab =new CADLWPolyLine();
            CADLine subcab;

            CADMatrix CADmat = new CADMatrix();
            CADmat.Data = mat.GetData();


            // 主缆
            double ll = PkT2 - PkT1;
            double s1 = PkT1 - PkB1;
            double s2 = PkB2 - PkT2;
            double xi = -(0.5 * ll + s1);

            var xlist = new List<double>() { xi };
            xlist.AddRange(GetList(xi, xi + s1 - StartSide, NumSide - 1));
            xlist.Add(-0.5 * ll);
            xlist.Add(-0.5 * ll + StartMid);
            xlist.AddRange(GetList(xi + s1 + StartMid, 0.5 * ll - StartMid, NumMid));
            xlist.Add(0.5 * ll);
            if (NumSide == 1)
            {
                xlist.Add(0.5 * ll + s2);
            }
            else
            {
                xlist.Add(0.5 * ll + StartSide);
                xlist.AddRange(GetList(0.5 * ll + StartSide, 0.5 * ll + s2, NumSide - 1));
            }
            for (int i = 0; i < xlist.Count; i++)
            {
                double xx0 = xlist[i] * 1000;
                double zz0 = GetZinM(xlist[i]) * 1000;
                var fs1 = CADmat.PtXMat(new DPoint(xx0, zz0, 0));
                CADVertex vert;
                vert = new CADVertex();
                vert.Point = fs1;
                maincab.Entities.Add(vert);
            }

            maincab.Layer = vDrawing.Converter.LayerByName("粗线");
            vDrawing.Converter.Loads(maincab);
            vDrawing.Converter.OnCreate(maincab);
            vDrawing.CurrentLayout.AddEntity(maincab);

            NetCADTool.InsertBG(ref vDrawing, "bg", new DPoint(0.5 * (PkT1 + PkT2) * 1000, GetZinM(0) * 1000, 0), 30);
            NetCADTool.InsertBG(ref vDrawing, "bg", new DPoint(PkB1 * 1000, GetZinM(-0.5 * ll-s1) * 1000, 0), 30);
            NetCADTool.InsertBG(ref vDrawing, "bg", new DPoint(PkB2 * 1000, GetZinM(0.5 * ll+s2) * 1000, 0), 30);

            NetCADTool.AddBlock(ref vDrawing, "tw", new DPoint(PkT1 * 1000, maggie.sQX.GetBG(PkT1) * 1000, 0), 1);
            NetCADTool.AddBlock(ref vDrawing, "tw", new DPoint(PkT2 * 1000, maggie.sQX.GetBG(PkT2) * 1000, 0), 1);

            NetCADTool.AddBlock(ref vDrawing, "zmd", new DPoint(PkB1 * 1000, HB1 * 1000, 0), 1);
            NetCADTool.AddBlock(ref vDrawing, "ymd", new DPoint(PkB2 * 1000, HB2 * 1000, 0), 1);
            NetCADTool.AddDim(ref vDrawing, "1-5000", new DPoint(PkB1 * 1000, HT1 * 1000 + 10 * 5000, 0), new DPoint(PkT1 * 1000, HT1 * 1000 + 10 * 5000, 0), 20 * 5000);
            NetCADTool.AddDim(ref vDrawing, "1-5000", new DPoint(PkT1 * 1000, HT1 * 1000 + 10 * 5000, 0), new DPoint(PkT2 * 1000, HT1 * 1000 + 10 * 5000, 0), 20 * 5000);
            NetCADTool.AddDim(ref vDrawing, "1-5000", new DPoint(PkT2 * 1000, HT1 * 1000 + 10 * 5000, 0), new DPoint(PkB2 * 1000, HT1 * 1000 + 10 * 5000, 0), 20 * 5000);
            // 吊杆
            for (int i = 0; i < xlist.Count; i++)
            {

                double xx0 = xlist[i] * 1000;
                double zz0 = GetZinM(xlist[i]) * 1000;
                var pt1 = CADmat.PtXMat(new DPoint(xx0, zz0, 0));

                if (new double[] { PkB1, PkB2, PkT1, PkT2 }.Contains(pt1.X/1000.0))
                {
                    continue;
                }

                double zz1 = maggie.sQX.GetBG(pt1.X/1000.0) * 1000;
                
                var pt2 = CADmat.PtXMat(new DPoint(xx0, zz1, 0));

                subcab = new CADLine();
                subcab.Point = pt1;
                subcab.Point1 = pt2;

                subcab.Layer = vDrawing.Converter.LayerByName("细线");
                vDrawing.Converter.Loads(subcab);
                vDrawing.Converter.OnCreate(subcab);
                vDrawing.CurrentLayout.AddEntity(subcab);
            }


            



        }

        
        private List<double> GetList(double xi, double xj, int numstep)
        {
            List<double> res = new List<double>();

            if (numstep <= 0 || xi > xj)
            {
                return new List<double>();
            }
            else
            {
                double step = (xj - xi) / numstep;
                double item = xi + step;
                while (item <= xj)
                {
                    res.Add(item);
                    item += step;
                }
                return res;
            }
        }

        double GetZinM(double x0)
        {
            double ll = PkT2 - PkT1;
            double s1 = PkT1 - PkB1;
            double s2 = PkB2 - PkT2;
            double C = H0;
            double A = (HT2 - H0) / Math.Pow((0.5 * ll), 2);
            double B = 0;

            double k1 = (HT1 - HB1) / s1;
            double b1 = HT1 - (-0.5 * ll) * k1;
            double k2 = (HB2 - HT1) / s2;
            double b2 = HT1 - (0.5 * ll) * k2;

            if (x0 <= -0.5 * ll - s1)
            {
                return HB1;
            }
            else if (x0 <= -0.5 * ll)
            {
                return x0 * k1 + b1;
            }
            else if (x0 <= 0.5 * ll)
            {
                return x0 * x0 * A + x0 * B + C;

            }
            else if (x0 <= 0.5 * ll + s2)
            {
                return x0 * k2 + b2;
            }
            else
            {
                return HB2;
            }
        }



    }





}
