using netDxf;
using SmartBridgeBuilder.RoadDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using CoordinateSystem = Tekla.Structures.Geometry3d.CoordinateSystem;
using Tekla.Structures.Catalogs;
using netDxf.Entities;
using Point = Tekla.Structures.Geometry3d.Point;
using CADImport.DWG;
using CADImport;
using SmartBridgeBuilder.Extension;

namespace SmartBridgeBuilder.Structure
{
    public class ConcretBeam
    {
        DxfDocument StartSection;
        DxfDocument EndSection;
        string startSecFile, endSecFile;

        public ConcretBeam(string dxffilepath)
        {
            StartSection = DxfDocument.Load(dxffilepath);
            EndSection = null;
            startSecFile = dxffilepath;
        }

        public ConcretBeam(string startdxf,string enddxf)
        {
            StartSection = DxfDocument.Load(startdxf);
            EndSection = DxfDocument.Load(enddxf);
            endSecFile = enddxf;
            startSecFile = startdxf;
        }

        public void ToDXF(ref DWGImage vDrawing, ref DesignLine cl, int startpk, int endpk, int dpk)
        {
            double pk0 = startpk;
            double pk1;
            double beamh = 4000;


             


            DPoint C0, C1,C2,C3;

            while (pk0 < endpk)
            {
                pk1 = pk0 + dpk;
                if (pk1 >= endpk)
                {
                    pk1 = endpk;
                }


                C0 = new DPoint(pk0 * 1000, cl.sQX.GetBG(pk0) * 1000, 0);
                C1 = new DPoint(pk1 * 1000, cl.sQX.GetBG(pk1) * 1000, 0);
                C2 = new DPoint(pk1 * 1000, cl.sQX.GetBG(pk1) * 1000 - beamh, 0);
                C3 = new DPoint(pk0 * 1000, cl.sQX.GetBG(pk0) * 1000 - beamh, 0);


                //NetCADTool.AddRect(ref vDrawing, C0, C1);
                NetCADTool.AddPolygen(ref vDrawing, new DPoint[] { C0, C1,C2,C3 },"细线");

                pk0 = pk1;
            }

        }

        public void ToTekla(ref Model myModel, ref DesignLine cl, double startpk, double endpk,double dpk)
        {
            double pk0 = startpk;
            double pk1;
            Brep Beam;
            Point C0, C1;
            string shapeItemString;
            if (EndSection==null)
            {
                shapeItemString = AddNewShapeItem(dpk * 1000);
            }
            else
            {
                shapeItemString = AddNewShapeItem2((endpk-startpk)*1000,10);
            }
            int num = (int)((endpk - startpk) / dpk);
            string shapeItemStringSide = AddNewShapeItem(((endpk - startpk) - dpk * num) * 1000);
            string SIname;

            while (pk0 < endpk)
            {
                pk1 = pk0 + dpk;
                if (pk1>=endpk)
                {
                    pk1 = endpk;
                    SIname = shapeItemStringSide;
                }
                else
                {
                    SIname = shapeItemString;
                }
                C0 = new Point(cl.pQX.GetCoord(pk0)[0] * 1000, cl.pQX.GetCoord(pk0)[1] * 1000, cl.sQX.GetBG(pk0) * 1000);
                C1 = new Point(cl.pQX.GetCoord(pk1)[0] * 1000, cl.pQX.GetCoord(pk1)[1] * 1000, cl.sQX.GetBG(pk1) * 1000);

                Beam = new Brep()
                {
                    StartPoint = C0,
                    EndPoint = C1,
                    Profile = { ProfileString = SIname },                    
                };
                Beam.Position.Plane = Position.PlaneEnum.MIDDLE;
                Beam.Position.Depth = Position.DepthEnum.MIDDLE;

                Beam.Insert();

                pk0 = pk1;
            }


        }


        private string AddNewShapeItem2(double LengthInMM,int NumSegment)
        {

            string ShapeName = Guid.NewGuid().ToString();

            double Det = LengthInMM / NumSegment;

            List<Vector> vertexlist = new List<Vector>();
            List<int[]> outerWires = new List<int[]>();
            var innerWires = new Dictionary<int, int[][]> { };

            LwPolyline outplst = new netDxf.Entities.LwPolyline();
            LwPolyline inplst = new netDxf.Entities.LwPolyline();
            LwPolyline outpled = new netDxf.Entities.LwPolyline();
            LwPolyline inpled = new netDxf.Entities.LwPolyline();

            CoordinateSystem curCS = new CoordinateSystem();
            CoordinateSystem cadCS = new CoordinateSystem(new Point(0, 0, 0), new Vector(0, -1, 0), new Vector(0, 0, 1));
            Matrix CurtoC0 = MatrixFactory.ByCoordinateSystems(cadCS, curCS);

            foreach (var a in StartSection.LwPolylines)
            {
                if (a.Layer.Name.Contains("out"))
                {
                    outplst = a;
                }
                if (a.Layer.Name.Contains("in"))
                {
                    inplst=a;
                }
            }
            foreach (var a in EndSection.LwPolylines)
            {
                if (a.Layer.Name.Contains("out"))
                {
                    outpled = a;
                }
                if (a.Layer.Name.Contains("in"))
                {
                    inpled = a;
                }
            }


            List<int> key = new List<int>();

            for (int j = 0; j < NumSegment+1; j++)
            {
                double xf = j / NumSegment;

                for (int k = 0; k < outplst.Vertexes.Count; k++)
                {
                    LwPolylineVertex a = ChaZhi(outplst.Vertexes[k], outpled.Vertexes[k],xf);
                    vertexlist.Add(new Vector(CurtoC0.Transform(new Point(a.Position.X, a.Position.Y))));
                }
                key.Add(vertexlist.Count);






            }



            var item = inplst;
            foreach (var pt in item.Vertexes)
            {
                vertexlist.Add(new Vector(CurtoC0.Transform(new Point(pt.Position.X, pt.Position.Y))));
            }
            key.Add(vertexlist.Count);
            item = inpled;
            foreach (var pt in item.Vertexes)
            {
                vertexlist.Add(new Vector(CurtoC0.Transform(new Point(pt.Position.X, pt.Position.Y, -LengthInMM))));
            }
            key.Add(vertexlist.Count);


            //var loop = Enumerable.Range(0, key[0]).ToArray();
            //outerWires.Add(loop);
            //loop = Enumerable.Range(key[0], key[0]).ToArray();
            //outerWires.Add(loop);
            
            
            ////四周四边形
            //for (int i = 0; i < outplst.Vertexes.Count; i++)
            //{
            //    int now = i;
            //    int next = now == outplst.Vertexes.Count - 1 ? 0 : i + 1;
            //    int before = now == 0 ? outplst.Vertexes.Count : i - 1;
            //    loop = new[] { now, now + key[0], next + key[0], next };
            //    outerWires.Add(loop);
            //}
            //for (int j = 0; j < inpls.Count; j++)
            //{
            //    var item = inpls[j];
            //    for (int i = 0; i < item.Vertexes.Count; i++)
            //    {
            //        int now = i;
            //        int next = now == item.Vertexes.Count - 1 ? 0 : i + 1;
            //        int before = now == 0 ? item.Vertexes.Count : i - 1;
            //        int offset = key[2 * j + 2] - key[2 * j + 1];
            //        loop = new[] { now + key[2 * j + 1], now + offset + key[2 * j + 1], next + offset + key[2 * j + 1], next + key[2 * j + 1] };
            //        outerWires.Add(loop);
            //    }
            //}

            //if (inpls.Count == 1)
            //{
            //    loop = Enumerable.Range(key[1], key[2] - key[1]).ToArray();
            //    innerWires.Add(0, new[] { loop });
            //    loop = Enumerable.Range(key[2], key[3] - key[2]).ToArray();
            //    innerWires.Add(1, new[] { loop });
            //}



            ////loop = Enumerable.Range(key[3], key[4] - key[3]).ToArray();
            ////innerWires.Add(0, new[] { loop });
            ////loop = Enumerable.Range(key[4], key[5] - key[4]).ToArray();
            ////innerWires.Add(1, new[] { loop });


            //var fbrep = new FacetedBrep(vertexlist.ToArray(), outerWires.ToArray(), innerWires);
            //var shapeItem = new ShapeItem
            //{
            //    Name = ShapeName,
            //    ShapeFacetedBrep = fbrep,
            //    UpAxis = ShapeUpAxis.Z_Axis
            //};
            //shapeItem.Insert();
            return ShapeName;
        }



        private LwPolylineVertex ChaZhi(LwPolylineVertex ni, LwPolylineVertex nj,double xf, int v=2)
        {
            double dx = nj.Position.X - ni.Position.X;
            double dy = nj.Position.Y - ni.Position.Y;

            return new LwPolylineVertex(dx * xf + ni.Position.X, dy * Math.Pow(xf,v) + ni.Position.Y);
            
        }

        private string AddNewShapeItem(double LenInMM)
        {
            string ShapeName = Guid.NewGuid().ToString();
                        
            List<Vector> vertexlist = new List<Vector>();
            List<int[]> outerWires = new List<int[]>();
            var innerWires = new Dictionary<int, int[][]> { };

            netDxf.Entities.LwPolyline outpl=new netDxf.Entities.LwPolyline();
            List<netDxf.Entities.LwPolyline> inpls=new List<netDxf.Entities.LwPolyline>();


            CoordinateSystem curCS = new CoordinateSystem();
            CoordinateSystem cadCS=new CoordinateSystem(new Point(0,0,0),new Vector(0,-1,0),new Vector(0,0,1));
            Matrix CurtoC0=MatrixFactory.ByCoordinateSystems(cadCS, curCS);

            foreach (var item in StartSection.LwPolylines)
            {
                if (item.Layer.Name.Contains("OUT")|| item.Layer.Name.Contains("out"))
                {
                    outpl = item;
                }
                if (item.Layer.Name.Contains("IN")|| item.Layer.Name.Contains("in"))
                {
                    inpls.Add(item);
                }
            }

            List<int> key = new List<int>();
            
            foreach (var item in outpl.Vertexes)
            {
                vertexlist.Add(new Vector(CurtoC0.Transform(new Point(item.Position.X, item.Position.Y))));
            }
            key.Add(vertexlist.Count);

            foreach (var item in outpl.Vertexes)
            {
                vertexlist.Add(new Vector(CurtoC0.Transform(new Point(item.Position.X, item.Position.Y,-LenInMM))));
            }
            key.Add(vertexlist.Count);
            foreach (var item in inpls)
            {
                foreach (var pt in item.Vertexes)
                {
                    vertexlist.Add(new Vector(CurtoC0.Transform(new Point(pt.Position.X, pt.Position.Y))));
                }
                key.Add(vertexlist.Count);
                foreach (var pt in item.Vertexes)
                {
                    vertexlist.Add(new Vector(CurtoC0.Transform(new Point(pt.Position.X, pt.Position.Y,-LenInMM))));
                }
                key.Add(vertexlist.Count);
            }
            
            var loop = Enumerable.Range(0, key[0]).ToArray();
            outerWires.Add(loop);
            loop = Enumerable.Range(key[0],key[0]).ToArray();
            outerWires.Add(loop);
            //四周四边形
            for (int i = 0; i < outpl.Vertexes.Count; i++)
            {
                int now = i;
                int next = now == outpl.Vertexes.Count - 1 ? 0 : i + 1;
                int before = now == 0 ? outpl.Vertexes.Count : i - 1;
                loop = new[] { now, now + key[0],next+key[0],next };
                outerWires.Add(loop);
            }
            for (int j = 0; j < inpls.Count; j++)
            {
                var item = inpls[j];
                for (int i = 0; i < item.Vertexes.Count; i++)
                {
                    int now = i;
                    int next = now == item.Vertexes.Count - 1 ? 0 : i + 1;
                    int before = now == 0 ? item.Vertexes.Count : i - 1;
                    int offset = key[2 * j + 2] - key[2 * j + 1];
                    loop = new[] { now + key[2 * j + 1], now + offset + key[2 * j + 1], next + offset + key[2 * j + 1], next + key[2 * j + 1] };
                    outerWires.Add(loop);
                }
            }

            if (inpls.Count==1)
            {
                loop = Enumerable.Range(key[1], key[2] - key[1]).ToArray();
                innerWires.Add(0, new[] { loop });
                loop = Enumerable.Range(key[2], key[3] - key[2]).ToArray();
                innerWires.Add(1, new[] { loop });
            }



            //loop = Enumerable.Range(key[3], key[4] - key[3]).ToArray();
            //innerWires.Add(0, new[] { loop });
            //loop = Enumerable.Range(key[4], key[5] - key[4]).ToArray();
            //innerWires.Add(1, new[] { loop });


            var fbrep = new FacetedBrep(vertexlist.ToArray(), outerWires.ToArray(), innerWires);
            var shapeItem = new ShapeItem
            {
                Name = ShapeName,
                ShapeFacetedBrep = fbrep,
                UpAxis = ShapeUpAxis.Z_Axis
            };
            shapeItem.Insert();
            return ShapeName;            
        }
    }
}
