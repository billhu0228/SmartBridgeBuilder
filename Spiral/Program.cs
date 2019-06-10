using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBridgeBuilder.SubStructure;
namespace SmartBridgeBuilder.Spiral
{
    class Program
    {
        static void Main(string[] args)
        {
            string jdfile = @"C:\Users\Bill\Desktop\参考\M1\M1\M1.JD";
            string sqxfile = @"C:\Users\Bill\Desktop\参考\M1\M1\M1.SQX";
            string subfile = @"C:\Users\Bill\Desktop\参考\M1\M1\M1.XB";
            string dmxfile = @"C:\Users\Bill\Desktop\参考\M1\M1\M1.DMX";

            PQX bill = new PQX(jdfile);
            SQX kitty = new SQX(sqxfile);
            SUB oo = new SUB(subfile);
            DMX tony = new DMX(dmxfile);

            tony.GetBG(12040);

            FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(sqxfile),"COORD.dat"), FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            double pk0 = bill.StartPoint.PK;
            while (pk0<bill.PKList.Last().Item3)
            {
                try
                {
                    double z0 = kitty.GetBG(pk0);
                    double[] v = bill.GetCoord(pk0);
                    double x0 = v[0];
                    double y0 = v[1];
                    sw.WriteLine("{0:F3},{1:F3},{2:F3}",y0,x0,z0);
                }
                catch (Exception)
                {
                    continue;
                }
                finally
                {
                    pk0 += 10;
                }
                
            }            
            sw.Flush();            
            sw.Close();
            fs.Close();


            // 退出
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

    }
}