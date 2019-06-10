using CADImport;
using CADImport.DWG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace SmartBridgeBuilder.RoadDesign
{
    public class DesignLine
    {
        public DMX dMX;
        public SQX sQX;
        public PQX pQX;
        public double Width;
        public BasePoint BP;

        public DesignLine(PQX bill, SQX kitty, DMX tony, int v)
        {
            pQX = bill;
            sQX = kitty;
            dMX = tony;
            Width = v;
        }

        public void ToTekla(ref Model myModel,double pkstart,double pkend,double dpk)
        {
            int n = 0;
            double pk0 = pkstart;
            Point pt0, pt1;
            Point dt0, dt1;
            pt0 = new Point(pQX.GetCoord(pk0)[0] * 1000, pQX.GetCoord(pk0)[1] * 1000, sQX.GetBG(pk0) * 1000);
            dt0 = new Point(pQX.GetCoord(pk0)[0] * 1000, pQX.GetCoord(pk0)[1] * 1000, dMX.GetBG(pk0) * 1000);
            ControlLine cl;
            while (pk0 < pkend)
            {
                try
                {
                    double z0 = sQX.GetBG(pk0)*1000;
                    double z1 = dMX.GetBG(pk0) * 1000;
                    double[] v = pQX.GetCoord(pk0);
                    double x0 = v[0] * 1000;
                    double y0 = v[1] * 1000;
                    pt1 = new Point(x0,y0, z0);
                    cl = new ControlLine(new LineSegment(pt0, pt1),false);
                    cl.Extension = 0;
                    cl.Color = ControlLine.ControlLineColorEnum.RED;
                    cl.Insert();
                    pt0 = pt1;

                    dt1 = new Point(x0, y0, z1);
                    cl = new ControlLine(new LineSegment(dt0, dt1), false);
                    cl.Extension = 0;
                    cl.Color = ControlLine.ControlLineColorEnum.BLACK;
                    cl.Insert();
                    dt0 = dt1;


                    n++;
                }
                catch (Exception)
                {
                    continue;
                }
                finally
                {
                    pk0 += dpk;
                }

            }

            myModel.CommitChanges();

        }

        public void ToDXF(ref DWGImage vDrawing, double pkstart, double pkend, double dpk)
        {

            int n = 0;
            double pk0 = pkstart;

            CADLWPolyLine gline = new CADLWPolyLine();
            CADLWPolyLine dline = new CADLWPolyLine();
            CADLWPolyLine wline = new CADLWPolyLine();
            CADLWPolyLine sline1 = new CADLWPolyLine();
            CADLWPolyLine sline2 = new CADLWPolyLine();

            CADVertex vert;
            DPoint pt;
            //vert.Point = fs1;
            //maincab.Entities.Add(vert);
            double zwater = -9000;

            while (pk0 < pkend)
            {
                try
                {
                    double z0 = sQX.GetBG(pk0) * 1000;
                    double z1 = dMX.GetBG(pk0) * 1000;
                    double zs1 = z0 - 400;
                    double zs2 = z0 - 4000;
                    


                    pt = new DPoint(pk0*1000,z0,0);
                    vert = new CADVertex();
                    vert.Point = pt;
                    dline.AddEntity(vert);

                    pt = new DPoint(pk0 * 1000, zs1, 0);
                    vert = new CADVertex();
                    vert.Point = pt;
                    sline1.AddEntity(vert);


                    pt = new DPoint(pk0 * 1000, zs2, 0);
                    vert = new CADVertex();
                    vert.Point = pt;
                    sline2.AddEntity(vert);

                    pt = new DPoint(pk0 * 1000, z1, 0);
                    vert = new CADVertex();
                    vert.Point = pt;
                    gline.AddEntity(vert);

                    if (z1<=zwater)
                    {
                        pt = new DPoint(pk0 * 1000, zwater, 0);
                        vert = new CADVertex();
                        vert.Point = pt;
                        wline.AddEntity(vert);
                    }


                }
                catch (Exception)
                {
                    throw new Exception("~!");
                }
                finally
                {
                    pk0 += dpk;
                }

            }

            dline.Layer = vDrawing.Converter.LayerByName("sjx");
            vDrawing.Converter.Loads(dline);
            vDrawing.Converter.OnCreate(dline);
            vDrawing.CurrentLayout.AddEntity(dline);

            sline1.Layer = vDrawing.Converter.LayerByName("细线");
            vDrawing.Converter.Loads(sline1);
            vDrawing.Converter.OnCreate(sline1);
            vDrawing.CurrentLayout.AddEntity(sline1);

            sline2.Layer = vDrawing.Converter.LayerByName("细线");
            vDrawing.Converter.Loads(sline2);
            vDrawing.Converter.OnCreate(sline2);
            vDrawing.CurrentLayout.AddEntity(sline2);


            wline.Layer = vDrawing.Converter.LayerByName("dmx");
            wline.Color = System.Drawing.Color.CadetBlue;
            vDrawing.Converter.Loads(wline);
            vDrawing.Converter.OnCreate(wline);
            vDrawing.CurrentLayout.AddEntity(wline);

            gline.Layer = vDrawing.Converter.LayerByName("dmx");
            vDrawing.Converter.Loads(gline);
            vDrawing.Converter.OnCreate(gline);
            vDrawing.CurrentLayout.AddEntity(gline);

        }
    }
}
