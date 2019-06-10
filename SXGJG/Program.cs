using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Colors;
using QRCoder;

[assembly: CommandClass(typeof(SmartBridgeBuilder.SXGJG.Program))]

namespace SmartBridgeBuilder.SXGJG
{
    public static class Program
    {
        static void Main(string[] args)
        {
            SXBridge bill = new SXBridge();
            string fjJD = @"F:\FJ\15 厂化图\Data\FJ.JD";
            using (StreamWriter file = new StreamWriter(Path.Combine(Path.GetDirectoryName(fjJD), "UCH.txt"), false))
            {

                foreach (int  y0 in new[] { -13250,-3750,3750,13250})
                {
                    if (y0<0)
                    {
                        foreach (int x0 in bill.UCHRight)
                        {
                            double hp = bill.GetHP(x0, true);
                            double dz = (y0 + 1000) * hp+68.75;
                            double z0 = bill.GetUCH(x0) + dz;                                                      
                            string line = string.Format("{0:F10},{1:F10},{2:F10}", x0, y0, z0);
                            file.WriteLine(line);
                        }
                    }
                    else
                    {
                        foreach (int x0 in bill.UCHLeft)
                        {
                            double hp = bill.GetHP(x0, false);
                            double dz =(y0-1000)*hp+68.75;
                            double z0 = bill.GetUCH(x0) + dz;
                            string line = string.Format("{0:F10},{1:F10},{2:F10}", x0, y0, z0);
                            file.WriteLine(line);
                        }
                    }                    
                }                
            }

            using (StreamWriter file = new StreamWriter(Path.Combine(Path.GetDirectoryName(fjJD), "LCH.txt"), false))
            {

                foreach (int y0 in new[] { -13250, -3750, 3750, 13250 })
                {
                    if (y0 < 0)
                    {
                        foreach (int x0 in bill.LCHRight)
                        {   
                            double z0 = bill.GetLCH(x0);
                            string line = string.Format("{0:F10},{1:F10},{2:F10}", x0, y0, z0);
                            file.WriteLine(line);
                        }
                    }
                    else
                    {
                        foreach (int x0 in bill.LCHLeft)
                        {        
                            double z0 = bill.GetLCH(x0);
                            string line = string.Format("{0:F10},{1:F10},{2:F10}", x0, y0, z0);
                            file.WriteLine(line);
                        }
                    }
                }
            }





        }

        [CommandMethod("sxplotC")]
        public static void DrawBridgeC()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Guid.NewGuid().ToString(), QRCodeGenerator.ECCLevel.M,true,true,QRCodeGenerator.EciMode.Utf8,5);


            
            QRCode qrCode = new QRCode(qrCodeData);



        }


        [CommandMethod("sxplotA")]
        public static void DrawBridgeA()
        {
            string outfilePath = @"D:\";

            //  CAD 基本指针
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            SXBridge bill = new SXBridge();
            Polyline3d uchline,lchline,webline;
            Point3d pt;


            Matrix3d loc2catia = Matrix3d.AlignCoordinateSystem(
                new Point3d(0, 0, 0), 
                new Vector3d(new double[] { 1, 0, 0 }), 
                new Vector3d(new double[] { 0, 1, 0 }), 
                new Vector3d(new double[] { 0, 0, 1 }),
                new Point3d(291574.74944347, 92740.07344521, -15290.48119669),
                new Vector3d(new double[] { 6340.39166433, 653.06886634, 0 }).GetNormal(),
                new Vector3d(new double[] { -1735.71486129, 16851.38062064, 0 }).GetNormal(),
                new Vector3d(new double[] { 0, 0, 1 })
                );


            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                foreach (int y0 in new[] { -13250, -3750, 3750, 13250 })
                {
                    uchline = new Polyline3d();
                    uchline.ColorIndex = 1;
                    modelSpace.AppendEntity(uchline);
                    if (y0 < 0)
                    {
                        foreach (int x0 in bill.UCHRight)
                        {
                            double hp = bill.GetHP(x0, true);
                            double dz = (y0 + 1000) * hp + 68.75;
                            double z0 = bill.GetUCH(x0) + dz;
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            uchline.AppendVertex(new PolylineVertex3d(pt));
                        }
                    }
                    else
                    {
                        foreach (int x0 in bill.UCHLeft)
                        {
                            double hp = bill.GetHP(x0, false);
                            double dz = (y0 - 1000) * hp + 68.75;
                            double z0 = bill.GetUCH(x0) + dz;
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            uchline.AppendVertex(new PolylineVertex3d(pt));
                        }
                    }                    
                    tr.AddNewlyCreatedDBObject(uchline, true);
                    tr.Commit();
                    //OutPut(uchline,Path.Combine(outfilePath, string.Format("uch{0}", y0)));
                }

                foreach (int y0 in new[] { -13250, -3750, 3750, 13250 })
                {
                    lchline = new Polyline3d();
                    lchline.ColorIndex = 2;
                    modelSpace.AppendEntity(lchline);

                    if (y0 < 0)
                    {
                        foreach (int x0 in bill.LCHRight)
                        {
                            double z0 = bill.GetLCH(x0);
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            lchline.AppendVertex(new PolylineVertex3d(pt));
                        }
                    }
                    else
                    {
                        foreach (int x0 in bill.LCHLeft)
                        {
                            double z0 = bill.GetLCH(x0);
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            lchline.AppendVertex(new PolylineVertex3d(pt));
                        }
                    }
                    tr.AddNewlyCreatedDBObject(lchline, true);
                }
                // 腹杆
                foreach (int y0 in new[] { -13250, -3750, 3750, 13250 })
                {
                    webline = new Polyline3d();
                    webline.ColorIndex = 3;
                    modelSpace.AppendEntity(webline);                    
                    if (y0 < 0)
                    {
                        int x0 = 0;
                        double hp = bill.GetHP(x0, true);
                        double dz = (y0 + 1000) * hp + 68.75;
                        double z0 = bill.GetUCH(x0) + dz;
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                        for (int i = 0; i < 95; i++)
                        {
                            if (i%2==0)
                            {
                                x0 = bill.LCHRight[i];
                                z0 = bill.GetLCH(x0);
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                            else
                            {
                                x0 = bill.UCHRight[i];
                                hp = bill.GetHP(x0, true);
                                dz = (y0 + 1000) * hp + 68.75;
                                z0 = bill.GetUCH(x0) + dz;
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                        }                        
                        x0 = bill.UCHRight[94];
                        hp = bill.GetHP(x0, true);
                        dz = (y0 + 1000) * hp + 68.75;
                        z0 = bill.GetUCH(x0) + dz;
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                    }
                    else
                    {
                        int x0 = 0;
                        double hp = bill.GetHP(x0, false);
                        double dz = (y0 - 1000) * hp + 68.75;
                        double z0 = bill.GetUCH(x0) + dz;
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                        for (int i = 0; i < 95; i++)
                        {
                            if (i % 2 == 0)
                            {
                                x0 = bill.LCHLeft[i];
                                z0 = bill.GetLCH(x0);
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                            else
                            {
                                x0 = bill.UCHLeft[i];
                                hp = bill.GetHP(x0, false);
                                dz = (y0 - 1000) * hp + 68.75;
                                z0 = bill.GetUCH(x0) + dz;
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                        }
                        x0 = bill.UCHLeft[94];
                        hp = bill.GetHP(x0, false);
                        dz = (y0 - 1000) * hp + 68.75;
                        z0 = bill.GetUCH(x0) + dz;
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                    }
                    tr.AddNewlyCreatedDBObject(webline, true);
                }
                tr.Commit();
            }
        }

        private static void OutPut(Polyline3d uchline, string v)
        {
            DBObjectCollection col = new DBObjectCollection();
            col.Clear();
            uchline.Explode(col);
            List<Line> linelist = new List<Line>();
            foreach (DBObject item in col)
            {
                linelist.Add((Line)item);
            }
            linelist.Sort((a, b) => (a.GetMidPoint3d().X.CompareTo(b.GetMidPoint3d().X)));

            using (StreamWriter sw = new StreamWriter(v, false))
            {
                foreach (var item in linelist)
                {
                    sw.WriteLine(item.Length);
                }
            }
        }

        [CommandMethod("sxplotb")]
        public static void DrawBridgeB()
        {
            //  CAD 基本指针
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            SXBridge bill = new SXBridge();
            Polyline3d uchline, lchline, webline;
            Point3d pt;
            int cc;

            Matrix3d loc2catia = Matrix3d.AlignCoordinateSystem(
                new Point3d(0, 0, 0),
                new Vector3d(new double[] { 1, 0, 0 }),
                new Vector3d(new double[] { 0, 1, 0 }),
                new Vector3d(new double[] { 0, 0, 1 }),
                new Point3d(291574.74944347, 92740.07344521, -15290.48119669),
                new Vector3d(new double[] { 6340.39166433, 653.06886634, 0 }).GetNormal(),
                new Vector3d(new double[] { -1735.71486129, 16851.38062064, 0 }).GetNormal(),
                new Vector3d(new double[] { 0, 0, 1 })
                );


            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                //上弦
                foreach (int y0 in new[] { -13250, -3750, 3750, 13250 })
                {
                    uchline = new Polyline3d();
                    uchline.ColorIndex = 1;
                    modelSpace.AppendEntity(uchline);
                    if (y0 < 0)
                    {
                        cc = 0;
                        foreach (int x0 in bill.UCHRight)
                        {
                            double hp = bill.GetHP(x0, true);
                            double dz = (y0 + 1000) * hp + 68.75;
                            double z0 = bill.GetUCH(x0) + dz+bill.UCHRightPC[cc];
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            uchline.AppendVertex(new PolylineVertex3d(pt));
                            cc += 1;
                        }
                    }
                    else
                    {
                        cc = 0;
                        foreach (int x0 in bill.UCHLeft)
                        {
                            double hp = bill.GetHP(x0, false);
                            double dz = (y0 - 1000) * hp + 68.75;
                            double z0 = bill.GetUCH(x0) + dz+bill.UCHLeftPC[cc];
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            uchline.AppendVertex(new PolylineVertex3d(pt));
                            cc += 1;
                        }
                    }
                    tr.AddNewlyCreatedDBObject(uchline, true);
                }
                // 下弦
                foreach (int y0 in new[] { -13250, -3750, 3750, 13250 })
                {
                    lchline = new Polyline3d();
                    lchline.ColorIndex = 2;
                    modelSpace.AppendEntity(lchline);

                    if (y0 < 0)
                    {
                        cc = 0;
                        foreach (int x0 in bill.LCHRight)
                        {
                            double z0 = bill.GetLCH(x0)+bill.LCHRightPC[cc];
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            lchline.AppendVertex(new PolylineVertex3d(pt));
                            cc += 1;
                        }
                    }
                    else
                    {
                        cc = 0;
                        foreach (int x0 in bill.LCHLeft)
                        {
                            double z0 = bill.GetLCH(x0)+bill.LCHLeftPC[cc];
                            pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                            lchline.AppendVertex(new PolylineVertex3d(pt));
                            cc += 1;
                        }
                    }
                    tr.AddNewlyCreatedDBObject(lchline, true);
                }
                // 腹杆
                foreach (int y0 in new[] { -13250, -3750, 3750, 13250 })
                {
                    webline = new Polyline3d();
                    webline.ColorIndex = 3;
                    modelSpace.AppendEntity(webline);
                    if (y0 < 0)
                    {
                        int x0 = 0;
                        double hp = bill.GetHP(x0, true);
                        double dz = (y0 + 1000) * hp + 68.75;
                        double z0 = bill.GetUCH(x0) + dz+bill.UCHRightPC[0];
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                        for (int i = 0; i < 95; i++)
                        {
                            if (i % 2 == 0)
                            {
                                x0 = bill.LCHRight[i];
                                z0 = bill.GetLCH(x0)+bill.LCHRightPC[i];
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                            else
                            {
                                x0 = bill.UCHRight[i];
                                hp = bill.GetHP(x0, true);
                                dz = (y0 + 1000) * hp + 68.75;
                                z0 = bill.GetUCH(x0) + dz+bill.UCHRightPC[i];
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                        }
                        x0 = bill.UCHRight[94];
                        hp = bill.GetHP(x0, true);
                        dz = (y0 + 1000) * hp + 68.75;
                        z0 = bill.GetUCH(x0) + dz + bill.UCHRightPC[94];
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                    }
                    else
                    {
                        int x0 = 0;
                        double hp = bill.GetHP(x0, false);
                        double dz = (y0 - 1000) * hp + 68.75;
                        double z0 = bill.GetUCH(x0) + dz+bill.UCHLeftPC[0];
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                        for (int i = 0; i < 95; i++)
                        {
                            if (i % 2 == 0)
                            {
                                x0 = bill.LCHLeft[i];
                                z0 = bill.GetLCH(x0)+bill.LCHLeftPC[i];
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                            else
                            {
                                x0 = bill.UCHLeft[i];
                                hp = bill.GetHP(x0, false);
                                dz = (y0 - 1000) * hp + 68.75;
                                z0 = bill.GetUCH(x0) + dz+bill.UCHLeftPC[i];
                                pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                                webline.AppendVertex(new PolylineVertex3d(pt));
                            }
                        }
                        x0 = bill.UCHLeft[94];
                        hp = bill.GetHP(x0, false);
                        dz = (y0 - 1000) * hp + 68.75;
                        z0 = bill.GetUCH(x0) + dz+bill.UCHLeftPC[94];
                        pt = new Point3d(x0, y0, z0).TransformBy(loc2catia);
                        webline.AppendVertex(new PolylineVertex3d(pt));
                    }
                    tr.AddNewlyCreatedDBObject(webline, true);
                }
                tr.Commit();
            }
        }





    }
}
