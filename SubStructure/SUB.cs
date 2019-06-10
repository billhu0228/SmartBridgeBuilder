using CADImport;
using CADImport.DWG;
using SmartBridgeBuilder.Extension;
using SmartBridgeBuilder.RoadDesign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace SmartBridgeBuilder.Structure
{
    public struct SubStr
    {
        public int typeID, PileLNum, PileTNum;
        public double offsetL, offsetT, offsetH;
        public double PK, PierL, PierT,  Lz, Dz;
        // data for pile
        public double PileSpaceL, PileSpaceT;
        public double PileCapL, PileCapT, PileCapH;
        public double CrossBeamTm, CrossBeamTs, CrossBeamHm, CrossBeamHs, CrossBeamL;
    }
    public class SUB
    {
        public List<SubStr> SubList;


        public SUB(string subfile)
        {
            SubList = new List<SubStr>();
            string[] altext = File.ReadAllLines(subfile);


            foreach (string item in altext)
            {
                if (item.StartsWith("//")||item=="")
                {
                    continue;
                }
                SubStr pt = new SubStr();
                try
                {
                    string line = item.TrimEnd('\r');
                    line = line.TrimEnd('\t');
                    var xx = Regex.Split(line, @"\s+");
                    pt.typeID = int.Parse(xx[0]);
                    pt.PK = double.Parse(xx[1]);
                    pt.PierL = double.Parse(xx[2]);
                    pt.PierT = double.Parse(xx[3]);

                    pt.PileCapL = double.Parse(xx[4]);
                    pt.PileCapT = double.Parse(xx[5]);
                    pt.PileCapH = double.Parse(xx[6]);

                    pt.Lz = double.Parse(xx[7]);
                    pt.Dz = double.Parse(xx[8]);

                    pt.PileLNum = int.Parse(xx[9]);
                    pt.PileTNum = int.Parse(xx[10]);
                    pt.PileSpaceL = double.Parse(xx[11]);
                    pt.PileSpaceT = double.Parse(xx[12]);

                    pt.CrossBeamTm = double.Parse(xx[13]);
                    pt.CrossBeamTs = double.Parse(xx[14]);
                    pt.CrossBeamHm = double.Parse(xx[15]);
                    pt.CrossBeamHs = double.Parse(xx[16]);
                    pt.CrossBeamL = double.Parse(xx[17]);

                    pt.offsetL = double.Parse(xx[18]);
                    pt.offsetT = double.Parse(xx[19]);
                    pt.offsetH = double.Parse(xx[20]);

                    SubList.Add(pt);
                }
                catch (Exception)
                {
                    throw;
                }

            }
            SubList.Sort((x, y) => x.PK.CompareTo(y.PK));










        }



        public void ToTekla(ref Model myModel,ref DesignLine cl )
        {
            CoordinateSystem global = new CoordinateSystem();
            CoordinateSystem loc;
            Matrix mat;
            foreach (var item in SubList)
            {
                double pk0 = item.PK;
                var ff = cl.pQX.GetCoord(pk0);
                var next = cl.pQX.GetCoord(pk0 + 0.1);
                Vector vv = new Vector(next[0] - ff[0], next[1] - ff[1],0);
                double ang = 90 - vv.GetAngleBetween(new Vector(1, 0, 0)) / Math.PI * 180;

                double x0 = ff[0] * 1000;
                double y0 = ff[1] * 1000;
                double z0 = cl.sQX.GetBG(pk0) * 1000;
                double zPierTop = z0 - item.CrossBeamHm * 1000 + item.offsetH * 1000;
                double zCapTop = (cl.dMX.GetBG(pk0) - 0.5) * 1000;
                double zCapBot = zCapTop - item.PileCapH * 1000;
                double zPileTop;
                double zPileBot;                               

                if (item.typeID==1)
                {
                    // 桥墩
                    string PierStr = string.Format("{0}*{1}", (int)(1000 * item.PierT), (int)(1000 * item.PierL));                    
                    loc = new CoordinateSystem(new Point(x0, y0, z0), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    if (zPierTop > zCapTop)
                    {
                        TeklaTools.CreatBeam(
                            mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, zCapTop - z0)),
                            mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, zPierTop - z0)), PierStr, Position.DepthEnum.MIDDLE, ang);
                    }
                    // 承台
                    string PierCapStr = string.Format("{0}*{1}", (int)(1000 * item.PileCapT), (int)(1000 * item.PileCapL));
                    TeklaTools.CreatBeam(
                        mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, zCapBot - z0)),
                        mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, zCapTop - z0)), PierCapStr, Position.DepthEnum.MIDDLE, ang);

                    // 盖梁
                    string CBstrS = string.Format("OBLVAR_B{0}-{1}-{2}-0", item.CrossBeamHm * 1000, item.CrossBeamHs * 1000, item.CrossBeamL * 1000);
                    string CBstrM = string.Format("{0}*{1}", item.CrossBeamHm * 1000, item.CrossBeamL * 1000);
                    loc = new CoordinateSystem(new Point(x0, y0, z0 + item.offsetH * 1000), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    double xloc = 0 + item.offsetL * 1000;
                    double yloc0 = (-0.5 * item.CrossBeamTm - item.CrossBeamTs) * 1000 + item.offsetT * 1000;
                    double yloc1 = (-0.5 * item.CrossBeamTm) * 1000 + item.offsetT * 1000;
                    double yloc2 = (0.5 * item.CrossBeamTm) * 1000 + item.offsetT * 1000;
                    double yloc3 = (0.5 * item.CrossBeamTm + item.CrossBeamTs) * 1000 + item.offsetT * 1000;

                    TeklaTools.CreatBeam(mat.Transform(new Point(xloc, yloc1, 0)), mat.Transform(new Point(xloc, yloc0, 0)), CBstrS, Position.DepthEnum.BEHIND);
                    TeklaTools.CreatBeam(mat.Transform(new Point(xloc, yloc1, 0)), mat.Transform(new Point(xloc, yloc2, 0)), CBstrM, Position.DepthEnum.BEHIND);
                    TeklaTools.CreatBeam(mat.Transform(new Point(xloc, yloc2, 0)), mat.Transform(new Point(xloc, yloc3, 0)), CBstrS, Position.DepthEnum.BEHIND);

                    // 桩基
                    zPileTop = zCapBot + 100;
                    zPileBot = zPileTop - item.Lz * 1000;
                    loc = new CoordinateSystem(new Point(x0, y0, zPileTop), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    for (int i = 0; i < item.PileLNum; i++)
                    {
                        for (int j = 0; j < item.PileTNum; j++)
                        {
                            double xi = 0 - 0.5 * (item.PileLNum - 1) * item.PileSpaceL * 1000 + i * item.PileSpaceL * 1000 + item.offsetL * 1000;
                            double yi = 0 - 0.5 * (item.PileTNum - 1) * item.PileSpaceT * 1000 + j * item.PileSpaceT * 1000 + item.offsetT * 1000;
                            var f = (int)(item.Dz * 1000);
                            string pilestr = "D" + f.ToString();

                            TeklaTools.CreatBeam(
                                mat.Transform(new Point(xi, yi, -item.Lz * 1000)),
                                mat.Transform(new Point(xi, yi, 0)),
                                pilestr,
                                Position.DepthEnum.MIDDLE
                                );
                        }
                    }
                }
                else if (item.typeID==2)
                {
                    // 桥墩
                    string PierStr = string.Format("D{0}", (int)(1000 * item.PierL));
                    loc = new CoordinateSystem(new Point(x0, y0, z0), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    if (zPierTop > zCapTop)
                    {
                        TeklaTools.CreatBeam(
                            mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000 + 0.5 * item.PileSpaceT * 1000, zCapTop - z0)),
                            mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000 + 0.5 * item.PileSpaceT * 1000, zPierTop - z0)),
                            PierStr, Position.DepthEnum.MIDDLE, 0);
                        TeklaTools.CreatBeam(
                            mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000 - 0.5 * item.PileSpaceT * 1000, zCapTop - z0)),
                            mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000 - 0.5 * item.PileSpaceT * 1000, zPierTop - z0)),
                            PierStr, Position.DepthEnum.MIDDLE, 0);
                    }
                    // 地系梁
                    string PierCapStr = string.Format("{0}*{1}", (int)(1000 * item.PileCapT), (int)(1000 * item.PileCapL));
                    TeklaTools.CreatBeam(
                        mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, zCapBot - z0)),
                        mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, zCapTop - z0)), PierCapStr, Position.DepthEnum.MIDDLE, ang);

                    // 桩基
                    zPileTop = zCapTop;
                    zPileBot = zPileTop - item.Lz * 1000;
                    loc = new CoordinateSystem(new Point(x0, y0, zPileTop), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    for (int i = 0; i < item.PileLNum; i++)
                    {
                        for (int j = 0; j < item.PileTNum; j++)
                        {
                            double xi = 0 - 0.5 * (item.PileLNum - 1) * item.PileSpaceL * 1000 + i * item.PileSpaceL * 1000 + item.offsetL * 1000;
                            double yi = 0 - 0.5 * (item.PileTNum - 1) * item.PileSpaceT * 1000 + j * item.PileSpaceT * 1000 + item.offsetT * 1000;
                            var f = (int)(item.Dz * 1000);
                            string pilestr = "D" + f.ToString();

                            TeklaTools.CreatBeam(
                                mat.Transform(new Point(xi, yi, -item.Lz * 1000)),
                                mat.Transform(new Point(xi, yi, 0)),
                                pilestr,
                                Position.DepthEnum.MIDDLE
                                );
                        }
                    }

                    // 盖梁
                    string CBstrS = string.Format("OBLVAR_B{0}-{1}-{2}-0", item.CrossBeamHm * 1000, item.CrossBeamHs * 1000, item.CrossBeamL * 1000);
                    string CBstrM = string.Format("{0}*{1}", item.CrossBeamHm * 1000, item.CrossBeamL * 1000);
                    loc = new CoordinateSystem(new Point(x0, y0, z0 + item.offsetH * 1000), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    double xloc = 0 + item.offsetL * 1000;
                    double yloc0 = (-0.5 * item.CrossBeamTm - item.CrossBeamTs) * 1000 + item.offsetT * 1000;
                    double yloc1 = (-0.5 * item.CrossBeamTm) * 1000 + item.offsetT * 1000;
                    double yloc2 = (0.5 * item.CrossBeamTm) * 1000 + item.offsetT * 1000;
                    double yloc3 = (0.5 * item.CrossBeamTm + item.CrossBeamTs) * 1000 + item.offsetT * 1000;

                    TeklaTools.CreatBeam(mat.Transform(new Point(xloc, yloc1, 0)), mat.Transform(new Point(xloc, yloc0, 0)), CBstrS, Position.DepthEnum.BEHIND);
                    TeklaTools.CreatBeam(mat.Transform(new Point(xloc, yloc1, 0)), mat.Transform(new Point(xloc, yloc2, 0)), CBstrM, Position.DepthEnum.BEHIND);
                    TeklaTools.CreatBeam(mat.Transform(new Point(xloc, yloc2, 0)), mat.Transform(new Point(xloc, yloc3, 0)), CBstrS, Position.DepthEnum.BEHIND);

                }
                else if (item.typeID==3)
                {
                    zCapTop = item.offsetH*1000;
                    zCapBot = zCapTop - item.PileCapH * 1000;
                    loc = new CoordinateSystem(new Point(x0, y0, zCapTop), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    // 承台
                    string PierCapStr = string.Format("{0}*{1}", (int)(1000 * item.PileCapT), (int)(1000 * item.PileCapL));
                    TeklaTools.CreatBeam(
                        mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, -item.PileCapH*1000)),
                        mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, 0)), PierCapStr, Position.DepthEnum.MIDDLE, ang);

                    // 桩基
                    zPileTop = zCapBot+100;
                    zPileBot = zPileTop - item.Lz * 1000;
                    loc = new CoordinateSystem(new Point(x0, y0, zPileTop), vv, vv.RotByZ(0.5 * Math.PI));
                    mat = MatrixFactory.ByCoordinateSystems(loc, global);
                    for (int i = 0; i < item.PileLNum; i++)
                    {
                        for (int j = 0; j < item.PileTNum; j++)
                        {
                            double xi = 0 - 0.5 * (item.PileLNum - 1) * item.PileSpaceL * 1000 + i * item.PileSpaceL * 1000 + item.offsetL * 1000;
                            double yi = 0 - 0.5 * (item.PileTNum - 1) * item.PileSpaceT * 1000 + j * item.PileSpaceT * 1000 + item.offsetT * 1000;
                            var f = (int)(item.Dz * 1000);
                            string pilestr = "D" + f.ToString();

                            TeklaTools.CreatBeam(
                                mat.Transform(new Point(xi, yi, -item.Lz * 1000)),
                                mat.Transform(new Point(xi, yi, 0)),
                                pilestr,
                                Position.DepthEnum.MIDDLE
                                );
                        }
                    }
                }
            }
        }

        public void ToDXF(ref DWGImage vDrawing, ref DesignLine cl)
        {
            foreach (var item in SubList)
            {
                double pk0 = item.PK;

                double x0 = pk0 * 1000;                
                double z0 = cl.sQX.GetBG(pk0) * 1000;
                double zPierTop = z0 - item.CrossBeamHm * 1000 + item.offsetH * 1000;
                double zCapTop = (cl.dMX.GetBG(pk0) - 0.5) * 1000;
                double zCapBot = zCapTop - item.PileCapH * 1000;
                double zPileTop;
                double zPileBot;
                DPoint pt1, pt2;

                if (item.typeID==1)
                {
                    //桩基
                    zPileTop = zCapBot + 100;
                    zPileBot = zPileTop - item.Lz * 1000;
                    for (int i = 0; i < item.PileLNum; i++)
                    {
                        double xi = 0 - 0.5 * (item.PileLNum - 1) * item.PileSpaceL * 1000 + i * item.PileSpaceL * 1000 + item.offsetL * 1000;
                        pt1 = new DPoint(x0 + xi - 0.5 * item.Dz * 1000, zPileBot, 0);
                        pt2 = new DPoint(x0 + xi + 0.5 * item.Dz * 1000, zPileTop, 0);
                        NetCADTool.AddRect(ref vDrawing, pt1, pt2);
                    }

                    // 承台
                    pt1 = new DPoint(x0 - 0.5 * item.PileCapL * 1000, zCapBot, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.PileCapL * 1000, zCapTop, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);


                    // 桥墩
                    pt1 = new DPoint(x0 - 0.5 * item.PierL * 1000, zCapTop, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.PierL * 1000, zPierTop, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);

                    // 盖梁
                    pt1 = new DPoint(x0 - 0.5 * item.CrossBeamL * 1000, zPierTop, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.CrossBeamL * 1000, zPierTop + item.CrossBeamHm * 1000, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);

                    pt1 = new DPoint(x0 - 0.5 * item.CrossBeamL * 1000, zPierTop + (item.CrossBeamHm - item.CrossBeamHs) * 1000, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.CrossBeamL * 1000, zPierTop + item.CrossBeamHm * 1000, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);
                }
                else if(item.typeID==2)
                {
                    //桩基
                    zPileTop = zCapBot + 100;
                    zPileBot = zPileTop - item.Lz * 1000;
                    for (int i = 0; i < item.PileLNum; i++)
                    {
                        double xi = 0 - 0.5 * (item.PileLNum - 1) * item.PileSpaceL * 1000 + i * item.PileSpaceL * 1000 + item.offsetL * 1000;
                        pt1 = new DPoint(x0 + xi - 0.5 * item.Dz * 1000, zPileBot, 0);
                        pt2 = new DPoint(x0 + xi + 0.5 * item.Dz * 1000, zPileTop, 0);
                        NetCADTool.AddRect(ref vDrawing, pt1, pt2);
                    }

                    // 桥墩
                    pt1 = new DPoint(x0 - 0.5 * item.PierL * 1000, zPileTop, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.PierL * 1000, zPierTop, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);

                    // 盖梁
                    pt1 = new DPoint(x0 - 0.5 * item.CrossBeamL * 1000, zPierTop, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.CrossBeamL * 1000, zPierTop+item.CrossBeamHm*1000, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);

                    pt1 = new DPoint(x0 - 0.5 * item.CrossBeamL * 1000, zPierTop+ (item.CrossBeamHm - item.CrossBeamHs)*1000, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.CrossBeamL * 1000, zPierTop + item.CrossBeamHm * 1000, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);

                }
                else if (item.typeID == 3)
                {
                    //桩基
                    zCapTop = item.offsetH*1000;
                    zCapBot = zCapTop - item.PileCapH * 1000;
                    zPileTop = zCapBot + 100;
                    zPileBot = zPileTop - item.Lz * 1000;
                    for (int i = 0; i < item.PileLNum; i++)
                    {
                        double xi = 0 - 0.5 * (item.PileLNum - 1) * item.PileSpaceL * 1000 + i * item.PileSpaceL * 1000 + item.offsetL * 1000;
                        pt1 = new DPoint(x0 + xi - 0.5 * item.Dz * 1000, zPileBot, 0);
                        pt2 = new DPoint(x0 + xi + 0.5 * item.Dz * 1000, zPileTop, 0);
                        NetCADTool.AddRect(ref vDrawing, pt1, pt2);
                    }
                    // 承台
                    pt1 = new DPoint(x0 - 0.5 * item.PileCapL * 1000, zCapBot, 0);
                    pt2 = new DPoint(x0 + 0.5 * item.PileCapL * 1000, zCapTop, 0);
                    NetCADTool.AddRect(ref vDrawing, pt1, pt2);
                }



            }
            




        }
    }
}
