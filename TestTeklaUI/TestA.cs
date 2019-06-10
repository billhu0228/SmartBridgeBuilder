using netDxf;
using netDxf.Entities;
using netDxf.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.CatalogInternal;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Point = Tekla.Structures.Geometry3d.Point;

namespace TestTeklaUI
{
    static class TestA
    {
        public static void X()
        {
            // your dxf file name
            string file = @"G:\20190411 - CCTV\江阴大桥\BT.dxf";

            DxfDocument loaded = DxfDocument.Load(file);

            CrossSection cs=new CrossSection();
            
            CrossSectionPoint pt = new CrossSectionPoint();
            pt.X = 1;
            pt.Y = 1;
            pt.Z = 0;
            pt.Chamfer = new Chamfer();

            cs.Select();

            cs.OuterSurface.Add(pt);

            ;
        }
        public static bool Example11()
        {
            bool Result = false;

            CatalogHandler CatalogHandler = new CatalogHandler();

            if (CatalogHandler.GetConnectionStatus())
            {
                ShapeItemEnumerator ShapeItemEnumerator = CatalogHandler.GetShapeItems();

                while (ShapeItemEnumerator.MoveNext())
                {
                    ShapeItem ShapeItem = ShapeItemEnumerator.Current as ShapeItem;
                    ShapeItem.Select("Concrete_Default");
                    var ff = ShapeItem.ShapeFacetedBrep;



                    if (ShapeItem.Name == "Concrete_Default")
                    {
                        Result = true;
                        //break;
                    }
                }
            }

            return Result;
        }

        public static void Example3()
        {
            var vertex = new[]
            {
            new Vector(0.0, 0.0, 0.0), // 0
            new Vector(300.0, 0.0, 0.0), // 1
            new Vector(300.0, 700.0, 0.0), // 2
            new Vector(0.0, 700.0, 0.0), // 3
            new Vector(300.0, 700.0, 0.0), // 4
            new Vector(300.0, 700.0, 2000.0), // 5
            new Vector(0.0, 700.0, 2000.0), // 6
            new Vector(100.0, 100.0, 0.0), // 7
            new Vector(200.0, 100.0, 0.0), // 8
            new Vector(200.0, 200.0, 0.0), // 9
            new Vector(100.0, 200.0, 0.0) // 10
        };
            var outerWires = new[]
            {
            new[] { 0, 1, 2, 3 },
            new[] { 3, 4, 5, 6 }
        };
            var innerWires = new Dictionary<int, int[][]>
        {
            { 0, new[] { new[] { 10, 9, 8, 7 } } }
        };

            var brep = new FacetedBrep(vertex, outerWires, innerWires);
            Console.WriteLine("This BREP has {0} faces", brep.Faces.Count);
        }

        public static void Example1(string ff)
        {
            Point point = new Point(0, 0, 0);
            Point point2 = new Point(1000, 0, 0);
            Brep brep = new Brep();
            brep.StartPoint = point;
            brep.EndPoint = point2;
            brep.Profile = new Profile { ProfileString = ff };
            bool result = brep.Insert();
        }

        public static void InsertSimpleCube(string Cname)
        {
            var vertices = new[]
            {
            new Vector(  0.0,   0.0,   0.0), // 0
            new Vector(300.0,   0.0,   0.0), // 1
            new Vector(300.0, 300.0,   0.0), // 2
            new Vector(  0.0, 300.0,   0.0), // 3
            new Vector(  0.0,   0.0, 300.0), // 4
            new Vector(300.0,   0.0, 300.0), // 5
            new Vector(300.0, 300.0, 300.0), // 6
            new Vector(  0.0, 300.0, 300.0), // 7
        };
            var outloop = new[] { new[] { 0, 3, 2, 1 },
                      new[] { 0, 1, 5, 4 },
                      new[] { 1, 2, 6, 5 },
                      new[] { 2, 3, 7, 6 },
                      new[] { 3, 0, 4, 7 },
                      new[] { 4, 5, 6, 7 }};

            var innerLoop = new Dictionary<int, int[][]> { };
            {
            };

            var brep = new FacetedBrep(vertices, outloop, innerLoop);
            var shapeItem = new ShapeItem
            {
                Name = Cname,
                ShapeFacetedBrep = brep,
                UpAxis = ShapeUpAxis.Z_Axis


            };
            var result = shapeItem.Insert();
        }

        public static void myExBrep()
        {
            var vertices = new[]
            {
                new Vector(0,-100,-100), // 0
                new Vector(0,100, -100), // 1
                new Vector(0,  100,100), // 2
                new Vector(0,   -100,100), // 3
                new Vector(500,-100,-100), // 0
                new Vector(500, 100,-100), // 1
                new Vector(500, 100, 100), // 2
                new Vector(500,-100, 100), // 3
            };
            var outloop = new[] { new[] { 0, 3, 2, 1 },
                                  new[] { 0, 1, 5, 4 },
                                  new[] { 1, 2, 6, 5 },
                                  new[] { 2, 3, 7, 6 },
                                  new[] { 3, 0, 4, 7 },
                                  new[] { 4, 5, 6, 7 }};

            var innerLoop = new Dictionary<int, int[][]>
            {
            };

            var brep = new FacetedBrep(vertices, outloop, innerLoop);
            var shapeItem = new ShapeItem
            {
                Name = "TestBill",
                ShapeFacetedBrep = brep,
                UpAxis = ShapeUpAxis.Z_Axis
            };

            shapeItem.Insert();

            var inmodel = new Brep()
            {
                StartPoint = new Point(0, 0, 0),
                EndPoint = new Point(100, 100, 0),
                Profile = { ProfileString = "TestBill" },
            };
            inmodel.Insert();
        }

        public static void Example2()
        {
            /* One edge inside the hole and one on the outer surface of the box will be hidden. */

            var vertices = new[]
            {
         /* The main cube vertices. Notice that all vertices are triplicated, because each is
            present on three different faces. Thus we get three different vertex normals for each physical
            corner too. */

         new Vector(   0.0,    0.0,    0.0 ), // 0
         new Vector( 500.0,    0.0,    0.0 ), // 1
         new Vector( 500.0,  500.0,    0.0 ), // 2
         new Vector(   0.0,  500.0,    0.0 ), // 3
         new Vector(   0.0,    0.0,  500.0 ), // 4
         new Vector( 500.0,    0.0,  500.0 ), // 5
         new Vector( 500.0,  500.0,  500.0 ), // 6
         new Vector(   0.0,  500.0,  500.0 ), // 7

         new Vector(   0.0,    0.0,    0.0 ), // 8
         new Vector( 500.0,    0.0,    0.0 ), // 9
         new Vector( 500.0,  500.0,    0.0 ), // 10
         new Vector(   0.0,  500.0,    0.0 ), // 11
         new Vector(   0.0,    0.0,  500.0 ), // 12
         new Vector( 500.0,    0.0,  500.0 ), // 13
         new Vector( 500.0,  500.0,  500.0 ), // 14
         new Vector(   0.0,  500.0,  500.0 ), // 15

         new Vector(   0.0,    0.0,    0.0 ), // 16
         new Vector( 500.0,    0.0,    0.0 ), // 17
         new Vector( 500.0,  500.0,    0.0 ), // 18
         new Vector(   0.0,  500.0,    0.0 ), // 19
         new Vector(   0.0,    0.0,  500.0 ), // 20
         new Vector( 500.0,    0.0,  500.0 ), // 21
         new Vector( 500.0,  500.0,  500.0 ), // 22
         new Vector(   0.0,  500.0,  500.0 ), // 23

         /* The hole corner vertices */

         new Vector( 100.0,  100.0,  0.0 ), // 24
         new Vector( 300.0,  100.0,  0.0 ), // 25
         new Vector( 300.0,  300.0,  0.0 ), // 26
         new Vector( 100.0,  300.0,  0.0 ), // 27

         new Vector( 100.0,  100.0, 500.0 ), // 28
         new Vector( 300.0,  100.0, 500.0 ), // 29
         new Vector( 300.0,  300.0, 500.0 ), // 30
         new Vector( 100.0,  300.0, 500.0 ), // 31

         new Vector( 100.0,  100.0,  0.0 ), // 32
         new Vector( 300.0,  100.0,  0.0 ), // 33
         new Vector( 300.0,  300.0,  0.0 ), // 34
         new Vector( 100.0,  300.0,  0.0 ), // 35

         new Vector( 100.0,  100.0, 500.0 ), // 36
         new Vector( 300.0,  100.0, 500.0 ), // 37
         new Vector( 300.0,  300.0, 500.0 ), // 38
         new Vector( 100.0,  300.0, 500.0 ), // 39                
     };

            var outloop = new[]
            {
         new[] {  0,   3,   2,   1 },
         new[] {  8,   9,  13,  12 },
         new[] { 17,  18,  22,  21 },
         new[] { 10,  11,  15,  14 },
         new[] { 19,  16,  20,  23 },
         new[] {  4,   5,   6,   7 },

         new[] { 31, 28, 24, 27 },
         new[] { 30, 39, 35, 26 },
         new[] { 29, 38, 34, 25 },
         new[] { 36, 37, 33, 32 }
     };

            var innerLoop = new Dictionary<int, int[][]>
            {
                { 0, new[] { new[] { 24, 25, 26, 27 } } },
                { 5, new[] { new[] { 28, 31, 30, 29 } } },
            };

            var normals = new Vector[]
            {
         // Basic cube vertex normals

         new Vector( 0,  0, -1 ),
         new Vector( 0,  0, -1 ),
         new Vector( 0,  0, -1 ),
         new Vector( 0,  0, -1 ),

         new Vector( 0,  0,  1 ),
         new Vector( 0,  0,  1 ),
         new Vector( 0,  0,  1 ),
         new Vector( 0,  0,  1 ),

         new Vector( 0,  0, -1 ), // CHANGE THIS TO (0, -1, 0) FOR A VISIBLE EDGE
         new Vector( 0,  0, -1 ), // CHANGE THIS TO (0, -1, 0) FOR A VISIBLE EDGE
         new Vector( 0,  1,  0 ),
         new Vector( 0,  1,  0 ),

         new Vector( 0, -1,  0 ),
         new Vector( 0, -1,  0 ),
         new Vector( 0,  1,  0 ),
         new Vector( 0,  1,  0 ),

         new Vector(-1,  0,  0 ),
         new Vector( 1,  0,  0 ),
         new Vector( 1,  0,  0 ),
         new Vector(-1,  0,  0 ),

         new Vector(-1,  0,  0 ),
         new Vector( 1,  0,  0 ),
         new Vector( 1,  0,  0 ),
         new Vector(-1,  0,  0 ),

         // Hole vertex normals

         new Vector(-1,  0,  0 ), // 24
         new Vector( 1,  0,  0 ), // 25
         new Vector( 0,  1,  0 ), // 26
         new Vector(-1,  0,  0 ), // 27

         new Vector(-1,  0,  0 ), // 28
         new Vector( 1,  0,  0 ), // 29
         new Vector( 0,  1,  0 ), // 30
         new Vector(-1,  0,  0 ), // 31

         new Vector( 0, -1,  0 ), // 32
         new Vector( 0, -1,  0 ), // 33
         new Vector( 1,  0,  0 ), // 34
         new Vector(-1,  0,  0 ), // 35 CHANGE THIS TO (0, 1, 0) FOR A VISIBLE EDGE

         new Vector( 0, -1,  0 ), // 36
         new Vector( 0, -1,  0 ), // 37
         new Vector( 1,  0,  0 ), // 38
         new Vector(-1,  0,  0 ), // 39 CHANGE THIS TO (0, 1, 0) FOR A VISIBLE EDGE
            };

            var brep = new FacetedBrepWithNormals(vertices, outloop, innerLoop, normals);

            var shapeItem = new ShapeItem
            {
                Name = "CubeWithHoleTwoEdgesHidden",
                ShapeFacetedBrep = brep,
                UpAxis = ShapeUpAxis.Z_Axis
            };

            var retVal = shapeItem.InsertUsingNormals();

            var brep2 = new Brep()
            {
                StartPoint = new Point(0, 8000, 0),
                EndPoint = new Point(1000, 8000, 0),
                Profile = { ProfileString = "CubeWithHoleTwoEdgesHidden" },
            };

            brep2.Insert();
        }

        public static void CheckAllShapes()
        {
            ShapeItem myShape = new ShapeItem();
            myShape.ShapeFacetedBrep = CreateBrepCube(1000);
            myShape.UpAxis = ShapeUpAxis.Z_Axis;

        }

        public static FacetedBrep CreateBrepCube(double length)
        {
            var vertices = new[]
            {
                new Vector(0.0,     0.0,    0.0), // 0
                new Vector(length,  0.0,    0.0), // 1
                new Vector(length,  length, 0.0), // 2
                new Vector(0.0,     length, 0.0), // 3
                new Vector(0.0,     0.0,    length), // 4
                new Vector(length,  0.0,    length), // 5
                new Vector(length,  length, length), // 6
                new Vector(0.0,     length, length), // 7
            };
            var outloop = new[] { new[] { 0, 3, 2, 1 },
                                  new[] { 0, 1, 5, 4 },
                                  new[] { 1, 2, 6, 5 },
                                  new[] { 2, 3, 7, 6 },
                                  new[] { 3, 0, 4, 7 },
                                  new[] { 4, 5, 6, 7 }};

            var innerLoop = new Dictionary<int, int[][]>
            {
            };

            return new FacetedBrep(vertices, outloop, innerLoop);
        }

        //static void F(PQX p,SQX s,string dir)
        //{

        //    FileStream fs = new FileStream(Path.Combine(dir, "COORD.dat"), FileMode.Create);
        //    StreamWriter sw = new StreamWriter(fs);

        //    double pk0 = p.StartPoint.PK;
        //    while (pk0 < p.PKList.Last().Item3)
        //    {
        //        try
        //        {
        //            double z0 = s.GetBG(pk0);
        //            double[] v = p.GetCoord(pk0);
        //            double x0 = v[0];
        //            double y0 = v[1];
        //            sw.WriteLine("{0:F3},{1:F3},{2:F3}", y0, x0, z0);
        //        }
        //        catch (Exception)
        //        {
        //            continue;
        //        }
        //        finally
        //        {
        //            pk0 += 10;
        //        }

        //    }
        //    sw.Flush();
        //    sw.Close();
        //    fs.Close();

        //}
    }

}
