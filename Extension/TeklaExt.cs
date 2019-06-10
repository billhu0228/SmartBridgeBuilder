using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;

namespace SmartBridgeBuilder.Extension
{
    public static partial class Ext
    {

        public static double[]GetData(this Matrix mat)
        {
            double[] res = new double[16];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res[i * 4 + j] = mat[i, j];                
                }
            }

            //res[3] = mat[3, 0];
            //res[7] = mat[3, 1];
            //res[11] = mat[3, 2];
            res[15] = 1.0;


            return res;
        }

        public static Point MoveTo(this Point A, double dx, double dy, double dz)
        {
            return new Point(A.X + dx, A.Y + dy, A.Z + dz);
        }
        public static Point MirrorY(this Point Pt)
        {
            return new Point(Pt.X, -Pt.Y, Pt.Z);
        }


        public static Vector RotBy(this Vector v, double rad)
        {
            double x1 = v.X * Math.Cos(rad) - v.Y * Math.Sin(rad);
            double y1 = v.X * Math.Sin(rad) + v.Y * Math.Cos(rad);
            return new Vector(x1, y1, v.Z);
        }


        public static Point RotByZ(this Point pt,double rad)
        {
            double x1 = pt.X * Math.Cos(rad) - pt.Y * Math.Sin(rad);
            double y1 = pt.X * Math.Sin(rad) + pt.Y * Math.Cos(rad);
            return new Point(x1, y1, pt.Z);
        }
        public static Vector RotByZ(this Vector v, double rad)
        {
            double x1 = v.X * Math.Cos(rad) - v.Y * Math.Sin(rad);
            double y1 = v.X * Math.Sin(rad) + v.Y * Math.Cos(rad);
            return new Vector(x1, y1,0);
        }
    }




}
