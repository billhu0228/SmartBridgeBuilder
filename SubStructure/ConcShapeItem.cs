using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartBridgeBuilder.RoadDesign;
using Tekla.Structures.Model;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using SmartBridgeBuilder.Extension;

namespace SmartBridgeBuilder.Structure
{

    class CSI
    {
        public int typeID;
        public string shapeString;        
        public double offsetL, offsetT, offsetH;
        public double pk0,pk1, H;
    }

    public class ConcShapeItem
    {
        List<CSI> CSIList;

        public ConcShapeItem(string subfile)
        {
            CSIList = new List<CSI>();

            string[] altext = File.ReadAllLines(subfile);

            string root= Path.GetDirectoryName(subfile);


            foreach (string item in altext)
            {
                CSI cc = new CSI();
                if (item.StartsWith("//"))
                {
                    continue;
                }                
                try
                {
                    string line = item.TrimEnd('\r');
                    line = line.TrimEnd('\t');
                    var xx = Regex.Split(line, @"\s+");
                    cc.typeID = int.Parse(xx[0]);
                    cc.shapeString = xx[1];
                    cc.pk0 = double.Parse(xx[2]);
                    cc.pk1 = double.Parse(xx[3]);
                    cc.H = double.Parse(xx[4]);           
                    cc.offsetL = double.Parse(xx[5]);
                    cc.offsetT = double.Parse(xx[6]);
                    cc.offsetH = double.Parse(xx[7]);

                    CSIList.Add(cc);
                }
                catch (Exception)
                {
                    throw;
                }
            }


        }

        public void ToTekla(ref Model myModel, ref DesignLine maggie)
        {
            foreach (var item in CSIList)
            {
                try
                {
                    var x0y0 = maggie.pQX.GetCoord(item.pk0);
                    double x0 = x0y0[0] * 1000;
                    double y0 = x0y0[1] * 1000;
                    double z0 = item.H * 1000;

                    var x1y1 = maggie.pQX.GetCoord(item.pk1);
                    double x1 = x1y1[0] * 1000;
                    double y1 = x1y1[1] * 1000;


                    Vector dir = new Vector(x1 - x0, y1 - y0, 0);

                    CoordinateSystem global = new CoordinateSystem();
                    CoordinateSystem loc = new CoordinateSystem(new Point(x0, y0, z0), dir, dir.RotByZ(0.5 * Math.PI));
                    Matrix mat = MatrixFactory.ByCoordinateSystems(loc, global);

                    Point St = mat.Transform(new Point(0 + item.offsetL * 1000, 0 + item.offsetT * 1000, 0 + item.offsetH * 1000));
                    Point Ed = mat.Transform(new Point((1+ item.offsetL) * 1000, 0 + item.offsetT * 1000, 0 + item.offsetH * 1000));

                    var Beam = new Brep()
                    {
                        StartPoint = St,
                        EndPoint = Ed,
                        Profile = { ProfileString = item.shapeString },
                    };
                    Beam.Position.Plane = Position.PlaneEnum.MIDDLE;
                    Beam.Position.Depth = Position.DepthEnum.MIDDLE;

                    Beam.Insert();
                }
                catch (Exception)
                {
                    throw;
                }

            }
            
        }
    }
}
