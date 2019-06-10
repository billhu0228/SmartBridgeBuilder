using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADImport;
using CADImport.DWG;
using netDxf;
using netDxf.Entities;
using Tekla.Structures.Geometry3d;
using Point = Tekla.Structures.Geometry3d.Point;
using CoordinateSystem = Tekla.Structures.Geometry3d.CoordinateSystem;
using System.Reflection;

namespace SmartBridgeBuilder.Extension
{
    public static class NetCADTool
    {

        public static T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }

        public static CADEntity GetCopy(this CADEntity ent,ref DWGImage refdwg)
        {
            string typename = ent.GetType().Name;


            if (ent is CADSolid)
            {

                CADSolid src = (CADSolid)ent;

                CADSolid res = new CADSolid();
                res.Point = src.Point;
                res.Point1 = src.Point1;
                res.Point2 = src.Point2;
                res.Point3 = src.Point3;

                res.Color = src.Color;
                res.Layer = src.Layer;

                return res;

            }

            else if (ent is CADLine)
            {
                CADLine src = (CADLine)ent;                 

                CADLine res = new CADLine();
                res.Point = src.Point;
                res.Point1 = src.Point1;
                res.LineType = src.LineType;
                res.LineWeight = src.LineWeight;
                res.Color = src.Color;
                res.Layer = src.Layer;
                res.LineTypeScale = src.LineTypeScale;
                return res;
            }
            else if (ent is CADArc)
            {

                CADArc src = (CADArc)ent;
                CADArc res = new CADArc();
                res.Point = src.Point;
                res.Radius = src.Radius;
                res.LineType = src.LineType;
                res.LineWeight = src.LineWeight;
                res.Color = src.Color;
                res.Layer = src.Layer;
                res.StartAngle = src.StartAngle;
                res.EndAngle = src.EndAngle;

                res.LineTypeScale = src.LineTypeScale;
                return res;

            }
            else if (ent is CADCircle)
            {
               

                CADCircle src = (CADCircle)ent;
                CADCircle res = new CADCircle();
                res.Point = src.Point;
                res.Radius = src.Radius;
                res.LineType = src.LineType;
                res.LineWeight = src.LineWeight;
                res.Color = src.Color;
                res.Layer = src.Layer;

                res.LineTypeScale = src.LineTypeScale;
                return res;


            }
            else if (ent is CADLWPolyLine)
            {
                CADLWPolyLine res = new CADLWPolyLine();
                CADLWPolyLine src = (CADLWPolyLine)ent;

                foreach (var item in src.Vertexes)
                {
                    res.Vertexes.Add((CADVertex)item);
                }

                res.LineType = src.LineType;
                res.LineWeight = src.LineWeight;
                res.Color = src.Color;
                res.Layer = src.Layer;
                res.Closed = src.Closed;


                return res;
            }
            else if (ent is CADText)
            {
                CADText src = (CADText)ent;

                CADText res = new CADText();

                res.Point = src.Point;
                res.Color = src.Color;
                res.Layer = src.Layer;

                res.Rotation = src.Rotation;
                res.XScale = src.XScale;
                res.Style = refdwg.Converter.StyleByName("仿宋");
                res.Text = src.Text;
                res.VAlign = src.VAlign;
     
                res.Point1 = src.Point1;
                res.Extrusion = src.Extrusion;
                res.ObliqueAngle = src.ObliqueAngle;
                res.Height = src.Height;
                res.HAlign = src.HAlign;
                //res.FontName = src.FontName;
                //res.TextStyle = src.TextStyle;
                return res;
            }           
            else
            {
                return null;
            }
                       

            //switch (ent.GetType().Name)
            //{
            //    //case "CADLine":
            //    //    CADLine tent = (CADLine)ent;
            //    //    CADLine res = new CADLine();
            //    //    res.Point=tent.Point;
            //    //    res.Point1 = tent.Point1;
            //    //    res.LineType = tent.LineType;
            //    //    res.LineWeight = tent.LineWeight;
            //    //    res.Color = tent.Color;
            //    //    res.Layer = tent.Layer;
            //    //    res.LineTypeScale = tent.LineTypeScale;
            //    //    return res;
            //    //case "CADCircle":
            //    //    CADCircle cc = (CADCircle)ent;
            //    //    CADCircle res2 = new CADCircle();
            //    //    res2.Point = cc.Point;
            //    //    res2.Radius = cc.Radius;
            //    //    res2.LineType = cc.LineType;
            //    //    res2.LineWeight = cc.LineWeight;
            //    //    res2.Color = cc.Color;
            //    //    res2.Layer = cc.Layer;

            //    //    res2.LineTypeScale = cc.LineTypeScale;
            //    //    return res2;

            //    case "CADLWPolyLine":
            //        CADLWPolyLine resly = new CADLWPolyLine();
            //        CADLWPolyLine pl = (CADLWPolyLine)ent;

            //        foreach (var item in pl.Vertexes)
            //        {
            //            resly.Vertexes.Add((CADVertex)item);
            //        }

            //        resly.LineType = pl.LineType;
            //        resly.LineWeight = pl.LineWeight;
            //        resly.Color = pl.Color;
            //        resly.Layer = pl.Layer;
            //        resly.Closed = pl.Closed;


            //        return resly;


            //    default:
            //        return null;
                  
            //}
        }


        public static void AddRect(ref DWGImage  vDrawing, DPoint pt1, DPoint pt2)
        {

            CADLWPolyLine res = new CADLWPolyLine();

            CADVertex vert;
            vert = new CADVertex();
            vert.Point =new DPoint(pt1.X,pt1.Y,0);            
            res.Entities.Add(vert);

            vert = new CADVertex();
            vert.Point = new DPoint(pt2.X, pt1.Y, 0);
            res.Entities.Add(vert);



            vert = new CADVertex();
            vert.Point = new DPoint(pt2.X, pt2.Y, 0);
            res.Entities.Add(vert);


            vert = new CADVertex();
            vert.Point = new DPoint(pt1.X, pt2.Y, 0);
            res.Entities.Add(vert);

            res.Closed = true;

            vDrawing.Converter.Loads(res);
            vDrawing.Converter.OnCreate(res);
            vDrawing.CurrentLayout.AddEntity(res);





        }

        public static void AddPolygen(ref DWGImage vDrawing, DPoint[] PTlist,string lname="粗线")
        {
            CADLWPolyLine res = new CADLWPolyLine();

            CADVertex vert;
            foreach (var item in PTlist)
            {
                vert = new CADVertex();
                vert.Point = new DPoint(item.X, item.Y, 0);
                res.Entities.Add(vert);
            }

            res.Closed = true;

            res.Layer = vDrawing.Converter.LayerByName(lname);
            vDrawing.Converter.Loads(res);
            vDrawing.Converter.OnCreate(res);
            vDrawing.CurrentLayout.AddEntity(res);

        }

        public static bool CleanDxfFile(string dxffile)
        {
            DxfDocument dxfDocument = DxfDocument.Load(dxffile);
            bool res = false;
            try
            {
                var ff = dxfDocument.Blocks["defender"];
                MText tt = (MText)ff.Entities[0];
                tt.Value = "";
                res = true;
            }
            catch (Exception)
            {
                ;
            }
            finally
            {
                dxfDocument.Save(dxffile);
            }
            return res;
        }

        public static void AddBlock(ref DWGImage vDrawing,CADBlock blk,DPoint insertPt,int v2)
        {

            CADBlock bg = blk;

            CADInsert bgref = new CADInsert();

            bgref.Block = bg;
            bgref.Point = insertPt;
            bgref.Scale = new DPoint(v2, v2, v2);


            vDrawing.Converter.Loads(bgref);
           
            vDrawing.CurrentLayout.AddEntity(bgref);
        }

        public static void AddBlock(ref DWGImage vDrawing, string v1, DPoint inserPoint, int v2)
        {

            CADBlock bg = vDrawing.Converter.BlockByName(v1);

            CADInsert bgref = new CADInsert();
           
            bgref.Block = bg;
            bgref.Point = inserPoint;
            bgref.Scale = new DPoint(v2, v2, v2);
           
           
            vDrawing.Converter.Loads(bgref);
            vDrawing.Converter.OnCreate(bgref);
            vDrawing.CurrentLayout.AddEntity(bgref);

        }

        public static void InsertBG(ref DWGImage vDrawing, string v1, DPoint inserPoint, int v2)
        {
            CADBlock bg = vDrawing.Converter.BlockByName(v1);
            CADInsert bgref = new CADInsert();
            CADAttdef attdef = (CADAttdef)bg.Entities[1];
            CADAttrib attref = new CADAttrib();
            bgref.Block = bg;
            bgref.Point = inserPoint;
            attref.AssignEntity(attdef);
            attref.Value = string .Format("{0:F3}",inserPoint.Y/1000);
            attref.Height = attref.Height * v2;


            CoordinateSystem global = new CoordinateSystem();
            CoordinateSystem loc = new CoordinateSystem(new Point(attref.Point.X*v2+inserPoint.X,attref.Point.Y*v2+inserPoint.Y, 0), new Vector(1, 0, 0), new Vector(0, 1, 0));
            Matrix mat = MatrixFactory.ByCoordinateSystems(loc, global);
            CADMatrix CADmat = new CADMatrix();
            CADmat.Data = mat.GetData();



            var  ta1= CADmat.PtXMat(attref.Point);


            attref.Point = CADmat.PtXMat(attref.Point);
            attref.Point1 = attref.Point;
            bgref.Attribs.Add(attref);
            bgref.Scale = new DPoint(v2, v2, v2);
            bgref.Layer = vDrawing.Converter.LayerByName("标注");
            vDrawing.Converter.Loads(bgref);
            vDrawing.Converter.OnCreate(bgref);
            vDrawing.CurrentLayout.AddEntity(bgref);


        }

        public static void AddDim(ref DWGImage vDrawing, string v1, DPoint dPoint1, DPoint dPoint2, int v2,string text="")
        {
            CADDimension dim = new CADDimension(CADDimension.DimensionType.Aligned, dPoint1, dPoint2, v2, false);
            CADDimensionStyle dst = vDrawing.Converter.DimensionStyleByName(v1);
            dim.Style = dst;
            if (text!="")
            {
                dim.TextOverride = text;
            }
            dim.Layer = vDrawing.Converter.LayerByName("标注");
            vDrawing.Converter.Loads(dim);
            vDrawing.Converter.OnCreate(dim);
            vDrawing.CurrentLayout.AddEntity(dim);
        }
    }
}
